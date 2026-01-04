package com.example.betreuer_app.api;

import com.example.betreuer_app.model.RespondToThesisRequestRequest;
import com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ThesisRequestApiService {

    @GET("thesis-requests")
    Call<ThesisRequestResponsePaginatedResponse> getMyRequests(
            @Query("page") int page,
            @Query("pageSize") int pageSize
    );

    @POST("thesis-requests/{id}/respond")
    Call<Void> respondToRequest(
            @Path("id") UUID id,
            @Body RespondToThesisRequestRequest request
    );
}
