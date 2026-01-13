package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.button.MaterialButton;

/**
 * TutorProfileActivity displays the profile of a selected tutor.
 * It provides options to view the tutor's thesis offers or create a supervision request.
 */
public class TutorProfileActivity extends AppCompatActivity {

    private String tutorId;
    private String tutorName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tutor_profile);

        if (getIntent() != null) {
            tutorId = getIntent().getStringExtra("TUTOR_ID");
            tutorName = getIntent().getStringExtra("TUTOR_NAME");
        }

        if (tutorId == null) {
            Toast.makeText(this, "Tutor ID missing", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        TextView tvTutorName = findViewById(R.id.tv_tutor_name);
        tvTutorName.setText(tutorName != null ? tutorName : "Unbekannter Tutor");

        MaterialButton btnViewOffers = findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = findViewById(R.id.btn_create_request);

        btnViewOffers.setOnClickListener(v -> {
            Intent intent = new Intent(TutorProfileActivity.this, ThesisOfferDashboardActivity.class); // Or a specific filtered list activity
            intent.putExtra("TUTOR_ID", tutorId); // Pass filter
            intent.putExtra("TUTOR_NAME", tutorName);
            // Note: ThesisOfferDashboardActivity currently shows "My Offers" for the logged-in user.
            // You might need to adapt it or create a new "TutorOffersActivity" to show offers of *another* user.
            // For now, I'm linking to it, but you might want to clarify this flow.
            // Actually, based on your prompt, you want to "see the ThesisOffers from that Tutor".
            // If ThesisOfferDashboardActivity is strictly for "My Offers", we should probably use a different activity or mode.
            // Let's assume for now we use the existing one but maybe we need to adjust it to load by tutor ID passed in intent?
            // I'll leave it as is for the structure, but keep in mind.
             intent.putExtra("MODE", "VIEW_TUTOR_OFFERS");
            startActivity(intent);
        });

        btnCreateRequest.setOnClickListener(v -> {
            Intent intent = new Intent(TutorProfileActivity.this, SupervisionRequestActivity.class);
            intent.putExtra("TUTOR_ID", tutorId);
            intent.putExtra("TUTOR_NAME", tutorName);

            // Leite Intent-Extras für zweiten Supervisor weiter
            if (getIntent().getBooleanExtra("SELECTING_SECOND_SUPERVISOR", false)) {
                intent.putExtra("SELECTING_SECOND_SUPERVISOR", true);
                intent.putExtra("THESIS_ID", getIntent().getStringExtra("THESIS_ID"));
            }

            startActivity(intent);
        });
    }
}
