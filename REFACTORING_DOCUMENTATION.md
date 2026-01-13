# Refactoring-Dokumentation: Android Activities - Business Logic Extraktion

## Zusammenfassung

Dieses Refactoring extrahiert die Business Logic aus Android Activities in testbare ViewModels und Utility-Klassen nach dem MVVM-Pattern (Model-View-ViewModel).

## Implementierte Komponenten

### 1. Basis-Infrastruktur

#### SessionManager (`util/SessionManager.java`)
- **Zweck**: Zentralisiert alle SharedPreferences-Operationen für Authentication
- **Features**:
  - `saveUserSession()` - Speichert Login-Daten
  - `isLoggedIn()` - Prüft Login-Status
  - `getToken()`, `getUserId()`, `getUserEmail()`, `getUserRole()` - Getter für User-Daten
  - `clearSession()` - Logout
  - `hasRole()`, `isTutor()`, `isStudent()` - Rollen-Prüfungen

#### Resource Wrapper (`util/Resource.java`)
- **Zweck**: Einheitliches Wrapper-Pattern für API-Responses
- **Status-Types**: SUCCESS, ERROR, LOADING
- **Vorteile**: Konsistentes Error-Handling, UI kann zwischen Loading/Success/Error unterscheiden

### 2. ViewModels

#### EditThesisViewModel (`viewmodel/EditThesisViewModel.java`)
- **Extrahierte Logic aus EditThesisActivity**:
  - `loadThesisDetails()` - Lädt Thesis-Daten
  - `loadSubjectAreas()` - Lädt Fachgebiete
  - `searchSubjectAreas()` - Sucht Fachgebiete
  - `validateThesisInput()` - Validiert User-Input
  - `saveThesisDetails()` - Speichert Änderungen
  - `uploadDocument()` - Upload von Dokumenten
  - `downloadDocument()` - Download von Dokumenten
- **LiveData Observables**:
  - `thesisDetails` - Thesis-Daten
  - `subjectAreas` - Fachgebiete-Liste
  - `saveResult` - Speicher-Ergebnis
  - `uploadResult` - Upload-Ergebnis
  - `downloadResult` - Download-Ergebnis

#### LoginViewModel (`viewmodel/LoginViewModel.java`)
- **Extrahierte Logic aus LoginActivity**:
  - `validateInputs()` - Input-Validierung
  - `login()` - Login-Operation
  - `checkAutoLogin()` - Auto-Login-Check mit Token-Validierung
  - `getUserInfo()` - User-Info für Navigation
  - `logout()` - Logout
- **LiveData Observables**:
  - `loginResult` - Login-Ergebnis
  - `autoLoginResult` - Auto-Login-Ergebnis

#### ViewModelFactory (`viewmodel/ViewModelFactory.java`)
- **Zweck**: Dependency Injection für ViewModels
- Unterstützt:
  - EditThesisViewModel (mit ThesisApiService, SubjectAreaRepository)
  - LoginViewModel (mit LoginRepository, SessionManager)

### 3. Aktualisierte Activities

#### EditThesisActivity (Refactored)
- **Neue Struktur**:
  - `createViewModel()` - Factory-Method für ViewModel (testbar)
  - `setupObservers()` - Observiert LiveData vom ViewModel
  - `displayThesisDetails()` - Zeigt Daten in UI
- **Entfernte Logik**:
  - API-Calls direkt → jetzt im ViewModel
  - Validierung → jetzt im ViewModel
  - Subject Area Mapping → jetzt im ViewModel
- **Rückwärtskompatibilität**: Factory-Methods erlauben Mock-Injection in Tests

#### LoginRepository (Erweitert)
- Neue Methode: `validateToken()` - Validiert Token durch Test-API-Call
- Unterstützt Auto-Login-Flow

## Vorteile des Refactorings

### 1. Testbarkeit
- **Unit-Tests für ViewModels**: ViewModels können ohne Android-Framework getestet werden
- **Isolation**: Business Logic ist von UI-Framework entkoppelt
- **Mocking**: Dependencies werden über Konstruktoren injiziert

### 2. Wartbarkeit
- **Separation of Concerns**: UI-Code (Activity) vs. Business Logic (ViewModel)
- **Wiederverwendbarkeit**: ViewModels können von mehreren Activities/Fragments genutzt werden
- **Lesbarkeit**: Activities sind jetzt "Thin Controllers" (nur UI-Updates)

