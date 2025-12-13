package com.example.betreuer_app.repository;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.UsersResponse;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class UserRepository {
    private UserApiService apiService;

    public UserRepository() {
        apiService = ApiClient.getUserApiService();
    }

    public void getUsers(int page, int pageSize, Callback<UsersResponse> callback) {
        Call<UsersResponse> call = apiService.getUsers(page, pageSize);
        call.enqueue(callback);
    }
}
