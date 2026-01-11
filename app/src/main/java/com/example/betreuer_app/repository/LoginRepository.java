package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.LoginRequest;
import com.example.betreuer_app.model.LoginResponse;

import retrofit2.Call;
import retrofit2.Callback;

public class LoginRepository {
    private UserApiService apiService;

    // Existing constructor used in the app
    public LoginRepository(Context context) {
        apiService = ApiClient.getUserApiService(context);
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
}
