package com.example.betreuer_app.api;

import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface SubjectAreaApiService {
    @GET("subject-areas")
    Call<SubjectAreaResponsePaginatedResponse> getSubjectAreas(@Query("page") int page, @Query("pageSize") int pageSize);

    @GET("subject-areas/{id}")
    Call<SubjectAreaResponse> getSubjectArea(@Path("id") UUID id);

    @GET("subject-areas/search")
    Call<SubjectAreaResponsePaginatedResponse> searchSubjectAreas(@Query("q") String query, @Query("page") int page, @Query("pageSize") int pageSize);
}
