package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.betreuer_app.model.LoggedInUser;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.LoginRepository;
import com.example.betreuer_app.util.Resource;
import com.example.betreuer_app.util.SessionManager;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * ViewModel for LoginActivity
 * Handles login, auto-login, and session management logic
 */
public class LoginViewModel extends ViewModel {

    private final LoginRepository loginRepository;
    private final SessionManager sessionManager;

    private final MutableLiveData<Resource<LoginResponse>> loginResult = new MutableLiveData<>();
    private final MutableLiveData<Resource<Boolean>> autoLoginResult = new MutableLiveData<>();

    public LoginViewModel(LoginRepository loginRepository, SessionManager sessionManager) {
        this.loginRepository = loginRepository;
        this.sessionManager = sessionManager;
    }

    public LiveData<Resource<LoginResponse>> getLoginResult() {
        return loginResult;
    }

    public LiveData<Resource<Boolean>> getAutoLoginResult() {
        return autoLoginResult;
    }

    /**
     * Validate login inputs
     */
    public ValidationResult validateInputs(String email, String password) {
        if (email == null || email.trim().isEmpty() || password == null || password.trim().isEmpty()) {
            return new ValidationResult(false, "Please enter email and password");
        }
        return new ValidationResult(true, null);
    }

    /**
     * Perform login
     */
    public void login(String email, String password) {
        loginResult.setValue(Resource.loading(null));

        loginRepository.login(email, password, new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginResponse = response.body();
                    LoggedInUser user = loginResponse.getUser();
                    String role = (user.getRoles() != null && !user.getRoles().isEmpty())
                            ? user.getRoles().get(0)
                            : null;
                    String token = loginResponse.getToken();

                    // Save session
                    sessionManager.saveUserSession(
                            token,
                            user.getId(),
                            user.getEmail(),
                            role
                    );

                    loginResult.setValue(Resource.success(loginResponse));
                } else {
                    loginResult.setValue(Resource.error("Login failed", null));
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                loginResult.setValue(Resource.error("Request failed: " + t.getMessage(), null));
            }
        });
    }

    /**
     * Check if user is already logged in and validate token
     */
    public void checkAutoLogin() {
        if (!sessionManager.isLoggedIn()) {
            autoLoginResult.setValue(Resource.error("Not logged in", false));
            return;
        }

        String token = sessionManager.getToken();
        String savedName = sessionManager.getUserEmail();
        String savedRole = sessionManager.getUserRole();

        if (token == null || savedName == null || savedRole == null) {
            sessionManager.clearSession();
            autoLoginResult.setValue(Resource.error("Invalid session", false));
            return;
        }

        autoLoginResult.setValue(Resource.loading(false));

        // Validate token by making a test API call
        loginRepository.validateToken(new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful()) {
                    autoLoginResult.setValue(Resource.success(true));
                } else {
                    // Token invalid, clear session
                    sessionManager.clearSession();
                    autoLoginResult.setValue(Resource.error("Token invalid", false));
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                // Network error or verification failed
                sessionManager.clearSession();
                autoLoginResult.setValue(Resource.error("Validation failed: " + t.getMessage(), false));
            }
        });
    }

    /**
     * Get user info for navigation
     */
    public UserInfo getUserInfo() {
        return new UserInfo(
                sessionManager.getUserEmail(),
                sessionManager.getUserRole()
        );
    }

    /**
     * Clear session (logout)
     */
    public void logout() {
        sessionManager.clearSession();
    }

    /**
     * Validation result class
     */
    public static class ValidationResult {
        public final boolean isValid;
        public final String errorMessage;

        public ValidationResult(boolean isValid, String errorMessage) {
            this.isValid = isValid;
            this.errorMessage = errorMessage;
        }
    }

    /**
     * User info class for navigation
     */
    public static class UserInfo {
        public final String name;
        public final String role;

        public UserInfo(String name, String role) {
            this.name = name;
            this.role = role;
        }
    }
}

