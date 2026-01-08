package com.example.betreuer_app;

import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.UserResponse;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisDetailActivity extends AppCompatActivity {

    private TextView textViewTitle;
    private TextView textViewStatus;
    private TextView textViewBillingStatus;
    private TextView textViewSubjectArea;
    private TextView textViewOwner;
    private TextView textViewTutor;
    private TextView textViewSecondSupervisor;
    private MaterialButton btnDownloadDocument;

    private ThesisApiService thesisApiService;
    private UserApiService userApiService;
    private SubjectAreaApiService subjectAreaApiService;

    private String thesisId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_detail);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        textViewTitle = findViewById(R.id.textViewTitle);
        textViewStatus = findViewById(R.id.textViewStatus);
        textViewBillingStatus = findViewById(R.id.textViewBillingStatus);
        textViewSubjectArea = findViewById(R.id.textViewSubjectArea);
        textViewOwner = findViewById(R.id.textViewOwner);
        textViewTutor = findViewById(R.id.textViewTutor);
        textViewSecondSupervisor = findViewById(R.id.textViewSecondSupervisor);
        btnDownloadDocument = findViewById(R.id.btn_download_document);

        thesisApiService = ApiClient.getThesisApiService(this);
        userApiService = ApiClient.getUserApiService(this);
        subjectAreaApiService = ApiClient.getSubjectAreaApiService(this);

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
            btnDownloadDocument.setOnClickListener(v -> downloadDocument(thesis));
        } else {
            btnDownloadDocument.setVisibility(View.GONE);
        }
    }
    
    private void downloadDocument(ThesisApiModel thesis) {
        if (thesis.getDocumentFileName() == null) return;
        
        Toast.makeText(this, "Download gestartet...", Toast.LENGTH_SHORT).show();
        
        thesisApiService.downloadThesisDocument(thesis.getId().toString()).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    boolean written = writeResponseBodyToDisk(response.body(), thesis.getDocumentFileName());
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
    
    private boolean writeResponseBodyToDisk(ResponseBody body, String fileName) {
        // Fallback for older versions
        File path = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS);
        if (!path.exists()) {
            path.mkdirs();
        }
        File file = new File(path, fileName);

        try (InputStream inputStream = body.byteStream();
             OutputStream outputStream = new FileOutputStream(file)) {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.read(buffer)) != -1) {
                outputStream.write(buffer, 0, bytesRead);
            }
            return true;
        } catch (IOException e) {
            return false;
        }
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
