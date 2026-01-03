package com.example.betreuer_app;

import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.CreateThesisOfferRequest;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesisOfferApiModel;
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

    private TextInputEditText etTitle;
    private TextInputEditText etDescription;
    private AutoCompleteTextView dropdownSubjectArea;
    private Button btnSave;
    
    private ThesisOfferRepository thesisOfferRepository;
    private SubjectAreaRepository subjectAreaRepository;
    
    // Map to store subject area names and their corresponding IDs
    private Map<String, UUID> subjectAreaMap = new HashMap<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_thesis_offer);

        // Initialize Repositories
        thesisOfferRepository = new ThesisOfferRepository(this);
        subjectAreaRepository = new SubjectAreaRepository(this);

        // Initialize UI Components
        etTitle = findViewById(R.id.et_thesis_title);
        etDescription = findViewById(R.id.et_thesis_description);
        dropdownSubjectArea = findViewById(R.id.subject_area_dropdown);
        btnSave = findViewById(R.id.btn_save_thesis_offer);
        MaterialToolbar toolbar = findViewById(R.id.toolbar);

        // Setup Toolbar
        toolbar.setNavigationOnClickListener(v -> finish());

        // Load Subject Areas for Dropdown
        loadSubjectAreas();

        // Setup Save Button
        btnSave.setOnClickListener(v -> saveThesisOffer());
    }

    private void loadSubjectAreas() {
        // Fetch subject areas from API (e.g., first 50)
        subjectAreaRepository.getSubjectAreas(1, 50, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<SubjectAreaResponse> areas = response.body().getItems();
                    if (areas != null) {
                        List<String> areaNames = new ArrayList<>();
                        for (SubjectAreaResponse area : areas) {
                            String name = area.getTitle(); // Changed from getName() to getTitle()
                            UUID id = area.getId();       
                            
                            if (name != null && id != null) {
                                areaNames.add(name);
                                subjectAreaMap.put(name, id);
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

        // Prepare Request
        CreateThesisOfferRequest request = new CreateThesisOfferRequest(title, subjectAreaId);
        if (!description.isEmpty()) {
            request.setDescription(description);
        }
        
        btnSave.setEnabled(false); // Prevent double clicks

        thesisOfferRepository.createThesisOffer(request, new Callback<ThesisOfferApiModel>() {
            @Override
            public void onResponse(Call<ThesisOfferApiModel> call, Response<ThesisOfferApiModel> response) {
                btnSave.setEnabled(true);
                if (response.isSuccessful()) {
                    Toast.makeText(CreateThesisOfferActivity.this, "Ausschreibung erfolgreich erstellt!", Toast.LENGTH_LONG).show();
                    finish(); // Close activity and go back to list
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
}
