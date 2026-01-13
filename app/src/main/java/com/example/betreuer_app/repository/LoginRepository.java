package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.LoginRequest;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.ThesesResponse;

import retrofit2.Call;
import retrofit2.Callback;

public class LoginRepository {
    private UserApiService apiService;
    private ThesisApiService thesisApiService;
    private Context context;

    // Existing constructor used in the app
    public LoginRepository(Context context) {
        this.context = context;
        apiService = ApiClient.getUserApiService(context);
        thesisApiService = ApiClient.getThesisApiService(context);
    }

    // New constructor for tests / dependency injection
    public LoginRepository(UserApiService apiService) {
        this.apiService = apiService;
    }

    public void login(String email, String password, Callback<LoginResponse> callback) {
        LoginRequest request = new LoginRequest(email, password);
        Call<LoginResponse> call = apiService.login(request);
        call.enqueue(callback);
    }

    /**
     * Validate token by making a test API call
     */
    public void validateToken(Callback<ThesesResponse> callback) {
        if (thesisApiService != null) {
            Call<ThesesResponse> call = thesisApiService.getTheses(1, 1);
            call.enqueue(callback);
        }
    }
}
