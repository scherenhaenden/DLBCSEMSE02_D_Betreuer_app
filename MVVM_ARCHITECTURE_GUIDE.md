# MVVM Architecture - Implementierungs-Guide

## Architektur-Überblick

```
┌─────────────────────────────────────────────────────────────┐
│                         Presentation Layer                   │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────────┐ │
│  │   Activity    │◄─│  ViewModel   │◄─│  ViewModelFactory│ │
│  │  (UI Logic)   │  │ (Business    │  │  (DI)            │ │
│  │               │  │  Logic)      │  │                  │ │
│  └───────────────┘  └──────────────┘  └──────────────────┘ │
└────────────────────────────┬────────────────────────────────┘
                             │
                             │ LiveData<Resource<T>>
                             │
┌────────────────────────────┴────────────────────────────────┐
│                         Domain Layer                         │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────────┐ │
│  │  Repository   │◄─│  Use Cases   │◄─│  Validation      │ │
│  │               │  │  (Optional)  │  │  Logic           │ │
│  └───────────────┘  └──────────────┘  └──────────────────┘ │
└────────────────────────────┬────────────────────────────────┘
                             │
                             │ API Calls / Data Access
                             │
┌────────────────────────────┴────────────────────────────────┐
│                         Data Layer                           │
│  ┌───────────────┐  ┌──────────────┐  ┌──────────────────┐ │
│  │  API Service  │  │SessionManager│  │  Local DB        │ │
│  │  (Retrofit)   │  │(SharedPrefs) │  │  (Optional)      │ │
│  └───────────────┘  └──────────────┘  └──────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Komponenten-Beschreibung

### 1. Presentation Layer

#### Activity (View)
- **Verantwortung**: Nur UI-Updates und User-Interaktionen
- **Sollte NICHT**:
  - API-Calls machen
  - Business Logic enthalten
  - Daten transformieren
- **Sollte**:
  - ViewModel observieren (LiveData)
  - UI-Events an ViewModel delegieren
  - UI-State anzeigen (Loading, Success, Error)

**Beispiel:**
```java
public class EditThesisActivity extends AppCompatActivity {
    private EditThesisViewModel viewModel;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit_thesis);
        
        viewModel = createViewModel();
        setupObservers();
        
        btnSave.setOnClickListener(v -> saveThesis());
    }
    
    private void setupObservers() {
        viewModel.getSaveResult().observe(this, resource -> {
            if (resource.isLoading()) {
                showLoading();
            } else if (resource.isSuccess()) {
                showSuccess();
            } else if (resource.isError()) {
                showError(resource.getMessage());
            }
        });
    }
    
    private void saveThesis() {
        String title = etTitle.getText().toString();
        String description = etDescription.getText().toString();
        viewModel.saveThesis(title, description);
    }
}
```

#### ViewModel
- **Verantwortung**: Business Logic, State Management
- **Sollte NICHT**:
  - Android-Kontext halten (außer ApplicationContext)
  - Views referenzieren
  - Lifecycle-aware Components direkt nutzen
- **Sollte**:
  - LiveData exposen
  - Repository/Use Cases nutzen
  - Input validieren
  - State managen

**Beispiel:**
```java
public class EditThesisViewModel extends ViewModel {
    private final ThesisRepository repository;
    private final MutableLiveData<Resource<Thesis>> saveResult = new MutableLiveData<>();
    
    public EditThesisViewModel(ThesisRepository repository) {
        this.repository = repository;
    }
    
    public LiveData<Resource<Thesis>> getSaveResult() {
        return saveResult;
    }
    
    public void saveThesis(String title, String description) {
        // Validation
        if (title.trim().isEmpty()) {
            saveResult.setValue(Resource.error("Title required", null));
            return;
        }
        
        // Loading state
        saveResult.setValue(Resource.loading(null));
        
        // Business logic
        repository.saveThesis(title, description, new Callback<Thesis>() {
            @Override
            public void onResponse(Call<Thesis> call, Response<Thesis> response) {
                if (response.isSuccessful()) {
                    saveResult.setValue(Resource.success(response.body()));
                } else {
                    saveResult.setValue(Resource.error("Save failed", null));
                }
            }
            
            @Override
            public void onFailure(Call<Thesis> call, Throwable t) {
                saveResult.setValue(Resource.error(t.getMessage(), null));
            }
        });
    }
}
```

#### ViewModelFactory
- **Verantwortung**: ViewModel-Instanzen mit Dependencies erstellen
- Implementiert `ViewModelProvider.Factory`

**Beispiel:**
```java
public class ViewModelFactory implements ViewModelProvider.Factory {
    private final ThesisRepository repository;
    
    public ViewModelFactory(ThesisRepository repository) {
        this.repository = repository;
    }
    
    @NonNull
    @Override
    public <T extends ViewModel> T create(@NonNull Class<T> modelClass) {
        if (modelClass.isAssignableFrom(EditThesisViewModel.class)) {
            return (T) new EditThesisViewModel(repository);
        }
        throw new IllegalArgumentException("Unknown ViewModel class");
    }
}
```

### 2. Domain Layer

#### Repository
- **Verantwortung**: Data Access Abstraction
- **Sollte**:
  - API Service kapseln
  - Datenquellen koordinieren (API + Local DB)
  - Caching implementieren (optional)
  - Error Mapping

**Beispiel:**
```java
public class ThesisRepository {
    private final ThesisApiService apiService;
    
    public ThesisRepository(ThesisApiService apiService) {
        this.apiService = apiService;
    }
    
