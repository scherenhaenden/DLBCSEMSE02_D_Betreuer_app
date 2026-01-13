package com.example.betreuer_app.api;

import com.example.betreuer_app.model.CreateThesisRequestRequest;
import com.example.betreuer_app.model.RespondToThesisRequestRequest;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.DELETE;
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

    @GET("thesis-requests/{id}")
    Call<ThesisRequestResponse> getThesisRequest(
            @Path("id") UUID id
    );

    @POST("thesis-requests/{id}/respond")
    Call<Void> respondToRequest(
            @Path("id") UUID id,
            @Body RespondToThesisRequestRequest request
    );

    @POST("thesis-requests")
    Call<ThesisRequestResponse> createRequest(
            @Body CreateThesisRequestRequest request
    );

    @DELETE("thesis-requests/{id}")
    Call<Void> deleteRequest(
            @Path("id") UUID id
    );

    @GET("thesis-requests/tutor/receiver")
    Call<ThesisRequestResponsePaginatedResponse> getIncomingRequests(
            @Query("status") String status,
            @Query("page") int page,
            @Query("pageSize") int pageSize
    );
}
