package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;

import com.example.betreuer_app.constants.AuthConstants;
import com.example.betreuer_app.constants.ThemeConstants;
import com.example.betreuer_app.model.LoggedInUser;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.LoginRepository;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.util.SessionManager;
import com.google.android.material.switchmaterial.SwitchMaterial;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * LoginActivity handles user authentication, including login and auto-login functionality,
 * as well as theme selection (light/dark mode).
 */
public class LoginActivity extends AppCompatActivity {

    private EditText emailEditText;
    private EditText passwordEditText;
    private Button loginButton;
    private ProgressBar progressBar;
    private LoginRepository loginRepository;
    private SessionManager sessionManager;
    private SwitchMaterial themeSwitch;

    /**
     * Called when the activity is starting. Initializes the UI components, sets up listeners,
     * and checks for auto-login.
     * @param savedInstanceState If the activity is being re-initialized after previously being shut down,
     * this Bundle contains the data it most recently supplied in onSaveInstanceState.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        initializeViews();

        loginRepository = new LoginRepository(this);
        sessionManager = new SessionManager(this);

        setupThemeSwitch();

        setupLoginButton();

        // Auto-login check
        checkAutoLogin();
    }

    /**
     * Initializes the UI views by finding them from the layout.
     */
    private void initializeViews() {
        emailEditText = findViewById(R.id.emailEditText);
        passwordEditText = findViewById(R.id.passwordEditText);
        loginButton = findViewById(R.id.loginButton);
        progressBar = findViewById(R.id.progressBar);
        themeSwitch = findViewById(R.id.themeSwitch);
    }

