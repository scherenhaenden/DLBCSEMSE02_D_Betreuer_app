package com.example.betreuer_app;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Toast;
import android.widget.TextView;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisDocumentResponse;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class EditThesisActivity extends AppCompatActivity {

    private TextInputEditText etTitle;
    private TextInputEditText etDescription;
    private AutoCompleteTextView dropdownSubjectArea;
    private TextView tvCurrentDocument;
    private MaterialButton btnDownloadDocument;
    private MaterialButton btnUploadDocument;
    private MaterialButton btnFindTutors;
    private MaterialButton btnSave;

    private ThesisApiService thesisApiService;
    private SubjectAreaRepository subjectAreaRepository;
    private SubjectAreaApiService subjectAreaApiService;
    private String thesisId;
    private ThesisApiModel currentThesis;
    private Uri selectedDocumentUri;

    /** Map to store the mapping between Subject Area names (displayed) and their IDs (values). */
    private java.util.Map<String, String> subjectAreaMap = new java.util.HashMap<>();
    private java.util.List<String> allSubjectAreaNames = new java.util.ArrayList<>();

    private ActivityResultLauncher<String> filePickerLauncher;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        try {
            setContentView(R.layout.activity_edit_thesis);

            etTitle = findViewById(R.id.et_thesis_title);
            etDescription = findViewById(R.id.et_thesis_description);
            dropdownSubjectArea = findViewById(R.id.dropdown_subject_area);
            tvCurrentDocument = findViewById(R.id.tv_current_document);
            btnDownloadDocument = findViewById(R.id.btn_download_document);
            btnUploadDocument = findViewById(R.id.btn_upload_document);
            btnSave = findViewById(R.id.btn_save_thesis);
            btnFindTutors = findViewById(R.id.btn_find_tutors);

            MaterialToolbar toolbar = findViewById(R.id.toolbar);
            if (toolbar != null) {
                toolbar.setNavigationOnClickListener(v -> finish());
            }

            thesisApiService = createThesisApiService();
            subjectAreaRepository = createSubjectAreaRepository();
            subjectAreaApiService = createSubjectAreaApiService();

            if (getIntent().hasExtra("THESIS_ID")) {
                thesisId = getIntent().getStringExtra("THESIS_ID");
                loadThesisDetails(thesisId);
            } else {
                Toast.makeText(this, "Thesis ID not provided", Toast.LENGTH_SHORT).show();
                finish();
                return;
            }

            btnSave.setOnClickListener(v -> saveThesisDetails());
            btnDownloadDocument.setOnClickListener(v -> downloadDocument());
            btnUploadDocument.setOnClickListener(v -> selectDocumentForUpload());
            btnFindTutors.setOnClickListener(v -> findTutors());

            filePickerLauncher = registerForActivityResult(new ActivityResultContracts.GetContent(),
                    uri -> {
                        if (uri != null) {
                            selectedDocumentUri = uri;
                            tvCurrentDocument.setText("Neues Dokument ausgewählt: " + getFileName(uri));
                            btnUploadDocument.setEnabled(true);
                        }
                    });

            // Load subject areas for the dropdown
            loadSubjectAreas();
            setupSubjectAreaSearch();

            Toast.makeText(this, "Activity erfolgreich geladen", Toast.LENGTH_SHORT).show();

        } catch (Exception e) {
            Toast.makeText(this, "Fehler beim Laden der Activity: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    protected ThesisApiService createThesisApiService() {
        return ApiClient.getThesisApiService(this);
    }

    protected SubjectAreaRepository createSubjectAreaRepository() {
        return new SubjectAreaRepository(getApplicationContext());
    }

    protected SubjectAreaApiService createSubjectAreaApiService() {
        return ApiClient.getSubjectAreaApiService(this);
    }

    private void loadThesisDetails(String id) {
        try {
            thesisApiService.getThesis(id).enqueue(new Callback<ThesisApiModel>() {
                @Override
                public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                    try {
                        if (response.isSuccessful() && response.body() != null) {
                            currentThesis = response.body();
                            etTitle.setText(currentThesis.getTitle());
                            etDescription.setText(currentThesis.getDescription());
                            tvCurrentDocument.setText(currentThesis.getDocumentFileName() != null ? currentThesis.getDocumentFileName() : "Kein Dokument hochgeladen");

                            // Set the selected subject area
                            if (currentThesis.getSubjectAreaId() != null) {
                                String subjectAreaId = currentThesis.getSubjectAreaId().toString();
                                String subjectAreaName = getSubjectAreaNameById(subjectAreaId);
                                if (subjectAreaName != null) {
                                    dropdownSubjectArea.setText(subjectAreaName, false);
                                } else {
                                    // Load the specific subject area if not in the map
                                    loadSpecificSubjectArea(subjectAreaId);
                                }
                            }

                            // Update document display
                            updateDocumentDisplay();
                        } else {
                            Toast.makeText(EditThesisActivity.this, "Failed to load thesis details", Toast.LENGTH_SHORT).show();
                        }
                    } catch (Exception e) {
                        Toast.makeText(EditThesisActivity.this, "Fehler beim Verarbeiten: " + e.getMessage(), Toast.LENGTH_LONG).show();
                        e.printStackTrace();
                    }
                }

                @Override
                public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                    Toast.makeText(EditThesisActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_LONG).show();
                    t.printStackTrace();
                }
            });
        } catch (Exception e) {
            Toast.makeText(this, "Fehler beim Laden der Details: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private void saveThesisDetails() {
        try {
            String title = etTitle.getText().toString().trim();
            String description = etDescription.getText().toString().trim();
            String selectedSubjectAreaName = dropdownSubjectArea.getText().toString();

            if (title.isEmpty()) {
                Toast.makeText(this, "Titel ist erforderlich", Toast.LENGTH_SHORT).show();
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

            RequestBody titlePart = RequestBody.create(MediaType.parse("text/plain"), title);
            RequestBody descriptionPart = RequestBody.create(MediaType.parse("text/plain"), description);
            RequestBody subjectAreaIdPart = subjectAreaId != null
                ? RequestBody.create(MediaType.parse("text/plain"), subjectAreaId)
                : null;

            thesisApiService.updateThesis(thesisId, titlePart, descriptionPart, subjectAreaIdPart, null).enqueue(new Callback<ThesisApiModel>() {
                @Override
                public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                    if (response.isSuccessful()) {
                        Toast.makeText(EditThesisActivity.this, "Änderungen erfolgreich gespeichert", Toast.LENGTH_SHORT).show();
                        finish();
                    } else {
                        Toast.makeText(EditThesisActivity.this, "Fehler beim Speichern der Änderungen", Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                    Toast.makeText(EditThesisActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        } catch (Exception e) {
            Toast.makeText(this, "Fehler beim Speichern der Änderungen: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private void uploadDocument() {
        try {
            if (selectedDocumentUri == null) {
                Toast.makeText(this, "Kein Dokument ausgewählt", Toast.LENGTH_SHORT).show();
                return;
            }

            File file = createFileFromUri(selectedDocumentUri);
            String mimeType = getContentResolver().getType(selectedDocumentUri);
            if (mimeType == null) {
                mimeType = "application/octet-stream";
            }
            RequestBody requestFile = RequestBody.create(MediaType.parse(mimeType), file);
            MultipartBody.Part body = MultipartBody.Part.createFormData("document", file.getName(), requestFile);

            thesisApiService.updateThesisDocument(thesisId, body).enqueue(new Callback<ThesisDocumentResponse>() {
                @Override
                public void onResponse(Call<ThesisDocumentResponse> call, Response<ThesisDocumentResponse> response) {
                    try {
                        if (response.isSuccessful()) {
                            Toast.makeText(EditThesisActivity.this, "Dokument erfolgreich hochgeladen", Toast.LENGTH_SHORT).show();
                            finish();
                        } else {
                            try {
                                String errorBody = response.errorBody() != null ? response.errorBody().string() : "Unknown error";
                                Toast.makeText(EditThesisActivity.this, "Fehler beim Hochladen: " + errorBody, Toast.LENGTH_LONG).show();
                            } catch (IOException e) {
                                Toast.makeText(EditThesisActivity.this, "Fehler beim Hochladen (Code: " + response.code() + ")", Toast.LENGTH_SHORT).show();
                            }
                        }
                    } catch (Exception e) {
                        Toast.makeText(EditThesisActivity.this, "Fehler: " + e.getMessage(), Toast.LENGTH_LONG).show();
                        e.printStackTrace();
                    }
                }

                @Override
                public void onFailure(Call<ThesisDocumentResponse> call, Throwable t) {
                    Toast.makeText(EditThesisActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
                    t.printStackTrace();
                }
            });
        } catch (IOException e) {
            Toast.makeText(this, "Fehler beim Verarbeiten der Datei: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        } catch (Exception e) {
            Toast.makeText(this, "Unerwarteter Fehler: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private void downloadDocument() {
        try {
            if (currentThesis == null || currentThesis.getDocumentFileName() == null) {
                Toast.makeText(this, "Kein Dokument zum Herunterladen verfügbar", Toast.LENGTH_SHORT).show();
                return;
            }

            thesisApiService.downloadThesisDocument(thesisId).enqueue(new Callback<okhttp3.ResponseBody>() {
                @Override
                public void onResponse(Call<okhttp3.ResponseBody> call, Response<okhttp3.ResponseBody> response) {
                    try {
                        if (response.isSuccessful() && response.body() != null) {
                            saveDownloadedFile(response.body(), currentThesis.getDocumentFileName());
                        } else {
                            Toast.makeText(EditThesisActivity.this, "Fehler beim Herunterladen des Dokuments", Toast.LENGTH_SHORT).show();
                        }
                    } catch (Exception e) {
                        Toast.makeText(EditThesisActivity.this, "Fehler beim Speichern: " + e.getMessage(), Toast.LENGTH_LONG).show();
                        e.printStackTrace();
                    }
                }

                @Override
                public void onFailure(Call<okhttp3.ResponseBody> call, Throwable t) {
                    Toast.makeText(EditThesisActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
                    t.printStackTrace();
                }
            });
        } catch (Exception e) {
            Toast.makeText(this, "Fehler beim Download: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private void selectDocumentForUpload() {
        try {
            filePickerLauncher.launch("*/*");
        } catch (Exception e) {
            Toast.makeText(this, "Fehler beim Öffnen der Dateiauswahl: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private File createFileFromUri(Uri uri) throws IOException {
        try {
            File tempFile = File.createTempFile("upload", ".tmp", getCacheDir());
            try (InputStream inputStream = getContentResolver().openInputStream(uri);
                 FileOutputStream outputStream = new FileOutputStream(tempFile)) {
                if (inputStream == null) {
                    throw new IOException("Cannot open input stream");
                }
                byte[] buffer = new byte[1024];
                int length;
                while ((length = inputStream.read(buffer)) > 0) {
                    outputStream.write(buffer, 0, length);
                }
            }
            return tempFile;
        } catch (Exception e) {
            e.printStackTrace();
            throw new IOException("Failed to create file from URI: " + e.getMessage(), e);
        }
    }

    private String getFileName(Uri uri) {
        try {
            String result = null;
            if (uri.getScheme() != null && uri.getScheme().equals("content")) {
                try (android.database.Cursor cursor = getContentResolver().query(uri, null, null, null, null)) {
                    if (cursor != null && cursor.moveToFirst()) {
                        int index = cursor.getColumnIndex(android.provider.OpenableColumns.DISPLAY_NAME);
                        if (index >= 0) {
                            result = cursor.getString(index);
                        }
                    }
                }
            }
            if (result == null) {
                result = uri.getPath();
                if (result != null) {
                    int cut = result.lastIndexOf('/');
                    if (cut != -1) {
                        result = result.substring(cut + 1);
                    }
                }
            }
            if (result == null) {
                result = "document";
            }
            return result;
        } catch (Exception e) {
            e.printStackTrace();
            return "document";
        }
    }

    private void saveDownloadedFile(okhttp3.ResponseBody body, String fileName) {
        try {
            File downloadsDir = android.os.Environment.getExternalStoragePublicDirectory(android.os.Environment.DIRECTORY_DOWNLOADS);
            File file = new File(downloadsDir, fileName);

            try (InputStream inputStream = body.byteStream();
                 FileOutputStream outputStream = new FileOutputStream(file)) {

                byte[] buffer = new byte[4096];
                int length;
                while ((length = inputStream.read(buffer)) > 0) {
                    outputStream.write(buffer, 0, length);
                }
                outputStream.flush();
            }

            Toast.makeText(this, "Dokument heruntergeladen: " + file.getAbsolutePath(), Toast.LENGTH_LONG).show();

        } catch (IOException e) {
            Toast.makeText(this, "Fehler beim Speichern der Datei: " + e.getMessage(), Toast.LENGTH_LONG).show();
            e.printStackTrace();
        }
    }

    private void loadSubjectAreas() {
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
                     loadSubjectAreas();
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

        // Add item click listener to handle selection
        dropdownSubjectArea.setOnItemClickListener((parent, view, position, id) -> {
            String selectedItem = (String) parent.getItemAtPosition(position);
            if (selectedItem != null) {
                dropdownSubjectArea.setText(selectedItem, false);
            }
        });
    }

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

    private void updateDropdown(java.util.List<SubjectAreaResponse> areas) {
        if (areas != null) {
            boolean hasNewItems = false;

            for (SubjectAreaResponse area : areas) {
                String name = area.getTitle();
                java.util.UUID id = area.getId();

                if (name != null && id != null && !subjectAreaMap.containsKey(name)) {
                    allSubjectAreaNames.add(name);
                    subjectAreaMap.put(name, id.toString());
                    hasNewItems = true;
                }
            }

            // Only update adapter if we have new items
            if (hasNewItems) {
                ArrayAdapter<String> adapter = new ArrayAdapter<>(
                        EditThesisActivity.this,
                        android.R.layout.simple_dropdown_item_1line,
                        allSubjectAreaNames);
                dropdownSubjectArea.setAdapter(adapter);
            }
        }
    }

    private String getSubjectAreaNameById(String id) {
        for (java.util.Map.Entry<String, String> entry : subjectAreaMap.entrySet()) {
            if (entry.getValue().equals(id)) {
                return entry.getKey();
            }
        }
        return null;
    }

    private void loadSpecificSubjectArea(String subjectAreaId) {
        subjectAreaApiService.getSubjectArea(java.util.UUID.fromString(subjectAreaId)).enqueue(new Callback<SubjectAreaResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponse> call, Response<SubjectAreaResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    SubjectAreaResponse area = response.body();
                    String name = area.getTitle();
                    java.util.UUID id = area.getId();

                    if (name != null && id != null) {
                        subjectAreaMap.put(name, id.toString());
                        if (!allSubjectAreaNames.contains(name)) {
                            allSubjectAreaNames.add(name);
                            // Update adapter with new item
                            ArrayAdapter<String> adapter = new ArrayAdapter<>(
                                    EditThesisActivity.this,
                                    android.R.layout.simple_dropdown_item_1line,
                                    allSubjectAreaNames);
                            dropdownSubjectArea.setAdapter(adapter);
                        }
                        dropdownSubjectArea.setText(name, false);
                    }
                } else {
                    Toast.makeText(EditThesisActivity.this, "Fehler beim Laden des Fachgebiets", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponse> call, Throwable t) {
                Toast.makeText(EditThesisActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
                t.printStackTrace();
            }
        });
    }

    private void updateDocumentDisplay() {
        if (currentThesis != null && currentThesis.getDocumentFileName() != null && !currentThesis.getDocumentFileName().isEmpty()) {
            btnDownloadDocument.setVisibility(View.VISIBLE);
            btnDownloadDocument.setText("Exposé herunterladen (" + currentThesis.getDocumentFileName() + ")");
            tvCurrentDocument.setVisibility(View.GONE); // Hide the text view since button shows the filename
        } else {
            btnDownloadDocument.setVisibility(View.GONE);
            tvCurrentDocument.setVisibility(View.VISIBLE);
            tvCurrentDocument.setText("Kein Dokument hochgeladen");
        }
    }

    private void findTutors() {
        if (currentThesis != null && currentThesis.getSubjectAreaId() != null) {
            Intent intent = new Intent(EditThesisActivity.this, TutorListActivity.class);
            intent.putExtra("SELECTED_SUBJECT_AREA_ID", currentThesis.getSubjectAreaId().toString());
            startActivity(intent);
        } else {
            Toast.makeText(this, "Kein Fachgebiet für die Thesis ausgewählt", Toast.LENGTH_SHORT).show();
        }
    }
}
