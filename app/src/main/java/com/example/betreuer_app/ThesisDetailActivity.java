package com.example.betreuer_app;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.BillingStatusResponse;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.UserResponse;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;

import java.util.List;
import java.util.UUID;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisDetailActivity extends AppCompatActivity {

    private TextView textViewTitle;
    private TextView textViewDescription;
    private TextView textViewStatus;
    private TextView textViewSubjectArea;
    private MaterialButton btnDownloadDocument;
    private MaterialButton btnEditThesis;
    private Spinner spinnerBillingStatus;

    // Views for the person details
    private TextView ownerName;
    private TextView tutorName;
    private TextView secondSupervisorName;

    private ThesisApiService thesisApiService;
    private UserApiService userApiService;
    private SubjectAreaApiService subjectAreaApiService;

    private FileDownloader fileDownloader;

    private String thesisId;
    private ThesisApiModel currentThesis;
    private ThesisApiModel thesisToDownload;
    private ActivityResultLauncher<String> requestPermissionLauncher;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_detail);

        fileDownloader = new FileDownloader();

        requestPermissionLauncher =
                registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                    if (isGranted) {
                        if (thesisToDownload != null) {
                            downloadDocument(thesisToDownload);
                        }
                    } else {
                        Toast.makeText(this, "Permission denied to write to storage", Toast.LENGTH_SHORT).show();
                    }
                });

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        // Initialize views from the main layout
        textViewTitle = findViewById(R.id.textViewTitle);
        textViewDescription = findViewById(R.id.textViewDescription);
        textViewStatus = findViewById(R.id.textViewStatus);
        textViewSubjectArea = findViewById(R.id.textViewSubjectArea);
        btnDownloadDocument = findViewById(R.id.btn_download_document);
        btnEditThesis = findViewById(R.id.btn_edit_thesis);
        spinnerBillingStatus = findViewById(R.id.spinner_billingstatus);

        // Initialize views from the included person layouts
        View ownerItem = findViewById(R.id.item_owner);
        ((TextView) ownerItem.findViewById(R.id.person_label)).setText("Student");
        ownerName = ownerItem.findViewById(R.id.person_name);

        View tutorItem = findViewById(R.id.item_tutor);
        ((TextView) tutorItem.findViewById(R.id.person_label)).setText("Betreuer");
        tutorName = tutorItem.findViewById(R.id.person_name);

        View secondSupervisorItem = findViewById(R.id.item_second_supervisor);
        ((TextView) secondSupervisorItem.findViewById(R.id.person_label)).setText("Zweitkorrektor");
        secondSupervisorName = secondSupervisorItem.findViewById(R.id.person_name);

        thesisApiService = ApiClient.getThesisApiService(this);
        userApiService = ApiClient.getUserApiService(this);
        subjectAreaApiService = ApiClient.getSubjectAreaApiService(this);

        if (getIntent().hasExtra("THESIS_ID")) {
            thesisId = getIntent().getStringExtra("THESIS_ID");
            loadBillingStatuses(); // This will load statuses, then thesis details
        } else {
            Toast.makeText(this, "Thesis ID not provided", Toast.LENGTH_SHORT).show();
            finish();
        }

        btnEditThesis.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisDetailActivity.this, EditThesisActivity.class);
            intent.putExtra("THESIS_ID", thesisId);
            startActivity(intent);
        });
    }

    private void loadBillingStatuses() {
        thesisApiService.getBillingStatuses().enqueue(new Callback<List<BillingStatusResponse>>() {
            @Override
            public void onResponse(Call<List<BillingStatusResponse>> call, Response<List<BillingStatusResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    setupBillingStatusSpinner(response.body());
                    loadThesisDetails(thesisId);
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to load billing statuses", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<List<BillingStatusResponse>> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error loading billing statuses: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setupBillingStatusSpinner(List<BillingStatusResponse> statuses) {
        ArrayAdapter<BillingStatusResponse> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, statuses);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinnerBillingStatus.setAdapter(adapter);

        spinnerBillingStatus.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                BillingStatusResponse selectedStatus = (BillingStatusResponse) parent.getItemAtPosition(position);
                if (currentThesis != null && !selectedStatus.getName().equals(currentThesis.getBillingStatus())) {
                    updateBillingStatus(selectedStatus);
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
    }

    private void updateBillingStatus(BillingStatusResponse newStatus) {
        if (currentThesis == null) return;

        ThesisApiService.BillingStatusUpdateRequest request = new ThesisApiService.BillingStatusUpdateRequest(newStatus.getId());

        thesisApiService.updateBillingStatus(currentThesis.getId().toString(), request).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(ThesisDetailActivity.this, "Billing status updated successfully", Toast.LENGTH_SHORT).show();
                    currentThesis.setBillingStatus(newStatus.getName());
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to update billing status", Toast.LENGTH_SHORT).show();
                    spinnerBillingStatus.setSelection(((ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter()).getPosition(getBillingStatusByName(currentThesis.getBillingStatus())));
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                spinnerBillingStatus.setSelection(((ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter()).getPosition(getBillingStatusByName(currentThesis.getBillingStatus())));
            }
        });
    }

    private BillingStatusResponse getBillingStatusByName(String name) {
        ArrayAdapter<BillingStatusResponse> adapter = (ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter();
        for (int i = 0; i < adapter.getCount(); i++) {
            BillingStatusResponse status = adapter.getItem(i);
            if (status != null && status.getName().equals(name)) {
                return status;
            }
        }
        return null;
    }

    private void loadThesisDetails(String id) {
        thesisApiService.getThesis(id).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    currentThesis = response.body();
                    displayThesisDetails(currentThesis);
                    loadAdditionalInfo(currentThesis);
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to load thesis details", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void displayThesisDetails(ThesisApiModel thesis) {
        textViewTitle.setText(thesis.getTitle());
        textViewDescription.setText(thesis.getDescription());
        textViewStatus.setText(thesis.getStatus());

        ArrayAdapter<BillingStatusResponse> adapter = (ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter();
        if (adapter != null) {
            for (int i = 0; i < adapter.getCount(); i++) {
                BillingStatusResponse status = adapter.getItem(i);
                if (status != null && status.getName().equals(thesis.getBillingStatus())) {
                    spinnerBillingStatus.setSelection(i);
                    break;
                }
            }
        }

        if (thesis.getDocumentFileName() != null && !thesis.getDocumentFileName().isEmpty()) {
            btnDownloadDocument.setVisibility(View.VISIBLE);
            btnDownloadDocument.setText("Exposé herunterladen (" + thesis.getDocumentFileName() + ")");
            btnDownloadDocument.setOnClickListener(v -> {
                this.thesisToDownload = thesis;
                requestDownloadPermission();
            });
        } else {
            btnDownloadDocument.setVisibility(View.GONE);
        }
    }

    private void requestDownloadPermission() {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.Q && ContextCompat.checkSelfPermission(
                this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.WRITE_EXTERNAL_STORAGE);
        } else {
            if (thesisToDownload != null) {
                downloadDocument(thesisToDownload);
            }
        }
    }

    private void downloadDocument(ThesisApiModel thesis) {
        if (thesis.getDocumentFileName() == null) return;

        Toast.makeText(this, "Download gestartet...", Toast.LENGTH_SHORT).show();

        thesisApiService.downloadThesisDocument(thesis.getId().toString()).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    boolean written = fileDownloader.writeResponseBodyToDisk(ThesisDetailActivity.this, response.body(), thesis.getDocumentFileName());
                    if (written) {
                        Toast.makeText(ThesisDetailActivity.this, "Download erfolgreich: " + thesis.getDocumentFileName(), Toast.LENGTH_LONG).show();
                    } else {
                        Toast.makeText(ThesisDetailActivity.this, "Fehler beim Speichern der Datei", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Download fehlgeschlagen", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void loadAdditionalInfo(ThesisApiModel thesis) {
        if (thesis.getSubjectAreaId() != null) {
            loadSubjectArea(thesis.getSubjectAreaId());
        } else {
            textViewSubjectArea.setText("N/A");
        }

        if (thesis.getOwnerId() != null) {
            loadUser(thesis.getOwnerId(), ownerName);
        } else {
            ownerName.setText("Unknown");
        }

        if (thesis.getTutorId() != null) {
            loadUser(thesis.getTutorId(), tutorName);
        } else {
            tutorName.setText("None");
        }

        if (thesis.getSecondSupervisorId() != null) {
            loadUser(thesis.getSecondSupervisorId(), secondSupervisorName);
        } else {
            secondSupervisorName.setText("None");
        }
    }

    private void loadUser(UUID userId, TextView targetView) {
        userApiService.getUser(userId).enqueue(new Callback<UserResponse>() {
            @Override
            public void onResponse(Call<UserResponse> call, Response<UserResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    UserResponse user = response.body();
                    String name = (user.getFirstName() != null ? user.getFirstName() : "") + " " +
                            (user.getLastName() != null ? user.getLastName() : "");
                    targetView.setText(name.trim());
                } else {
                    targetView.setText("Error loading user");
                }
            }

            @Override
            public void onFailure(Call<UserResponse> call, Throwable t) {
                targetView.setText("Error");
            }
        });
    }

    private void loadSubjectArea(UUID subjectAreaId) {
        subjectAreaApiService.getSubjectArea(subjectAreaId).enqueue(new Callback<SubjectAreaResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponse> call, Response<SubjectAreaResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    textViewSubjectArea.setText(response.body().getTitle());
                } else {
                    textViewSubjectArea.setText("N/A");
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponse> call, Throwable t) {
                textViewSubjectArea.setText("N/A");
            }
        });
    }
}
