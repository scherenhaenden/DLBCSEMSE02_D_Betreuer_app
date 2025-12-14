package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.UsersResponse;

import retrofit2.Call;
import retrofit2.Callback;

public class UserRepository {
    private UserApiService apiService;

    public UserRepository(Context context) {
        apiService = ApiClient.getUserApiService(context);
    }

    public void getUsers(int page, int pageSize, Callback<UsersResponse> callback) {
        Call<UsersResponse> call = apiService.getUsers(page, pageSize);
        call.enqueue(callback);
    }
}
