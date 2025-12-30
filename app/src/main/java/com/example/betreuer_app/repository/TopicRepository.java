package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.TopicApiService;
import com.example.betreuer_app.model.TopicsResponse;
import retrofit2.Call;
import retrofit2.Callback;

public class TopicRepository {
    private TopicApiService apiService;

    public TopicRepository(Context context) {
        apiService = ApiClient.getTopicApiService(context);
    }

    public void getTopics(int page, int pageSize, Callback<TopicsResponse> callback) {
        Call<TopicsResponse> call = apiService.getTopics(page, pageSize);
        call.enqueue(callback);
    }

    public void searchTopics(String query, int page, int pageSize, Callback<TopicsResponse> callback) {
        Call<TopicsResponse> call = apiService.searchTopics(query, page, pageSize);
        call.enqueue(callback);
    }
}