    public void saveThesis(String title, String description, Callback<Thesis> callback) {
        RequestBody titlePart = RequestBody.create(MediaType.parse("text/plain"), title);
        RequestBody descPart = RequestBody.create(MediaType.parse("text/plain"), description);
        
        apiService.updateThesis(titlePart, descPart).enqueue(callback);
    }
}
```

#### Use Cases (Optional, für komplexe Operationen)
- **Verantwortung**: Multi-Step Business Operations
- **Beispiel**: CreateThesisUseCase (Validierung → Upload → Notification)

```java
public class CreateThesisUseCase {
    private final ThesisRepository thesisRepo;
    private final DocumentRepository docRepo;
    
    public void execute(ThesisData data, Document doc, Callback<Result> callback) {
        // Step 1: Validate
        if (!validate(data)) {
            callback.onError("Validation failed");
            return;
        }
        
        // Step 2: Upload document
        docRepo.upload(doc, new Callback<String>() {
            @Override
            public void onSuccess(String docId) {
                // Step 3: Create thesis with document ID
                data.setDocumentId(docId);
                thesisRepo.create(data, callback);
            }
            
            @Override
            public void onError(String error) {
                callback.onError(error);
            }
        });
    }
}
```

### 3. Data Layer

#### API Service (Retrofit)
- **Verantwortung**: HTTP Requests
- Interface mit Retrofit-Annotations

```java
public interface ThesisApiService {
    @Multipart
    @PUT("api/thesis/{id}")
    Call<Thesis> updateThesis(
        @Part("title") RequestBody title,
        @Part("description") RequestBody description
    );
}
```

#### SessionManager
- **Verantwortung**: Auth State, Token Management
- Wrapper für SharedPreferences

```java
public class SessionManager {
    public void saveUserSession(String token, String userId, String email, String role);
    public boolean isLoggedIn();
    public String getToken();
    public void clearSession();
}
```

## Resource Wrapper Pattern

```java
public class Resource<T> {
    public enum Status { SUCCESS, ERROR, LOADING }
    
    private final Status status;
    private final T data;
    private final String message;
    
    public static <T> Resource<T> success(T data) {
        return new Resource<>(Status.SUCCESS, data, null);
    }
    
    public static <T> Resource<T> error(String message, T data) {
        return new Resource<>(Status.ERROR, data, message);
    }
    
    public static <T> Resource<T> loading(T data) {
        return new Resource<>(Status.LOADING, data, null);
    }
}
```

**Verwendung in Activity:**
```java
viewModel.getData().observe(this, resource -> {
    switch (resource.getStatus()) {
        case LOADING:
            progressBar.setVisibility(View.VISIBLE);
            break;
        case SUCCESS:
            progressBar.setVisibility(View.GONE);
            displayData(resource.getData());
            break;
        case ERROR:
            progressBar.setVisibility(View.GONE);
            showError(resource.getMessage());
            break;
    }
});
```

## Best Practices

### 1. ViewModel Lifecycle
- ViewModels überleben Configuration Changes (Screen Rotation)
- Cleanup in `onCleared()` (z.B. RxJava Subscriptions)
- Niemals Activity/Fragment/View in ViewModel halten

### 2. LiveData vs. MutableLiveData
- **MutableLiveData**: Intern im ViewModel (private)
- **LiveData**: Exposed für Activity/Fragment (public)

```java
private final MutableLiveData<Data> _data = new MutableLiveData<>();
public LiveData<Data> getData() {
    return _data;
}
```

### 3. Error Handling
- Einheitliches Error-Format über Resource
- Spezifische Error-Messages für User
- Logging für Developer

### 4. Testing

#### ViewModel Tests (Unit Tests)
```java
@Test
public void saveThesis_withEmptyTitle_returnsError() {
    // Given
    EditThesisViewModel viewModel = new EditThesisViewModel(mockRepository);
    
    // When
    viewModel.saveThesis("", "Description");
    
    // Then
    Resource<Thesis> result = viewModel.getSaveResult().getValue();
    assertTrue(result.isError());
    assertEquals("Title required", result.getMessage());
}
```

#### Activity Tests (Integration Tests mit Robolectric)
```java
@Test
public void saveButton_withValidInput_callsViewModel() {
    // Given
    activity = Robolectric.buildActivity(EditThesisActivity.class).create().get();
    EditThesisViewModel mockViewModel = mock(EditThesisViewModel.class);
    TestEditThesisActivity.mockViewModel = mockViewModel;
    
    // When
    etTitle.setText("Title");
    btnSave.performClick();
    
    // Then
    verify(mockViewModel).saveThesis(eq("Title"), any());
}
```

## Migration Checklist

- [ ] Erstelle ViewModel-Klasse
- [ ] Extrahiere Business Logic aus Activity
- [ ] Erstelle LiveData für alle State-Changes
- [ ] Implementiere Observer-Setup in Activity
- [ ] Aktualisiere ViewModelFactory
- [ ] Passe Tests an
- [ ] Teste thoroughly (Unit + Integration)
- [ ] Dokumentiere Changes

## Abhängigkeiten (build.gradle.kts)

```kotlin
dependencies {
    // Lifecycle Components
    implementation("androidx.lifecycle:lifecycle-viewmodel:2.6.1")
    implementation("androidx.lifecycle:lifecycle-livedata:2.6.1")
    implementation("androidx.lifecycle:lifecycle-runtime:2.6.1")
    
    // Testing
    testImplementation("androidx.arch.core:core-testing:2.2.0") // InstantTaskExecutorRule
}
```

