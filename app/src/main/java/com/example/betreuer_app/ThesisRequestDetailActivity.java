package com.example.betreuer_app;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.model.RespondToThesisRequestRequest;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.google.android.material.appbar.MaterialToolbar;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisRequestDetailActivity extends AppCompatActivity {

    private UUID requestId;
    private ThesisRequestApiService apiService;

    private TextView thesisTitle;
    private TextView requesterName;
    private TextView receiverName;
    private TextView message;
    private TextView status;
    private TextView date;
    private LinearLayout actionsLayout;
    private Button btnAccept;
    private Button btnReject;

    @Override
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

        btnAccept.setOnClickListener(v -> respondToRequest(true));
        btnReject.setOnClickListener(v -> respondToRequest(false));

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

    private void displayDetails(ThesisRequestResponse request) {
        thesisTitle.setText(request.getThesisTitle() != null ? request.getThesisTitle() : "No Title");
        
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

        if ("ACCEPTED".equalsIgnoreCase(statusText) || "REJECTED".equalsIgnoreCase(statusText)) {
            actionsLayout.setVisibility(View.GONE);
        } else {
            actionsLayout.setVisibility(View.VISIBLE);
        }
    }

    private void respondToRequest(boolean accept) {
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
