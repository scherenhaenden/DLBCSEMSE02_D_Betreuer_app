package com.example.betreuer_app;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * StudentCreateThesisActivity provides a user interface for creating a new thesis.
 * Users can input the thesis title and search for a subject area (topic) via an autocomplete dropdown.
 * Upon successful creation, the activity displays a success message and closes.
 * If creation fails, an error message is shown.
 */
public class StudentCreateThesisActivity extends AppCompatActivity {

    /** Input field for entering the thesis title. */
    private TextInputEditText etTitle;

    /** AutoCompleteTextView for searching and selecting the subject area (topic). */
    private AutoCompleteTextView dropdownSubjectArea;

    /** Button to trigger the thesis creation process. */
    private MaterialButton btnCreate;

    /** Repository for handling thesis-related API operations. */
    private ThesisRepository thesisRepository;

    /** Repository for handling subject area-related API operations. */
    private SubjectAreaRepository subjectAreaRepository;

    /** Map to store the mapping between Subject Area names and their IDs. */
    private Map<String, String> subjectAreaMap = new HashMap<>();

    /**
     * Called when the activity is starting. This method initializes the UI components,
     * sets up the repositories, sets up the search functionality, and configures the button click listener.
     * @param savedInstanceState If the activity is being re-initialized after previously being shut down,
     * this Bundle contains the data it most recently supplied in onSaveInstanceState.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.student_activity_create_thesis);

        etTitle = findViewById(R.id.et_thesis_title);
        dropdownSubjectArea = findViewById(R.id.subject_area_dropdown);
        btnCreate = findViewById(R.id.btn_create_thesis);

        thesisRepository = new ThesisRepository(getApplicationContext());
        subjectAreaRepository = new SubjectAreaRepository(getApplicationContext());

        // Initial load of subject areas (e.g. top 100)
        loadInitialSubjectAreas();
        
        setupSubjectAreaSearch();

        btnCreate.setOnClickListener(v -> {
            String title = String.valueOf(etTitle.getText()).trim();
            String selectedSubjectAreaName = dropdownSubjectArea.getText().toString();

            if (title.isEmpty()) {
                etTitle.setError("Titel ist erforderlich");
                return;
            }

            String topicId = null;
            if (!selectedSubjectAreaName.isEmpty() && subjectAreaMap.containsKey(selectedSubjectAreaName)) {
                topicId = subjectAreaMap.get(selectedSubjectAreaName);
            } else if (!selectedSubjectAreaName.isEmpty()) {
                dropdownSubjectArea.setError("Bitte wählen Sie ein gültiges Fachgebiet aus der Suche");
                return;
            }

            btnCreate.setEnabled(false);
            createThesis(title, topicId);
        });
        
        com.google.android.material.appbar.MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());
    }

    /**
     * Loads the initial list of subject areas to populate the dropdown before searching.
     */
    private void loadInitialSubjectAreas() {
        subjectAreaRepository.getSubjectAreas(1, 100, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    updateDropdown(response.body().getItems());
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                // Silently fail, user can still search
            }
        });
    }

    /**
     * Sets up the search functionality for the subject area dropdown.
     * Adds a TextWatcher to trigger API searches as the user types.
     */
    private void setupSubjectAreaSearch() {
        dropdownSubjectArea.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (s.length() >= 2) { // Start searching after 2 characters
                    performSearch(s.toString());
                } else if (s.length() == 0) {
                     // If cleared, reload initial list
                     loadInitialSubjectAreas();
                }
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });
        
        // Ensure dropdown shows when focused even if empty
        dropdownSubjectArea.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus && dropdownSubjectArea.getText().length() == 0) {
                 dropdownSubjectArea.showDropDown();
            }
        });
        
        dropdownSubjectArea.setOnClickListener(v -> dropdownSubjectArea.showDropDown());
    }

    /**
     * Performs a search for subject areas using the API.
     * Updates the dropdown adapter with the search results.
     * @param query The search query entered by the user.
     */
    private void performSearch(String query) {
        subjectAreaRepository.searchSubjectAreas(query, 1, 20, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    updateDropdown(response.body().getItems());
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                // Silently fail for search suggestions to avoid spamming user
            }
        });
    }
    
    private void updateDropdown(List<SubjectAreaResponse> areas) {
        if (areas != null) {
            List<String> areaNames = new ArrayList<>();
            
            for (SubjectAreaResponse area : areas) {
                String name = area.getTitle();
                UUID id = area.getId();
                
                if (name != null && id != null) {
                    areaNames.add(name);
                    subjectAreaMap.put(name, id.toString());
                }
            }

            ArrayAdapter<String> adapter = new ArrayAdapter<>(
                    StudentCreateThesisActivity.this,
                    android.R.layout.simple_dropdown_item_1line,
                    areaNames
            );
            dropdownSubjectArea.setAdapter(adapter);
            
            // If user is typing, filtering might hide results, force show
            if (dropdownSubjectArea.hasFocus()) {
                dropdownSubjectArea.showDropDown();
            }
        }
    }

    /**
     * Initiates the thesis creation process by calling the API with the provided title and topic ID.
     * Handles the API response asynchronously, showing success or error messages accordingly.
     * @param title The title of the thesis to be created. Must not be null or empty.
     * @param topicId The optional topic ID associated with the thesis. Can be null if not specified.
     */
    private void createThesis(String title, String topicId) {
        thesisRepository.createThesis(title, topicId, new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                btnCreate.setEnabled(true);
                if (response.isSuccessful()) {
                    Toast.makeText(StudentCreateThesisActivity.this, "Thesis erfolgreich erstellt", Toast.LENGTH_SHORT).show();
                    finish(); // Close activity and go back
                } else {
                    Toast.makeText(StudentCreateThesisActivity.this, "Fehler beim Erstellen: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                btnCreate.setEnabled(true);
                Toast.makeText(StudentCreateThesisActivity.this, "Fehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
