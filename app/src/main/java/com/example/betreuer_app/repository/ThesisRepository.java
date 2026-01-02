package com.example.betreuer_app.repository;

import android.content.Context;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.CreateThesisRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;

import retrofit2.Call;
import retrofit2.Callback;

public class ThesisRepository {
    private ThesisApiService apiService;

    public ThesisRepository(Context context) {
        apiService = ApiClient.getThesisApiService(context);
    }

    public void getTheses(int page, int pageSize, Callback<ThesesResponse> callback) {
        Call<ThesesResponse> call = apiService.getTheses(page, pageSize);
        call.enqueue(callback);
    }

    public void createThesis(String title, String topicId, Callback<ThesisApiModel> callback) {
        CreateThesisRequest request = new CreateThesisRequest(title, topicId);
        Call<ThesisApiModel> call = apiService.createThesis(request);
        call.enqueue(callback);
    }
}