### 3. Robustheit
- **Lifecycle-Awareness**: ViewModels überleben Configuration Changes
- **Einheitliches Error-Handling**: Resource-Wrapper standardisiert Fehlerbehandlung
- **Zentralisierte Session-Verwaltung**: Ein Ort für Auth-Logic

## Noch zu refactorierende Activities

Die folgenden Activities enthalten noch Business Logic und sollten refactoriert werden:

### Hohe Priorität
1. **DashboardActivity** - Dashboard-Logic
2. **ThesisListActivity** - List-Loading, Pagination
3. **StudentCreateThesisActivity** - Thesis-Erstellung

### Mittlere Priorität
4. **ThesisDetailActivity** - Detail-Ansicht-Logic
5. **TutorListActivity** - Tutor-Suche-Logic
6. **TutorProfileActivity** - Profile-Display-Logic

### Niedrige Priorität
7. **ThesisOfferDashboardActivity**
8. **ThesisOfferDetailActivity**
9. **ThesisRequestActivity**
10. **ThesisRequestDetailActivity**
11. **SupervisionRequestActivity**
12. **CreateThesisOfferActivity**
13. **MainActivity**

## Migration-Pattern für weitere Activities

```java
// 1. Erstelle ViewModel
public class XyzViewModel extends ViewModel {
    private final XyzApiService apiService;
    private final MutableLiveData<Resource<Data>> data = new MutableLiveData<>();
    
    public XyzViewModel(XyzApiService apiService) {
        this.apiService = apiService;
    }
    
    public LiveData<Resource<Data>> getData() {
        return data;
    }
    
    public void loadData() {
        data.setValue(Resource.loading(null));
        apiService.getData().enqueue(new Callback<Data>() {
            @Override
            public void onResponse(Call<Data> call, Response<Data> response) {
                if (response.isSuccessful() && response.body() != null) {
                    data.setValue(Resource.success(response.body()));
                } else {
                    data.setValue(Resource.error("Error message", null));
                }
            }
            
            @Override
            public void onFailure(Call<Data> call, Throwable t) {
                data.setValue(Resource.error(t.getMessage(), null));
            }
        });
    }
}

// 2. Activity nutzt ViewModel
public class XyzActivity extends AppCompatActivity {
    private XyzViewModel viewModel;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_xyz);
        
        // Initialize ViewModel
        viewModel = createViewModel();
        
        // Setup observers
        setupObservers();
        
        // Load data
        viewModel.loadData();
    }
    
    protected XyzViewModel createViewModel() {
        XyzApiService apiService = createApiService();
        ViewModelFactory factory = new ViewModelFactory(apiService);
        return new ViewModelProvider(this, factory).get(XyzViewModel.class);
    }
    
    protected XyzApiService createApiService() {
        return ApiClient.getXyzApiService(this);
    }
    
    private void setupObservers() {
        viewModel.getData().observe(this, resource -> {
            if (resource.isSuccess()) {
                displayData(resource.getData());
            } else if (resource.isError()) {
                showError(resource.getMessage());
            } else if (resource.isLoading()) {
                showLoading();
            }
        });
    }
}
```

## Test-Kompatibilität

### EditThesisActivityTest (Anpassungen erforderlich)
Die bestehenden Tests in `EditThesisActivityTest.java` müssen minimal angepasst werden:

```java
public static class TestEditThesisActivity extends EditThesisActivity {
    public static EditThesisViewModel mockViewModel;
    
    @Override
    protected EditThesisViewModel createViewModel() {
        return mockViewModel;
    }
}
```

Die Factory-Method-Pattern erlauben es, das ViewModel in Tests zu mocken, ohne die gesamte Test-Struktur zu ändern.

## Nächste Schritte

1. **Tests aktualisieren**: EditThesisActivityTest an neues ViewModel-Pattern anpassen
2. **DashboardActivity refactorieren**: Als nächste kritische Activity
3. **LoginActivity refactorieren**: LoginViewModel integrieren
4. **Use-Cases einführen**: Für komplexe Multi-Step-Operationen (z.B. Thesis-Erstellung mit Validierung + Upload)
5. **Weitere Activities migrieren**: Nach Priority-Liste oben

## Wichtige Hinweise

- **Keine Breaking Changes**: Alle Activities funktionieren weiterhin, alte Logic ist kommentiert/parallel
- **Inkrementelle Migration**: Jede Activity kann einzeln migriert werden
- **Tests bleiben lauffähig**: Factory-Methods sichern Test-Kompatibilität
- **Konsistentes Pattern**: Alle neuen ViewModels folgen dem gleichen Muster

