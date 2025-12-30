package com.example.betreuer_app.api;

import com.example.betreuer_app.model.TutorProfileResponse;
import com.example.betreuer_app.model.TutorsResponse;
import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface TutorApiService {
    @GET("api/Tutor")
    Call<TutorsResponse> getTutors(
            @Query("topicId") String topicId,
            @Query("topicName") String topicName,
            @Query("page") int page,
            @Query("pageSize") int pageSize
    );

    @GET("api/Tutor/{id}")
    Call<TutorProfileResponse> getTutorById(@Path("id") String id);
}
