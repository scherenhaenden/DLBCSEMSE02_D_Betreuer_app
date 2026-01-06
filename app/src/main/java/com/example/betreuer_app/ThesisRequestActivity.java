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
import com.google.android.material.appbar.MaterialToolbar;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisRequestActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private TextView emptyView;
    private ThesisRequestAdapter adapter;
    private ThesisRequestApiService apiService;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_request);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        recyclerView = findViewById(R.id.recyclerViewRequests);
        emptyView = findViewById(R.id.textViewEmpty);

        apiService = ApiClient.getThesisRequestApiService(this);

        setupRecyclerView();
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadRequests();
    }

    private void setupRecyclerView() {
        adapter = new ThesisRequestAdapter();
        adapter.setOnRequestActionClickListener(new ThesisRequestAdapter.OnRequestActionClickListener() {
            @Override
            public void onAccept(ThesisRequestResponse request) {
                respondToRequest(request, true);
            }

            @Override
            public void onReject(ThesisRequestResponse request) {
                respondToRequest(request, false);
            }
        });

        adapter.setOnItemClickListener(new ThesisRequestAdapter.OnItemClickListener() {
            @Override
            public void onItemClick(ThesisRequestResponse request) {
                Intent intent = new Intent(ThesisRequestActivity.this, ThesisRequestDetailActivity.class);
                intent.putExtra("request_id", request.getId().toString());
                startActivity(intent);
            }
        });

        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        recyclerView.setAdapter(adapter);
    }

    private void loadRequests() {
        // Fetching page 0, size 50. Adjust as needed.
        apiService.getMyRequests(0, 50).enqueue(new Callback<ThesisRequestResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponsePaginatedResponse> call, Response<ThesisRequestResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<ThesisRequestResponse> requests = response.body().getItems();
                    if (requests == null) requests = new ArrayList<>();
                    
                    adapter.setRequests(requests);
                    
                    if (requests.isEmpty()) {
                        emptyView.setVisibility(View.VISIBLE);
                        recyclerView.setVisibility(View.GONE);
                    } else {
                        emptyView.setVisibility(View.GONE);
                        recyclerView.setVisibility(View.VISIBLE);
                    }
                } else {
                    Toast.makeText(ThesisRequestActivity.this, "Failed to load requests", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                Toast.makeText(ThesisRequestActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void respondToRequest(ThesisRequestResponse request, boolean accept) {
        RespondToThesisRequestRequest body = new RespondToThesisRequestRequest(accept, accept ? "Accepted" : "Rejected");
        
        apiService.respondToRequest(request.getId(), body).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                if (response.isSuccessful()) {
                    String message = accept ? "Request accepted" : "Request rejected";
                    Toast.makeText(ThesisRequestActivity.this, message, Toast.LENGTH_SHORT).show();
                    loadRequests(); // Reload list to update status
                } else {
                    Toast.makeText(ThesisRequestActivity.this, "Action failed: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(ThesisRequestActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
