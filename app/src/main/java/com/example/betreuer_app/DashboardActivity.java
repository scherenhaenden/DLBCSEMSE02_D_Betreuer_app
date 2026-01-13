package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.ViewStub;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.ui.DashboardUiHelper;
import com.example.betreuer_app.viewmodel.DashboardViewModel;
import com.example.betreuer_app.viewmodel.ViewModelFactory;
import com.google.android.material.appbar.MaterialToolbar;

public class DashboardActivity extends AppCompatActivity {

    private DashboardViewModel viewModel;
    private DashboardUiHelper uiHelper;
    private String userRole;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dashboard);

        // Initialize ViewModel
        ThesisRepository thesisRepository = createThesisRepository();
        ThesisRequestApiService thesisRequestApiService = ApiClient.getThesisRequestApiService(this);
        ViewModelFactory factory = new ViewModelFactory(thesisRepository, thesisRequestApiService);
        viewModel = new ViewModelProvider(this, factory).get(DashboardViewModel.class);

        // Initialize UI Helper
        uiHelper = new DashboardUiHelper(this);

        // Set up toolbar
        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        // Get user info from intent
        String userName = getIntent().getStringExtra("USER_NAME");
        userRole = getIntent().getStringExtra("USER_ROLE");

        // Set welcome message
        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        if (userName != null) {
            welcomeTextView.setText("Hallo " + userName + "!");
        }

        // Set up role-specific UI
        setupRoleSpecificUi();

        // Observe ViewModel
        observeViewModel();
    }

    /**
     * Set up UI based on user role
     */
    private void setupRoleSpecificUi() {
        ViewStub stubStudent = findViewById(R.id.stub_student_dashboard);
        ViewStub stubLecturer = findViewById(R.id.stub_lecturer_dashboard);

        if (userRole != null) {
            if (userRole.equalsIgnoreCase("student")) {
                uiHelper.setupStudentDashboard(stubStudent, stubLecturer);
            } else if (userRole.equalsIgnoreCase("tutor")) {
                uiHelper.setupLecturerDashboard(stubStudent, stubLecturer);
            }
        }
    }

    /**
     * Observe ViewModel LiveData
     */
    private void observeViewModel() {
        // Observe thesis count
        viewModel.getThesisCount().observe(this, resource -> {
            if (resource != null) {
                switch (resource.getStatus()) {
                    case SUCCESS:
                        if (resource.getData() != null) {
                            updateThesisCountUi(resource.getData());
                        }
                        break;
                    case ERROR:
                        if (resource.getMessage() != null) {
                            Toast.makeText(this, resource.getMessage(), Toast.LENGTH_SHORT).show();
                        }
                        break;
                    case LOADING:
                        // Optional: Show loading indicator
                        break;
                }
            }
        });

        // Observe pending requests count (for tutors)
        viewModel.getPendingRequestsCount().observe(this, resource -> {
            if (resource != null && resource.getStatus() == com.example.betreuer_app.util.Resource.Status.SUCCESS) {
                if (resource.getData() != null) {
                    uiHelper.updateLecturerRequestsCount(resource.getData());
                }
            }
        });

        // Observe session expiration
        viewModel.getSessionExpired().observe(this, expired -> {
            if (expired != null && expired) {
                Toast.makeText(this, "Sitzung abgelaufen. Bitte erneut einloggen.", Toast.LENGTH_LONG).show();
                logout();
            }
        });
    }

    /**
     * Update thesis count UI based on user role
     */
    private void updateThesisCountUi(int count) {
        if (userRole != null) {
            if (userRole.equalsIgnoreCase("student")) {
                uiHelper.updateStudentThesisCount(count);
            } else if (userRole.equalsIgnoreCase("tutor")) {
                uiHelper.updateLecturerThesisCount(count);
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
        viewModel.loadDashboardData(userRole);
    }
}
