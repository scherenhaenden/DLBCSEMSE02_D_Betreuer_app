package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import retrofit2.Call;
import retrofit2.Callback;

public class SubjectAreaRepository {
    private SubjectAreaApiService apiService;

    public SubjectAreaRepository(Context context) {
        apiService = ApiClient.getSubjectAreaApiService(context);
    }

    public void getSubjectAreas(int page, int pageSize, Callback<SubjectAreaResponsePaginatedResponse> callback) {
        Call<SubjectAreaResponsePaginatedResponse> call = apiService.getSubjectAreas(page, pageSize);
        call.enqueue(callback);
    }

    public void searchSubjectAreas(String query, int page, int pageSize, Callback<SubjectAreaResponsePaginatedResponse> callback) {
        Call<SubjectAreaResponsePaginatedResponse> call = apiService.searchSubjectAreas(query, page, pageSize);
        call.enqueue(callback);
    }
}
