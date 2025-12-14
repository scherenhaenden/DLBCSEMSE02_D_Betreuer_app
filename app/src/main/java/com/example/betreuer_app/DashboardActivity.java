package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.card.MaterialCardView;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DashboardActivity extends AppCompatActivity {

    private ThesisRepository thesisRepository;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dashboard);

        thesisRepository = new ThesisRepository(this);

        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        TextView studentDashboardTitle = findViewById(R.id.studentDashboardTitle);
        TextView lecturerDashboardTitle = findViewById(R.id.lecturerDashboardTitle);
        View studentView = findViewById(R.id.studentDashboardView);
        View lecturerView = findViewById(R.id.lecturerDashboardView);
        TextView studentThesisCountTextView = findViewById(R.id.studentThesisCountTextView);
        TextView lecturerThesisCountTextView = findViewById(R.id.lecturerThesisCountTextView);
        MaterialCardView studentThesisCard = findViewById(R.id.student_thesis_card);
        MaterialCardView lecturerThesisCard = findViewById(R.id.lecturer_thesis_card);

        String userName = getIntent().getStringExtra("USER_NAME");
        String userRole = getIntent().getStringExtra("USER_ROLE");

        if (userName != null) {
            welcomeTextView.setText("Hallo " + userName + "!");
        }

        View.OnClickListener openThesisList = v -> {
            Intent intent = new Intent(DashboardActivity.this, ThesisListActivity.class);
            startActivity(intent);
        };

        studentThesisCard.setOnClickListener(openThesisList);
        lecturerThesisCard.setOnClickListener(openThesisList);

        if (userRole != null) {
            if (userRole.equalsIgnoreCase("student")) {
                studentDashboardTitle.setText("Dein Dashboard als (Student)");
                studentView.setVisibility(View.VISIBLE);
                lecturerView.setVisibility(View.GONE);
            } else if (userRole.equalsIgnoreCase("tutor")) {
                lecturerDashboardTitle.setText("Dein Dashboard als (Dozent)");
                lecturerView.setVisibility(View.VISIBLE);
                studentView.setVisibility(View.GONE);
            }
        }

        // Fetch the thesis count
        thesisRepository.getTheses(1, 1, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    int thesisCount = response.body().getTotalCount();
                    String thesisText = (thesisCount == 1) ? "Abschlussarbeit" : "Abschlussarbeiten";

                    if (userRole != null) {
                        if (userRole.equalsIgnoreCase("student")) {
                            studentThesisCountTextView.setText("Du hast " + thesisCount + " " + thesisText + " im System.");
                        } else if (userRole.equalsIgnoreCase("tutor")) {
                            lecturerThesisCountTextView.setText("Du betreust " + thesisCount + " " + thesisText + ".");
                        }
                    }
                } else {
                    Toast.makeText(DashboardActivity.this, "Failed to load thesis count", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                Toast.makeText(DashboardActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
