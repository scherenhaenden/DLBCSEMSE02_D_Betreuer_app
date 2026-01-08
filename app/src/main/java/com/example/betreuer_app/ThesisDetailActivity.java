package com.example.betreuer_app;

import android.Manifest;
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
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.UserResponse;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisDetailActivity extends AppCompatActivity {

    private TextView textViewTitle;
    private TextView textViewStatus;

    private TextView textViewSubjectArea;
    private TextView textViewOwner;
    private TextView textViewTutor;
    private TextView textViewSecondSupervisor;
    private MaterialButton btnDownloadDocument;

    private ThesisApiService thesisApiService;
    private UserApiService userApiService;

    private FileDownloader fileDownloader;

    private String thesisId;
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

        textViewTitle = findViewById(R.id.textViewTitle);
        textViewStatus = findViewById(R.id.textViewStatus);

        textViewSubjectArea = findViewById(R.id.textViewSubjectArea);
        textViewOwner = findViewById(R.id.textViewOwner);
        textViewTutor = findViewById(R.id.textViewTutor);
        textViewSecondSupervisor = findViewById(R.id.textViewSecondSupervisor);
        btnDownloadDocument = findViewById(R.id.btn_download_document);

        thesisApiService = ApiClient.getThesisApiService(this);
        userApiService = ApiClient.getUserApiService(this);

        //Rechungsstatus mit Dropdown-Spinner

        Spinner spinner_billingstatus = findViewById(R.id.spinner_billingstatus);
        spinner_billingstatus.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {

            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                String selectedItem = parent.getItemAtPosition(position).toString();


                Toast.makeText(ThesisDetailActivity.this,
                        "Ausgewählt: " + selectedItem,
                        Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
                // optional
            }
        });
        List<String> items = new ArrayList<>();
        items.add("NONE");
        items.add("ISSUED");
        items.add("PAID");

        ArrayAdapter<String> billingstatus_adapter = new ArrayAdapter<>(
                this,
                android.R.layout.simple_spinner_item,
                items
        );

        billingstatus_adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner_billingstatus.setAdapter(billingstatus_adapter);

        if (getIntent().hasExtra("THESIS_ID")) {
            thesisId = getIntent().getStringExtra("THESIS_ID");
            loadThesisDetails(thesisId);
        } else {
            Toast.makeText(this, "Thesis ID not provided", Toast.LENGTH_SHORT).show();
            finish();
        }
    }

    private void loadThesisDetails(String id) {
        thesisApiService.getThesis(id).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ThesisApiModel thesis = response.body();
                    displayThesisDetails(thesis);
                    loadAdditionalInfo(thesis);
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
        textViewStatus.setText(thesis.getStatus());

        textViewBillingStatus.setText(thesis.getBillingStatus());

        if (thesis.getDocumentFileName() != null && !thesis.getDocumentFileName().isEmpty()) {
            btnDownloadDocument.setVisibility(View.VISIBLE);
            btnDownloadDocument.setText("Thesis herunterladen (" + thesis.getDocumentFileName() + ")");
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
        // Load Subject Area
        if (thesis.getTopicId() != null) {
            textViewSubjectArea.setText("N/A"); // Placeholder until we can fetch topic -> subject area
        } else {
            textViewSubjectArea.setText("N/A");
        }

        // Load Owner (Student)
        if (thesis.getOwnerId() != null) {
            loadUser(thesis.getOwnerId(), textViewOwner);
        } else {
            textViewOwner.setText("Unknown");
        }

        // Load Tutor
        if (thesis.getTutorId() != null) {
            loadUser(thesis.getTutorId(), textViewTutor);
        } else {
            textViewTutor.setText("None");
        }

        // Load Second Supervisor
        if (thesis.getSecondSupervisorId() != null) {
            loadUser(thesis.getSecondSupervisorId(), textViewSecondSupervisor);
        } else {
            textViewSecondSupervisor.setText("None");
        }
    }

    private void loadUser(String userId, TextView targetView) {
        try {
            UUID uuid = UUID.fromString(userId);
            userApiService.getUser(uuid).enqueue(new Callback<UserResponse>() {
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
        } catch (IllegalArgumentException e) {
            targetView.setText("Invalid ID");
        }
    }


}
