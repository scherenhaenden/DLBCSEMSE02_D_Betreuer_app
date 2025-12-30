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

import com.example.betreuer_app.model.LoggedInUser;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.LoginRepository;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.switchmaterial.SwitchMaterial;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {

    private EditText emailEditText;
    private EditText passwordEditText;
    private Button loginButton;
    private ProgressBar progressBar;
    private LoginRepository loginRepository;
    private SwitchMaterial themeSwitch;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        emailEditText = findViewById(R.id.emailEditText);
        passwordEditText = findViewById(R.id.passwordEditText);
        loginButton = findViewById(R.id.loginButton);
        progressBar = findViewById(R.id.progressBar);
        themeSwitch = findViewById(R.id.themeSwitch);

        loginRepository = new LoginRepository(this);

        SharedPreferences themePreferences = getSharedPreferences("theme_prefs", MODE_PRIVATE);
        boolean isDarkMode = themePreferences.getBoolean("is_dark_mode", false);
        themeSwitch.setChecked(isDarkMode);

        themeSwitch.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
            } else {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
            }
            SharedPreferences.Editor editor = themePreferences.edit();
            editor.putBoolean("is_dark_mode", isChecked);
            editor.apply();
        });

        loginButton.setOnClickListener(v -> {
            String email = emailEditText.getText().toString().trim();
            String password = passwordEditText.getText().toString().trim();

            if (email.isEmpty() || password.isEmpty()) {
                Toast.makeText(LoginActivity.this, "Please enter email and password", Toast.LENGTH_SHORT).show();
                return;
            }

            progressBar.setVisibility(View.VISIBLE);
            loginButton.setEnabled(false);

            loginRepository.login(email, password, new Callback<LoginResponse>() {
                @Override
                public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                    progressBar.setVisibility(View.GONE);
                    loginButton.setEnabled(true);

                    if (response.isSuccessful() && response.body() != null) {
                        LoggedInUser user = response.body().getUser();
                        String role = (user.getRoles() != null && !user.getRoles().isEmpty()) ? user.getRoles().get(0) : null;

                        // Save the token and user info
                        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
                        SharedPreferences.Editor editor = authPreferences.edit();
                        editor.putString("jwt_token", response.body().getToken());
                        editor.putString("user_name", user.getFirstName());
                        editor.putString("user_role", role);
                        editor.apply();

                        Intent intent = new Intent(LoginActivity.this, DashboardActivity.class);
                        intent.putExtra("USER_NAME", user.getFirstName());
                        if (role != null) {
                            intent.putExtra("USER_ROLE", role);
                        }
                        startActivity(intent);
                        finish();
                    } else {
                        Toast.makeText(LoginActivity.this, "Login failed", Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(Call<LoginResponse> call, Throwable t) {
                    progressBar.setVisibility(View.GONE);
                    loginButton.setEnabled(true);
                    Toast.makeText(LoginActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            });
        });

        // Auto-login check
        checkAutoLogin();
    }

    private void checkAutoLogin() {
        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
        String token = authPreferences.getString("jwt_token", null);
        String savedName = authPreferences.getString("user_name", null);
        String savedRole = authPreferences.getString("user_role", null);
        if (token != null && savedName != null && savedRole != null) {
            progressBar.setVisibility(View.VISIBLE);
            loginButton.setVisibility(View.GONE);
            emailEditText.setVisibility(View.GONE);
            passwordEditText.setVisibility(View.GONE);
            themeSwitch.setVisibility(View.GONE);

            ThesisRepository thesisRepository = new ThesisRepository(this);
            thesisRepository.getTheses(1, 1, new Callback<ThesesResponse>() {
                @Override
                public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                    if (response.isSuccessful()) {
                        Intent intent = new Intent(LoginActivity.this, DashboardActivity.class);
                        intent.putExtra("USER_NAME", savedName);
                        intent.putExtra("USER_ROLE", savedRole);
                        startActivity(intent);
                        finish();
                    } else {
                        // Token invalid
                        showLogin();
                    }
                }

                @Override
                public void onFailure(Call<ThesesResponse> call, Throwable t) {
                    // Network error or verification failed
                    showLogin();
                }
            });
        } else {
            // No valid saved data, show login
            showLogin();
        }
    }

    private void showLogin() {
        // Clear token as it might be invalid
        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
        SharedPreferences.Editor editor = authPreferences.edit();
        editor.clear();
        editor.apply();

        progressBar.setVisibility(View.GONE);
        loginButton.setVisibility(View.VISIBLE);
        emailEditText.setVisibility(View.VISIBLE);
        passwordEditText.setVisibility(View.VISIBLE);
        themeSwitch.setVisibility(View.VISIBLE);
    }
}
