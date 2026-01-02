package com.example.betreuer_app.api;

import com.example.betreuer_app.model.TopicsResponse;
import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface TopicApiService {
    @GET("topics")
    Call<TopicsResponse> getTopics(@Query("page") int page, @Query("pageSize") int pageSize);

    @GET("topics/search")
    Call<TopicsResponse> searchTopics(@Query("q") String query, @Query("page") int page, @Query("pageSize") int pageSize);
}
