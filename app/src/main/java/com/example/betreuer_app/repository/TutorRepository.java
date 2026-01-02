package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.TutorApiService;
import com.example.betreuer_app.model.TutorProfileResponse;
import com.example.betreuer_app.model.TutorsResponse;
import retrofit2.Call;
import retrofit2.Callback;

public class TutorRepository {
    private TutorApiService apiService;

    public TutorRepository(Context context) {
        apiService = ApiClient.getTutorApiService(context);
    }

    public void getTutors(String subjectAreaId, String subjectAreaName, String name, int page, int pageSize, Callback<TutorsResponse> callback) {
        Call<TutorsResponse> call = apiService.getTutors(subjectAreaId, subjectAreaName, name, page, pageSize);
        call.enqueue(callback);
    }

    public void getTutorById(String id, Callback<TutorProfileResponse> callback) {
        Call<TutorProfileResponse> call = apiService.getTutorById(id);
        call.enqueue(callback);
    }
}
