package com.example.betreuer_app.repository;

import android.content.Context;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.ThesesResponse;

import retrofit2.Call;
import retrofit2.Callback;

public class ThesisRepository {
    private UserApiService apiService;

    public ThesisRepository(Context context) {
        apiService = ApiClient.getUserApiService(context);
    }

    public void getTheses(int page, int pageSize, Callback<ThesesResponse> callback) {
        Call<ThesesResponse> call = apiService.getTheses(page, pageSize);
        call.enqueue(callback);
    }
}
