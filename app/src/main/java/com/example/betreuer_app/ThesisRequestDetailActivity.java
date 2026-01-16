package com.example.betreuer_app;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.constants.RequestStatuses;
import com.example.betreuer_app.model.RespondToThesisRequestRequest;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.google.android.material.appbar.MaterialToolbar;

import java.util.UUID;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisRequestDetailActivity extends AppCompatActivity {

    private UUID requestId;
    private ThesisRequestApiService apiService;
    private ThesisApiService thesisApiService;
    private FileDownloader fileDownloader;

    private TextView thesisTitle;
    private TextView requesterName;
    private TextView receiverName;
    private TextView message;
    private TextView status;
    private TextView date;
    private TextView requestType;
    private TextView startDate;
    private TextView endDate;
    private Button btnDownloadDocument;
    private LinearLayout actionsLayout;
    private Button btnAccept;
    private Button btnReject;

    private ThesisRequestResponse currentRequest;


    @Override
    /**
     * Initializes the activity and sets up the user interface components.
     */
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_request_detail);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        thesisTitle = findViewById(R.id.textViewThesisTitle);
        requesterName = findViewById(R.id.textViewRequester);
        receiverName = findViewById(R.id.textViewReceiver);
        message = findViewById(R.id.textViewMessage);
        status = findViewById(R.id.textViewStatus);
        date = findViewById(R.id.textViewDate);
        requestType = findViewById(R.id.textViewRequestType);
        startDate = findViewById(R.id.textViewStartDate);
        endDate = findViewById(R.id.textViewEndDate);
        btnDownloadDocument = findViewById(R.id.buttonDownloadDocument);
        actionsLayout = findViewById(R.id.layoutActions);
        btnAccept = findViewById(R.id.buttonAccept);
        btnReject = findViewById(R.id.buttonReject);

        String idString = getIntent().getStringExtra("request_id");
        if (idString == null) {
            Toast.makeText(this, "Request ID not found", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        try {
            requestId = UUID.fromString(idString);
        } catch (IllegalArgumentException e) {
            Toast.makeText(this, "Invalid Request ID", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        apiService = ApiClient.getThesisRequestApiService(this);
        thesisApiService = ApiClient.getThesisApiService(this);
        fileDownloader = new FileDownloader();

        btnAccept.setOnClickListener(v -> respondToRequest(true));
        btnReject.setOnClickListener(v -> respondToRequest(false));
        btnDownloadDocument.setOnClickListener(v -> downloadDocument());

        loadRequestDetails();
    }

    private void loadRequestDetails() {
        apiService.getThesisRequest(requestId).enqueue(new Callback<ThesisRequestResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponse> call, Response<ThesisRequestResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    displayDetails(response.body());
                } else {
                    Toast.makeText(ThesisRequestDetailActivity.this, "Failed to load details", Toast.LENGTH_SHORT).show();
                    finish();
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponse> call, Throwable t) {
                Toast.makeText(ThesisRequestDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }

    /**
     * Displays the details of a ThesisRequestResponse object in the UI.
     *
     * This method updates various UI components with information from the provided
     * ThesisRequestResponse, including the thesis title, request type, requester and
     * receiver names, message, status, and dates. It also manages the visibility of
     * action buttons based on the current user's role and the request's status.
     *
     * @param request the ThesisRequestResponse object containing the details to display
     */
    private void displayDetails(ThesisRequestResponse request) {
        currentRequest = request;
        runOnUiThread(() -> {
            thesisTitle.setText(request.getThesisTitle() != null ? request.getThesisTitle() : "No Title");

            requestType.setText(request.getRequestType() != null ? request.getRequestType() : "Unknown Type");

            String requester = "Unknown";
            if (request.getRequester() != null) {
                requester = request.getRequester().getFirstName() + " " + request.getRequester().getLastName();
            }
            requesterName.setText(requester);

            String receiver = "Unknown";
            if (request.getReceiver() != null) {
                receiver = request.getReceiver().getFirstName() + " " + request.getReceiver().getLastName();
            }
            receiverName.setText(receiver);

            message.setText(request.getMessage() != null ? request.getMessage() : "No message provided.");

            String statusText = request.getStatus() != null ? request.getStatus() : "PENDING";
            status.setText(statusText);

            date.setText(request.getCreatedAt() != null ? request.getCreatedAt() : "Unknown date");
            startDate.setText(request.getPlannedStartOfSupervision() != null ? request.getPlannedStartOfSupervision() : "Not specified");
            endDate.setText(request.getPlannedEndOfSupervision() != null ? request.getPlannedEndOfSupervision() : "Not specified");

            // Show/hide download button based on document availability
            if (request.getDocumentFileName() != null && request.getDocumentId() != null) {
                btnDownloadDocument.setVisibility(View.VISIBLE);
                btnDownloadDocument.setText("Dokument herunterladen: " + request.getDocumentFileName());
            } else {
                btnDownloadDocument.setVisibility(View.GONE);
            }

            // Get current user ID
            SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
            String currentUserId = authPreferences.getString("user_id", null);

            // Check if user is the requester or receiver
            boolean isRequester = currentUserId != null && request.getRequester() != null &&
                    currentUserId.equals(request.getRequester().getId().toString());
            boolean isReceiver = currentUserId != null && request.getReceiver() != null &&
                    currentUserId.equals(request.getReceiver().getId().toString());

            if (RequestStatuses.ACCEPTED.equalsIgnoreCase(statusText) || RequestStatuses.REJECTED.equalsIgnoreCase(statusText)) {
                actionsLayout.setVisibility(View.GONE);
            } else {
                actionsLayout.setVisibility(View.VISIBLE);

                if (isReceiver) {
                    // User is receiver (tutor) - show Accept/Reject
                    btnAccept.setVisibility(View.VISIBLE);
                    btnReject.setVisibility(View.VISIBLE);
                    btnAccept.setText("Annehmen");
                    btnReject.setText("Ablehnen");
                } else if (isRequester) {
                    // User is requester (student) - show only Delete
                    btnAccept.setVisibility(View.GONE);
                    btnReject.setVisibility(View.VISIBLE);
                    btnReject.setText("Löschen");
                } else {
                    // Fallback - hide actions
                    actionsLayout.setVisibility(View.GONE);
                }
            }
        });
    }

    private void respondToRequest(boolean accept) {
        // Get current user ID to determine if this is a delete action
        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
        String currentUserId = authPreferences.getString("user_id", null);

        // Check if we need to load the request details to determine user role
        // For now, we'll assume the button text determines the action
        boolean isDeleteAction = "Löschen".equals(btnReject.getText().toString());

        if (isDeleteAction) {
            // Use DELETE API for deletion
            deleteRequest();
        } else {
            // Use respond API for accept/reject
            RespondToThesisRequestRequest body = new RespondToThesisRequestRequest(accept, accept ? "Accepted" : "Rejected");

            apiService.respondToRequest(requestId, body).enqueue(new Callback<Void>() {
                @Override
                public void onResponse(Call<Void> call, Response<Void> response) {
                    if (response.isSuccessful()) {
                        String msg = accept ? "Request accepted" : "Request rejected";
                        Toast.makeText(ThesisRequestDetailActivity.this, msg, Toast.LENGTH_SHORT).show();
                        loadRequestDetails(); // Refresh view
                    } else {
                        Toast.makeText(ThesisRequestDetailActivity.this, "Action failed: " + response.code(), Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(Call<Void> call, Throwable t) {
                    Toast.makeText(ThesisRequestDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        }
    }

    /**
     * Initiates the download of a document associated with the current request.
     *
     * This method checks if the currentRequest and its required fields are not null.
     * If valid, it starts the download process by calling the thesisApiService to fetch
     * the document. The response is handled to either confirm a successful download
     * or report any errors encountered during the process.
     */
    private void downloadDocument() {
        if (currentRequest == null || currentRequest.getDocumentFileName() == null || currentRequest.getThesisId() == null) {
            Toast.makeText(this, "Kein Dokument verfügbar", Toast.LENGTH_SHORT).show();
            return;
        }

        Toast.makeText(this, "Download gestartet...", Toast.LENGTH_SHORT).show();

        thesisApiService.downloadThesisDocument(currentRequest.getThesisId().toString()).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    boolean written = fileDownloader.writeResponseBodyToDisk(ThesisRequestDetailActivity.this, response.body(), currentRequest.getDocumentFileName());
                    if (written) {
                        Toast.makeText(ThesisRequestDetailActivity.this, "Download erfolgreich: " + currentRequest.getDocumentFileName(), Toast.LENGTH_LONG).show();
                    } else {
                        Toast.makeText(ThesisRequestDetailActivity.this, "Fehler beim Speichern der Datei", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(ThesisRequestDetailActivity.this, "Download fehlgeschlagen", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(ThesisRequestDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void deleteRequest() {
        apiService.deleteRequest(requestId).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(ThesisRequestDetailActivity.this, "Request deleted", Toast.LENGTH_SHORT).show();
                    finish(); // Close the detail activity since the request is deleted
                } else {
                    Toast.makeText(ThesisRequestDetailActivity.this, "Delete failed: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(ThesisRequestDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
