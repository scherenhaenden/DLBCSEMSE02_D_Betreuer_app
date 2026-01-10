package com.example.betreuer_app;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisOfferApplicationApiService;
import com.example.betreuer_app.constants.AuthConstants;
import com.example.betreuer_app.model.CreateThesisOfferApplicationRequest;
import com.example.betreuer_app.model.ThesisOfferApplicationResponse;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisOfferDetailActivity extends AppCompatActivity {

    private ThesisOfferApplicationApiService apiService;
    private String thesisOfferId;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_offer_detail);

        apiService = ApiClient.getThesisOfferApplicationApiService(this);

        String title = getIntent().getStringExtra("THESIS_OFFER_TITLE");
        String description = getIntent().getStringExtra("THESIS_OFFER_DESCRIPTION");
        thesisOfferId = getIntent().getStringExtra("THESIS_OFFER_ID");

        TextView titleView = findViewById(R.id.tv_offer_detail_title);
        TextView descriptionView = findViewById(R.id.tv_offer_detail_description);
        Button applyButton = findViewById(R.id.btn_apply_for_offer);

        titleView.setText(title);
        descriptionView.setText(description);

        com.google.android.material.appbar.MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        applyButton.setOnClickListener(v -> {
            if (thesisOfferId != null) {
                applyForThesisOffer();
            }
        });
    }

    private void applyForThesisOffer() {
        SharedPreferences prefs = getSharedPreferences(AuthConstants.PREFS_NAME, Context.MODE_PRIVATE);
        String studentId = prefs.getString(AuthConstants.KEY_USER_ID, null);

        if (studentId == null) {
            Toast.makeText(this, "Fehler: Sie müssen als Student angemeldet sein.", Toast.LENGTH_SHORT).show();
            return;
        }

        // The message is currently hardcoded, but you could add an EditText for it.
        String message = "Ich bewerbe mich für dieses Thema.";

        CreateThesisOfferApplicationRequest request = new CreateThesisOfferApplicationRequest(
                UUID.fromString(thesisOfferId),
                message,
                UUID.fromString(studentId)
        );

        apiService.createApplication(request).enqueue(new Callback<ThesisOfferApplicationResponse>() {
            @Override
            public void onResponse(Call<ThesisOfferApplicationResponse> call, Response<ThesisOfferApplicationResponse> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(ThesisOfferDetailActivity.this, "Bewerbung erfolgreich gesendet!", Toast.LENGTH_LONG).show();
                    finish();
                } else {
                    Toast.makeText(ThesisOfferDetailActivity.this, "Fehler bei der Bewerbung: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisOfferApplicationResponse> call, Throwable t) {
                Toast.makeText(ThesisOfferDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
