package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.adapter.ThesisOfferAdapter;
import com.example.betreuer_app.constants.AuthConstants;
import com.example.betreuer_app.model.ThesisOfferApiModel;
import com.example.betreuer_app.model.ThesisOfferResponse;
import com.example.betreuer_app.repository.ThesisOfferRepository;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import java.util.List;
import java.util.UUID;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisOfferDashboardActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private TextView emptyView;
    private ThesisOfferRepository repository;
    private String userId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_offer_dashboard);

        // Retrieve current user ID
        SharedPreferences prefs = getSharedPreferences(AuthConstants.PREFS_NAME, MODE_PRIVATE);
        userId = prefs.getString(AuthConstants.KEY_USER_ID, null);

        repository = new ThesisOfferRepository(this);

        recyclerView = findViewById(R.id.rv_thesis_offers);
        emptyView = findViewById(R.id.tv_empty_list);
        
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        FloatingActionButton fab = findViewById(R.id.fab_add_thesis_offer);
        fab.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisOfferDashboardActivity.this, CreateThesisOfferActivity.class);
            startActivity(intent);
        });
        
        com.google.android.material.appbar.MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (userId != null) {
            loadThesisOffers();
        } else {
            Toast.makeText(this, "Fehler: Benutzer-ID nicht gefunden.", Toast.LENGTH_SHORT).show();
        }
    }

    private void loadThesisOffers() {
        repository.getThesisOffersByUser(UUID.fromString(userId), 1, 50, new Callback<ThesisOfferResponse>() {
            @Override
            public void onResponse(Call<ThesisOfferResponse> call, Response<ThesisOfferResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<ThesisOfferApiModel> offers = response.body().getItems();
                    if (offers != null && !offers.isEmpty()) {
                        ThesisOfferAdapter adapter = new ThesisOfferAdapter(offers);
                        recyclerView.setAdapter(adapter);
                        recyclerView.setVisibility(View.VISIBLE);
                        emptyView.setVisibility(View.GONE);
                    } else {
                        recyclerView.setVisibility(View.GONE);
                        emptyView.setVisibility(View.VISIBLE);
                    }
                } else {
                    Toast.makeText(ThesisOfferDashboardActivity.this, "Fehler beim Laden: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisOfferResponse> call, Throwable t) {
                Toast.makeText(ThesisOfferDashboardActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
