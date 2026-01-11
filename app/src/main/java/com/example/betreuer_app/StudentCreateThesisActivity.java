package com.example.betreuer_app;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.provider.OpenableColumns;
import android.text.Editable;
import android.text.TextWatcher;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
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
 * Users can input the thesis title, search for a subject area (topic) via an autocomplete dropdown,
 * and optionally upload a document (e.g., PDF) to accompany the thesis proposal.
 *
 * <p>Key Features:
 * <ul>
 *     <li>Title Input: Required field for the thesis title.</li>
 *     <li>Subject Area Search: AutoCompleteTextView that queries the API for matching subject areas as the user types.</li>
 *     <li>Document Upload: Option to select a file from the device storage.</li>
 * </ul>
 *
 * <p>Upon successful creation, the activity displays a success message and closes.
 * If creation fails, an error message is shown.
 */
public class StudentCreateThesisActivity extends AppCompatActivity {

    /** Input field for entering the thesis title. */
    private TextInputEditText etTitle;

    /** Input field for entering the thesis description. */
    private TextInputEditText etDescription;

    /** AutoCompleteTextView for searching and selecting the subject area (topic). */
    private AutoCompleteTextView dropdownSubjectArea;

    /** Button to trigger the thesis creation process. */
    private MaterialButton btnCreate;

    /** Button to open the file picker. */
    private MaterialButton btnSelectFile;

    /** TextView to display the name of the selected file. */
    private TextView tvSelectedFile;

    /** Repository for handling thesis-related API operations. */
    private ThesisRepository thesisRepository;

    /** Repository for handling subject area-related API operations. */
    private SubjectAreaRepository subjectAreaRepository;

    /** Map to store the mapping between Subject Area names (displayed) and their IDs (values). */
    private Map<String, String> subjectAreaMap = new HashMap<>();

    /** URI of the file selected by the user for upload. Null if no file selected. */
    private Uri selectedFileUri = null;