    /**
     * Sets up the theme switch listener to handle light/dark mode toggling.
     */
    private void setupThemeSwitch() {
        SharedPreferences themePreferences = getSharedPreferences(ThemeConstants.PREFS_NAME, MODE_PRIVATE);
        boolean isDarkMode = themePreferences.getBoolean(ThemeConstants.KEY_IS_DARK_MODE, false);
        themeSwitch.setChecked(isDarkMode);

        themeSwitch.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
            } else {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
            }
            SharedPreferences.Editor editor = themePreferences.edit();
            editor.putBoolean(ThemeConstants.KEY_IS_DARK_MODE, isChecked);
            editor.apply();
        });
    }

    /**
     * Sets up the login button click listener to handle user authentication.
     */
    private void setupLoginButton() {
        loginButton.setOnClickListener(v -> {
            if (!validateInputs()) return;
            performLogin();
        });
    }

    /**
     * Validates the email and password inputs.
     * @return true if inputs are valid, false otherwise.
     */
    private boolean validateInputs() {
        String email = emailEditText.getText().toString().trim();
        String password = passwordEditText.getText().toString().trim();

        if (email.isEmpty() || password.isEmpty()) {
            Toast.makeText(LoginActivity.this, "Please enter email and password", Toast.LENGTH_SHORT).show();
            return false;
        }

        return true;
    }

    /**
     * Performs the login operation by calling the API.
     */
    private void performLogin() {
        String email = emailEditText.getText().toString().trim();
        String password = passwordEditText.getText().toString().trim();

        progressBar.setVisibility(View.VISIBLE);
        loginButton.setEnabled(false);

        loginRepository.login(email, password, new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                handleLoginResponse(response);
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                handleLoginFailure(t);
            }
        });
    }

    /**
     * Handles the successful login response.
     * @param response The response from the login API call.
     */
    private void handleLoginResponse(Response<LoginResponse> response) {
        progressBar.setVisibility(View.GONE);
        loginButton.setEnabled(true);

        if (response.isSuccessful() && response.body() != null) {
            LoggedInUser user = response.body().getUser();
            String role = (user.getRoles() != null && !user.getRoles().isEmpty()) ? user.getRoles().get(0) : null;
            String token = response.body().getToken();

            saveUserData(user, role, token);
            navigateToDashboard(user.getFirstName(), role);
        } else {
            Toast.makeText(LoginActivity.this, "Login failed", Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Handles login failure.
     * @param t The throwable from the failed API call.
     */
    private void handleLoginFailure(Throwable t) {
        progressBar.setVisibility(View.GONE);
        loginButton.setEnabled(true);
        Toast.makeText(LoginActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
    }

    /**
     * Saves the user data to SharedPreferences using SessionManager.
     * @param user The logged-in user.
     * @param role The user's role.
     * @param token The JWT token.
     */
    private void saveUserData(LoggedInUser user, String role, String token) {
        sessionManager.saveUserSession(token, user.getId(), user.getEmail(), role);

        // Also save user name for backward compatibility with DashboardActivity
        SharedPreferences authPreferences = getSharedPreferences(AuthConstants.PREFS_NAME, MODE_PRIVATE);
        SharedPreferences.Editor editor = authPreferences.edit();
        editor.putString(AuthConstants.KEY_USER_NAME, user.getFirstName());
        editor.apply();
    }

    /**
     * Navigates to the DashboardActivity with user details.
     * @param name The user's name.
     * @param role The user's role.
     */
    private void navigateToDashboard(String name, String role) {
        // Check if activity is still alive
        if (isFinishing() || isDestroyed()) {
            return;
        }

        Intent intent = new Intent(LoginActivity.this, DashboardActivity.class);
        intent.putExtra("USER_NAME", name);
        if (role != null) {
            intent.putExtra("USER_ROLE", role);
        }
        // Add flags to prevent back navigation to login
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    /**
     * Checks if the user is already logged in by verifying the saved token with a test API call.
     * If valid, proceeds to DashboardActivity; otherwise, shows the login form.
     */
    private void checkAutoLogin() {
        // Quick check first - if not logged in, show login immediately
        if (!sessionManager.isLoggedIn()) {
            showLogin();
            return;
        }

        String token = sessionManager.getToken();
        String savedRole = sessionManager.getUserRole();

        // Get user name from SharedPreferences for backward compatibility
        SharedPreferences authPreferences = getSharedPreferences(AuthConstants.PREFS_NAME, MODE_PRIVATE);
        String savedName = authPreferences.getString(AuthConstants.KEY_USER_NAME, null);

        // Validate we have all required data
        if (token == null || savedName == null || savedRole == null) {
            // Missing data - clear session and show login
            sessionManager.clearSession();
            showLogin();
            return;
        }

        // Show loading state
        progressBar.setVisibility(View.VISIBLE);
        loginButton.setVisibility(View.GONE);
        emailEditText.setVisibility(View.GONE);
        passwordEditText.setVisibility(View.GONE);
        themeSwitch.setVisibility(View.GONE);

        // Validate token with a simple API call
        ThesisRepository thesisRepository = new ThesisRepository(this);
        thesisRepository.getTheses(1, 1, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                // Check if activity is still alive
                if (isFinishing() || isDestroyed()) {
                    return;
                }

                if (response.isSuccessful()) {
                    // Token is valid - navigate to dashboard
                    navigateToDashboard(savedName, savedRole);
                } else {
                    // Token invalid - clear session and show login
                    sessionManager.clearSession();
                    showLogin();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                // Check if activity is still alive
                if (isFinishing() || isDestroyed()) {
                    return;
                }

                // Network error or verification failed
                // Don't clear session on network errors - show login form but keep credentials
                showLogin();
            }
        });
    }

    /**
     * Displays the login form.
     * Note: Does NOT clear session here - caller should decide if session should be cleared.
     */
    private void showLogin() {
        // Check if activity is still alive
        if (isFinishing() || isDestroyed()) {
            return;
        }

        progressBar.setVisibility(View.GONE);
        loginButton.setVisibility(View.VISIBLE);
        emailEditText.setVisibility(View.VISIBLE);
        passwordEditText.setVisibility(View.VISIBLE);
        themeSwitch.setVisibility(View.VISIBLE);
    }
}
