package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.appbar.MaterialToolbar;
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

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        TextView studentDashboardTitle = findViewById(R.id.studentDashboardTitle);
        TextView lecturerDashboardTitle = findViewById(R.id.lecturerDashboardTitle);
        View studentView = findViewById(R.id.studentDashboardView);
        View lecturerView = findViewById(R.id.lecturerDashboardView);
        TextView studentThesisCountTextView = findViewById(R.id.studentThesisCountTextView);
        TextView lecturerThesisCountTextView = findViewById(R.id.lecturerThesisCountTextView);
        MaterialCardView studentThesisCard = findViewById(R.id.student_thesis_card);
        MaterialCardView lecturerThesisCard = findViewById(R.id.lecturer_thesis_card);
        Button btnCreateNewThesis = findViewById(R.id.btn_create_new_thesis);
        Button btnFindTutor = findViewById(R.id.btn_find_tutor);

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

        btnCreateNewThesis.setOnClickListener(v -> {
            Intent intent = new Intent(DashboardActivity.this, CreateThesisActivity.class);
            startActivity(intent);
        });

        btnFindTutor.setOnClickListener(v -> {
            Intent intent = new Intent(DashboardActivity.this, TutorListActivity.class);
            startActivity(intent);
        });

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
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.dashboard_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item) {
        if (item.getItemId() == R.id.action_logout) {
            logout();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private void logout() {
        // Clear the token
        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
        SharedPreferences.Editor editor = authPreferences.edit();
        editor.clear();
        editor.apply();

        // Navigate to LoginActivity
        Intent intent = new Intent(DashboardActivity.this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadDashboardData();
    }

    private void loadDashboardData() {
        String userRole = getIntent().getStringExtra("USER_ROLE");
        TextView studentThesisCountTextView = findViewById(R.id.studentThesisCountTextView);
        TextView lecturerThesisCountTextView = findViewById(R.id.lecturerThesisCountTextView);
        
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
                } else if (response.code() == 401) { // Unauthorized
                    Toast.makeText(DashboardActivity.this, "Sitzung abgelaufen. Bitte erneut einloggen.", Toast.LENGTH_LONG).show();
                    logout();
                } else {
                    Toast.makeText(DashboardActivity.this, "Failed to load dashboard data. Code: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                Toast.makeText(DashboardActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
