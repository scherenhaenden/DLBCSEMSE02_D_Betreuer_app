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
import com.example.betreuer_app.repository.LoginRepository;
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
                        // Save the token
                        SharedPreferences authPreferences = getSharedPreferences("auth_prefs", MODE_PRIVATE);
                        SharedPreferences.Editor editor = authPreferences.edit();
                        editor.putString("jwt_token", response.body().getToken());
                        editor.apply();

                        LoggedInUser user = response.body().getUser();
                        Intent intent = new Intent(LoginActivity.this, DashboardActivity.class);
                        intent.putExtra("USER_NAME", user.getFirstName());
                        if (user.getRoles() != null && !user.getRoles().isEmpty()) {
                            intent.putExtra("USER_ROLE", user.getRoles().get(0));
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
    }
}
