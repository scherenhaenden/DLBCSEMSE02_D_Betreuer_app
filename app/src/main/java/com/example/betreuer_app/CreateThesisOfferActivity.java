package com.example.betreuer_app;

import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.CreateThesisOfferRequest;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesisOfferApiModel;
import com.example.betreuer_app.model.UpdateThesisOfferRequest;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.repository.ThesisOfferRepository;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.textfield.TextInputEditText;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CreateThesisOfferActivity extends AppCompatActivity {

    public static final String EXTRA_OFFER_ID = "offer_id";
    public static final String EXTRA_OFFER_TITLE = "offer_title";
    public static final String EXTRA_OFFER_DESCRIPTION = "offer_description";
    public static final String EXTRA_OFFER_SUBJECT_AREA_ID = "offer_subject_area_id";

    private TextInputEditText etTitle;
    private TextInputEditText etDescription;
    private AutoCompleteTextView dropdownSubjectArea;
    private Button btnSave;
    private MaterialToolbar toolbar;

    private ThesisOfferRepository thesisOfferRepository;
    private SubjectAreaRepository subjectAreaRepository;

    private Map<String, UUID> subjectAreaMap = new HashMap<>();
    
    // Variables for edit mode
    private boolean isEditMode = false;
    private UUID offerId = null;
    private UUID preselectedSubjectAreaId = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_thesis_offer);

        thesisOfferRepository = new ThesisOfferRepository(this);
        subjectAreaRepository = new SubjectAreaRepository(this);

        etTitle = findViewById(R.id.et_thesis_title);
        etDescription = findViewById(R.id.et_thesis_description);
        dropdownSubjectArea = findViewById(R.id.subject_area_dropdown);
        btnSave = findViewById(R.id.btn_save_thesis_offer);
        toolbar = findViewById(R.id.toolbar);

        toolbar.setNavigationOnClickListener(v -> finish());
        
        checkForEditMode();

        loadSubjectAreas();

        btnSave.setOnClickListener(v -> saveThesisOffer());
    }
    
    private void checkForEditMode() {
        if (getIntent().hasExtra(EXTRA_OFFER_ID)) {
            isEditMode = true;
            String idStr = getIntent().getStringExtra(EXTRA_OFFER_ID);
            if (idStr != null) {
                offerId = UUID.fromString(idStr);
            }
            
            toolbar.setTitle("Ausschreibung bearbeiten");
            btnSave.setText("Änderungen speichern");
            
            String title = getIntent().getStringExtra(EXTRA_OFFER_TITLE);
            String description = getIntent().getStringExtra(EXTRA_OFFER_DESCRIPTION);
            String subjectAreaIdStr = getIntent().getStringExtra(EXTRA_OFFER_SUBJECT_AREA_ID);
            
            if (title != null) etTitle.setText(title);
            if (description != null) etDescription.setText(description);
            if (subjectAreaIdStr != null) preselectedSubjectAreaId = UUID.fromString(subjectAreaIdStr);
        }
    }

    private void loadSubjectAreas() {
        subjectAreaRepository.getSubjectAreas(1, 50, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<SubjectAreaResponse> areas = response.body().getItems();
                    if (areas != null) {
                        List<String> areaNames = new ArrayList<>();
                        for (SubjectAreaResponse area : areas) {
                            String name = area.getTitle();
                            UUID id = area.getId();
                            
                            if (name != null && id != null) {
                                areaNames.add(name);
                                subjectAreaMap.put(name, id);
                                
                                // Pre-select subject area in edit mode
                                if (isEditMode && preselectedSubjectAreaId != null && preselectedSubjectAreaId.equals(id)) {
                                    dropdownSubjectArea.setText(name, false);
                                }
                            }
                        }

                        ArrayAdapter<String> adapter = new ArrayAdapter<>(
                                CreateThesisOfferActivity.this,
                                android.R.layout.simple_dropdown_item_1line,
                                areaNames
                        );
                        dropdownSubjectArea.setAdapter(adapter);
                    }
                } else {
                    Toast.makeText(CreateThesisOfferActivity.this, "Failed to load subject areas", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                Toast.makeText(CreateThesisOfferActivity.this, "Error loading subject areas: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void saveThesisOffer() {
        String title = etTitle.getText() != null ? etTitle.getText().toString().trim() : "";
        String description = etDescription.getText() != null ? etDescription.getText().toString().trim() : "";
        String selectedSubjectAreaName = dropdownSubjectArea.getText().toString();

        if (title.isEmpty()) {
            etTitle.setError("Titel ist erforderlich");
            return;
        }

        if (selectedSubjectAreaName.isEmpty() || !subjectAreaMap.containsKey(selectedSubjectAreaName)) {
            dropdownSubjectArea.setError("Bitte wählen Sie einen gültigen Fachbereich");
            return;
        }

        UUID subjectAreaId = subjectAreaMap.get(selectedSubjectAreaName);
        
        btnSave.setEnabled(false);

        if (isEditMode) {
            updateExistingThesisOffer(title, description, subjectAreaId);
        } else {
            createNewThesisOffer(title, description, subjectAreaId);
        }
    }
    
    private void createNewThesisOffer(String title, String description, UUID subjectAreaId) {
        CreateThesisOfferRequest request = new CreateThesisOfferRequest(title, subjectAreaId);
        if (!description.isEmpty()) {
            request.setDescription(description);
        }

        thesisOfferRepository.createThesisOffer(request, new Callback<ThesisOfferApiModel>() {
            @Override
            public void onResponse(Call<ThesisOfferApiModel> call, Response<ThesisOfferApiModel> response) {
                btnSave.setEnabled(true);
                if (response.isSuccessful()) {
                    Toast.makeText(CreateThesisOfferActivity.this, "Ausschreibung erfolgreich erstellt!", Toast.LENGTH_LONG).show();
                    finish();
                } else {
                    Toast.makeText(CreateThesisOfferActivity.this, "Fehler beim Erstellen: " + response.code(), Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisOfferApiModel> call, Throwable t) {
                btnSave.setEnabled(true);
                Toast.makeText(CreateThesisOfferActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
            }
        });
    }

    private void updateExistingThesisOffer(String title, String description, UUID subjectAreaId) {
        UpdateThesisOfferRequest request = new UpdateThesisOfferRequest(title, description, subjectAreaId);
        
        thesisOfferRepository.updateThesisOffer(offerId, request, new Callback<ThesisOfferApiModel>() {
            @Override
            public void onResponse(Call<ThesisOfferApiModel> call, Response<ThesisOfferApiModel> response) {
                btnSave.setEnabled(true);
                if (response.isSuccessful()) {
                    Toast.makeText(CreateThesisOfferActivity.this, "Ausschreibung erfolgreich aktualisiert!", Toast.LENGTH_LONG).show();
                    finish();
                } else {
                    Toast.makeText(CreateThesisOfferActivity.this, "Fehler beim Aktualisieren: " + response.code(), Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisOfferApiModel> call, Throwable t) {
                btnSave.setEnabled(true);
                Toast.makeText(CreateThesisOfferActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
            }
        });
    }
}