    /**
     * Called when the activity is starting. This method initializes the UI components,
     * sets up the repositories, configures the subject area search behavior,
     * and initializes the file picker and button click listeners.
     *
     * @param savedInstanceState If the activity is being re-initialized after previously being shut down,
     * this Bundle contains the data it most recently supplied in onSaveInstanceState.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.student_activity_create_thesis);

        etTitle = findViewById(R.id.et_thesis_title);
        etDescription = findViewById(R.id.et_thesis_description);
        dropdownSubjectArea = findViewById(R.id.subject_area_dropdown);
        btnCreate = findViewById(R.id.btn_create_thesis);
        btnSelectFile = findViewById(R.id.btn_select_file);
        tvSelectedFile = findViewById(R.id.tv_selected_file);

        thesisRepository = new ThesisRepository(getApplicationContext());
        subjectAreaRepository = new SubjectAreaRepository(getApplicationContext());

        // Load the initial list of subject areas (e.g., top 100) to populate the dropdown before searching.
        loadInitialSubjectAreas();

        // Setup text watcher and listeners for the search functionality
        setupSubjectAreaSearch();

        // File Selection Launcher using ActivityResultContracts
        ActivityResultLauncher<Intent> filePickerLauncher = registerForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                result -> {
                    if (result.getResultCode() == Activity.RESULT_OK && result.getData() != null) {
                        selectedFileUri = result.getData().getData();
                        if (selectedFileUri != null) {
                            String fileName = getFileName(selectedFileUri);
                            tvSelectedFile.setText(fileName != null ? fileName : "Datei ausgewählt");
                        }
                    }
                }
        );

        btnSelectFile.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
            intent.setType("*/*"); // Allow all file types; restrict to "application/pdf" etc. if needed
            intent.addCategory(Intent.CATEGORY_OPENABLE);
            filePickerLauncher.launch(intent);
        });

        btnCreate.setOnClickListener(v -> {
            String title = String.valueOf(etTitle.getText()).trim();
            String description = String.valueOf(etDescription.getText()).trim();
            String selectedSubjectAreaName = dropdownSubjectArea.getText().toString();

            if (title.isEmpty()) {
                etTitle.setError("Titel ist erforderlich");
                return;
            }

            String subjectAreaId = null;
            // Validate and resolve topic ID from the map
            if (!selectedSubjectAreaName.isEmpty() && subjectAreaMap.containsKey(selectedSubjectAreaName)) {
                subjectAreaId = subjectAreaMap.get(selectedSubjectAreaName);
            } else if (!selectedSubjectAreaName.isEmpty()) {
                dropdownSubjectArea.setError("Bitte wählen Sie ein gültiges Fachgebiet aus der Suche");
                return;
            }

            btnCreate.setEnabled(false);

            // Choose the appropriate API method based on whether a file was selected
            if (selectedFileUri != null) {
                createThesisWithFile(title, description, subjectAreaId, selectedFileUri);
            } else {
                createThesis(title, description, subjectAreaId);
            }
        });

        com.google.android.material.appbar.MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());
    }

    /**
     * Helper method to extract the display name of a file from its URI.
     * Queries the content resolver for OpenableColumns.DISPLAY_NAME.
     *
     * @param uri The URI of the file.
     * @return The file name, or the last segment of the path if query fails.
     */
    private String getFileName(Uri uri) {
        String result = null;
        if (uri.getScheme() != null && uri.getScheme().equals("content")) {
            try (Cursor cursor = getContentResolver().query(uri, null, null, null, null)) {
                if (cursor != null && cursor.moveToFirst()) {
                    int index = cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME);
                    if(index >= 0) {
                        result = cursor.getString(index);
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        if (result == null) {
            result = uri.getPath();
            int cut = result.lastIndexOf('/');
            if (cut != -1) {
                result = result.substring(cut + 1);
            }
        }
        return result;
    }

    /**
     * Loads the initial list of subject areas from the API to populate the dropdown.
     * This ensures the user sees some options immediately when opening the dropdown.
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
                // Silently fail, user can still attempt to search
            }
        });
    }

    /**
     * Sets up the search functionality for the subject area dropdown.
     * Adds a TextWatcher to trigger API searches as the user types (debounce could be added for optimization).
     * Handles focus and click events to show the dropdown appropriately.
     */
    private void setupSubjectAreaSearch() {
        dropdownSubjectArea.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (s.length() >= 2) { 
                    // Start searching after 2 characters
                    performSearch(s.toString());
                } else if (s.length() == 0) {
                     // If cleared, reload initial list so dropdown isn't empty
                     loadInitialSubjectAreas();
                }
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Ensure dropdown shows when focused even if empty (showing initial list)
        dropdownSubjectArea.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus && dropdownSubjectArea.getText().length() == 0) {
                 dropdownSubjectArea.showDropDown();
            }
        });

        dropdownSubjectArea.setOnClickListener(v -> dropdownSubjectArea.showDropDown());
    }

    /**
     * Performs a search for subject areas using the API search endpoint.
     * Updates the dropdown adapter with the search results.
     *
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

    /**
     * Updates the dropdown adapter with a list of subject areas.
     * Also updates the internal map for name-to-ID resolution.
     *
     * @param areas The list of subject areas to display.
     */
    private void updateDropdown(List<SubjectAreaResponse> areas) {
        if (areas != null) {
            List<String> areaNames = new ArrayList<>();
            subjectAreaMap.clear();

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

            // If user is typing, filtering might hide results, force show to display new API results
            if (dropdownSubjectArea.hasFocus()) {
                dropdownSubjectArea.showDropDown();
            }
        }
    }

    /**
     * Initiates the thesis creation process (without file) by calling the API.
     *
     * @param title The title of the thesis.
     * @param description The optional description of the thesis.
     * @param subjectAreaId The optional subject area ID.
     */
    private void createThesis(String title, String description, String subjectAreaId) {
        thesisRepository.createThesis(title, description, subjectAreaId, new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                handleResponse(response);
            }
            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                handleFailure(t);
            }
        });
    }

    /**
     * Initiates the thesis creation process WITH a file upload by calling the API.
     *
     * @param title The title of the thesis.
     * @param description The optional description of the thesis.
     * @param subjectAreaId The optional subject area ID.
     * @param fileUri The URI of the selected file to upload.
     */
    private void createThesisWithFile(String title, String description, String subjectAreaId, Uri fileUri) {
        thesisRepository.createThesisWithFile(title, description, subjectAreaId, fileUri, new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                handleResponse(response);
            }
            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                handleFailure(t);
            }
        });
    }

    /**
     * Handles successful or failed API responses.
     * Re-enables the create button and shows appropriate toast messages.
     *
     * @param response The retrofit response.
     */
    private void handleResponse(Response<ThesisApiModel> response) {
        btnCreate.setEnabled(true);
        if (response.isSuccessful()) {
            Toast.makeText(StudentCreateThesisActivity.this, "Thesis erfolgreich erstellt", Toast.LENGTH_SHORT).show();
            finish(); // Close activity and go back
        } else {
            Toast.makeText(StudentCreateThesisActivity.this, "Fehler beim Erstellen: " + response.code(), Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Handles API failure (network error, etc.).
     *
     * @param t The throwable error.
     */
    private void handleFailure(Throwable t) {
        btnCreate.setEnabled(true);
        Toast.makeText(StudentCreateThesisActivity.this, "Fehler: " + (t != null ? t.getMessage() : "Unbekannt"), Toast.LENGTH_SHORT).show();
    }
}
