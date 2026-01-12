package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewStub;
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
    
    // Member variables for dynamic views loaded from ViewStubs
    private View studentView;
    private View lecturerView;
    
    // UI components that will be initialized after ViewStub inflation
    private TextView studentThesisCountTextView;
    private TextView lecturerThesisCountTextView;
    private TextView lecturerRequestsCountTextView;
    private MaterialCardView studentThesisCard;
    private MaterialCardView lecturerThesisCard;
    private MaterialCardView lecturerRequestsCard;
    private Button btnCreateNewThesis;
    private Button btnManageThesisOffers;
    private Button btnFindTutor;
    private Button btnPendingRequests;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dashboard);

        thesisRepository = createThesisRepository();

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        ViewStub stubStudent = findViewById(R.id.stub_student_dashboard);
        ViewStub stubLecturer = findViewById(R.id.stub_lecturer_dashboard);

        String userName = getIntent().getStringExtra("USER_NAME");
        String userRole = getIntent().getStringExtra("USER_ROLE");

        if (userName != null) {
            welcomeTextView.setText("Hallo " + userName + "!");
        }

        View.OnClickListener openThesisList = v -> {
            Intent intent = new Intent(DashboardActivity.this, ThesisListActivity.class);
            startActivity(intent);
        };

        if (userRole != null) {
            if (userRole.equalsIgnoreCase("student")) {
                // Inflate Student View
                if (stubStudent != null) {
                    if (studentView == null) {
                        studentView = stubStudent.inflate();
                    } else {
                        studentView.setVisibility(View.VISIBLE);
                    }
                    
                    if (stubLecturer != null) stubLecturer.setVisibility(View.GONE);
                    if (lecturerView != null) lecturerView.setVisibility(View.GONE);
                    
                    // Initialize Student UI components
                    studentThesisCountTextView = studentView.findViewById(R.id.studentThesisCountTextView);
                    studentThesisCard = studentView.findViewById(R.id.student_thesis_card);
                    btnCreateNewThesis = studentView.findViewById(R.id.btn_create_new_thesis);
                    btnFindTutor = studentView.findViewById(R.id.btn_find_tutor);
                    btnPendingRequests = studentView.findViewById(R.id.btn_pending_requests);

                    studentThesisCard.setOnClickListener(openThesisList);
                    
                    btnCreateNewThesis.setOnClickListener(v -> {
                        Intent intent = new Intent(DashboardActivity.this, StudentCreateThesisActivity.class);
                        startActivity(intent);
                    });

                    btnFindTutor.setOnClickListener(v -> {
                        Intent intent = new Intent(DashboardActivity.this, TutorListActivity.class);
                        startActivity(intent);
                    });

                    btnPendingRequests.setOnClickListener(v -> {
                        Intent intent = new Intent(DashboardActivity.this, ThesisRequestActivity.class);
                        startActivity(intent);
                    });
                }
            } else if (userRole.equalsIgnoreCase("tutor")) {
                // Inflate Lecturer View
                if (stubLecturer != null) {
                    if (lecturerView == null) {
                        lecturerView = stubLecturer.inflate();
                    } else {
                        lecturerView.setVisibility(View.VISIBLE);
                    }

                    if (stubStudent != null) stubStudent.setVisibility(View.GONE);
                    if (studentView != null) studentView.setVisibility(View.GONE);

                    // Initialize Lecturer UI components
                    lecturerThesisCountTextView = lecturerView.findViewById(R.id.lecturerThesisCountTextView);
                    lecturerRequestsCountTextView = lecturerView.findViewById(R.id.lecturerRequestsCountTextView);
                    lecturerThesisCard = lecturerView.findViewById(R.id.lecturer_thesis_card);
                    lecturerRequestsCard = lecturerView.findViewById(R.id.lecturer_requests_card);
                    btnManageThesisOffers = lecturerView.findViewById(R.id.btn_manage_thesis_offers);

                    lecturerThesisCard.setOnClickListener(openThesisList);
                    
                    btnManageThesisOffers.setOnClickListener(v -> {
                        Intent intent = new Intent(DashboardActivity.this, ThesisOfferDashboardActivity.class);
                        startActivity(intent);
                    });
                    
                    lecturerRequestsCard.setOnClickListener(v -> {
                        Intent intent = new Intent(DashboardActivity.this, ThesisRequestActivity.class);
                        startActivity(intent);
                    });
                }
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

    protected ThesisRepository createThesisRepository() {
        return new ThesisRepository(this);
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
        
        thesisRepository.getTheses(1, 1, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    int thesisCount = response.body().getTotalCount();
                    String thesisText = (thesisCount == 1) ? "Abschlussarbeit" : "Abschlussarbeiten";

                    if (userRole != null) {
                        if (userRole.equalsIgnoreCase("student") && studentThesisCountTextView != null) {
                            studentThesisCountTextView.setText("Du hast " + thesisCount + " " + thesisText + " im System.");
                        } else if (userRole.equalsIgnoreCase("tutor") && lecturerThesisCountTextView != null) {
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
