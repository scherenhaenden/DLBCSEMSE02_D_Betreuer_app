package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.model.RespondToThesisRequestRequest;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse;
import com.example.betreuer_app.ui.requests.ThesisRequestAdapter;
import com.example.betreuer_app.util.SessionManager;
import com.google.android.material.appbar.MaterialToolbar;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * Activity to display ONLY outgoing/sent requests (where the current user is the requester).
 * Used by tutors to see requests they have sent (e.g., for co-supervision with a second supervisor).
 */
public class SentRequestsActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private TextView emptyView;
    private ThesisRequestAdapter adapter;
    private ThesisRequestApiService apiService;
    private SessionManager sessionManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_request);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setTitle("Gesendete Anfragen");
        toolbar.setNavigationOnClickListener(v -> finish());

        recyclerView = findViewById(R.id.recyclerViewRequests);
        emptyView = findViewById(R.id.textViewEmpty);

        apiService = ApiClient.getThesisRequestApiService(this);
        sessionManager = new SessionManager(this);

        setupRecyclerView();
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadSentRequests();
    }

    private void setupRecyclerView() {
        adapter = new ThesisRequestAdapter();

        String currentUserId = sessionManager.getUserId();
        adapter.setCurrentUserId(currentUserId);

        adapter.setOnRequestActionClickListener(new ThesisRequestAdapter.OnRequestActionClickListener() {
            @Override
            public void onAccept(ThesisRequestResponse request) {
                // Not applicable for sent requests
            }

            @Override
            public void onReject(ThesisRequestResponse request) {
                // Not applicable for sent requests
            }

            @Override
            public void onCancel(ThesisRequestResponse request) {
                cancelRequest(request);
            }

            @Override
            public void onDelete(ThesisRequestResponse request) {
                deleteRequest(request);
            }
        });

        adapter.setOnItemClickListener(new ThesisRequestAdapter.OnItemClickListener() {
            @Override
            public void onItemClick(ThesisRequestResponse request) {
                Intent intent = new Intent(SentRequestsActivity.this, ThesisRequestDetailActivity.class);
                intent.putExtra("request_id", request.getId().toString());
                startActivity(intent);
            }
        });

        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        recyclerView.setAdapter(adapter);
    }

    private void loadSentRequests() {
        // Load ONLY outgoing/sent requests using the appropriate API endpoint
        if (sessionManager.isTutor()) {
            // Tutors use the tutor-specific endpoint
            apiService.getOutgoingRequests(null, 1, 50).enqueue(new Callback<ThesisRequestResponsePaginatedResponse>() {
                @Override
                public void onResponse(Call<ThesisRequestResponsePaginatedResponse> call, Response<ThesisRequestResponsePaginatedResponse> response) {
                    handleResponse(response);
                }

                @Override
                public void onFailure(Call<ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                    handleFailure(t);
                }
            });
        } else {
            // Students use the general endpoint (which returns their sent requests)
            apiService.getMyRequests(1, 50).enqueue(new Callback<ThesisRequestResponsePaginatedResponse>() {
                @Override
                public void onResponse(Call<ThesisRequestResponsePaginatedResponse> call, Response<ThesisRequestResponsePaginatedResponse> response) {
                    handleResponse(response);
                }

                @Override
                public void onFailure(Call<ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                    handleFailure(t);
                }
            });
        }
    }

    private void handleResponse(Response<ThesisRequestResponsePaginatedResponse> response) {
        if (response.isSuccessful() && response.body() != null) {
            List<ThesisRequestResponse> requests = response.body().getItems();
            if (requests == null) requests = new ArrayList<>();
            updateRequestList(requests);
        } else {
            Toast.makeText(SentRequestsActivity.this, "Fehler beim Laden der gesendeten Anfragen", Toast.LENGTH_SHORT).show();
        }
    }

    private void handleFailure(Throwable t) {
        Toast.makeText(SentRequestsActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
    }

    private void updateRequestList(List<ThesisRequestResponse> requests) {
        adapter.setRequests(requests);

        if (requests.isEmpty()) {
            emptyView.setVisibility(View.VISIBLE);
            emptyView.setText("Du hast noch keine Anfragen gesendet.");
            recyclerView.setVisibility(View.GONE);
        } else {
            emptyView.setVisibility(View.GONE);
            recyclerView.setVisibility(View.VISIBLE);
        }
    }

    private void cancelRequest(ThesisRequestResponse request) {
        RespondToThesisRequestRequest body = new RespondToThesisRequestRequest(false, "Anfrage vom Sender abgebrochen");

        apiService.respondToRequest(request.getId(), body).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(SentRequestsActivity.this, "Anfrage abgebrochen", Toast.LENGTH_SHORT).show();
                    loadSentRequests();
                } else {
                    Toast.makeText(SentRequestsActivity.this, "Abbrechen fehlgeschlagen: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(SentRequestsActivity.this, "Fehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void deleteRequest(ThesisRequestResponse request) {
        apiService.deleteRequest(request.getId()).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(SentRequestsActivity.this, "Anfrage gelöscht", Toast.LENGTH_SHORT).show();
                    loadSentRequests();
                } else {
                    Toast.makeText(SentRequestsActivity.this, "Löschen fehlgeschlagen: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(SentRequestsActivity.this, "Fehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}

