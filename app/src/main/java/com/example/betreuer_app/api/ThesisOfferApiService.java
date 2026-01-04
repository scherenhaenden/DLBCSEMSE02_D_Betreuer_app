package com.example.betreuer_app.api;

import com.example.betreuer_app.model.CreateThesisOfferRequest;
import com.example.betreuer_app.model.ThesisOfferApiModel;
import com.example.betreuer_app.model.ThesisOfferResponse;
import com.example.betreuer_app.model.UpdateThesisOfferRequest;

import java.util.UUID;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ThesisOfferApiService {

    @GET("thesis-offers")
    Call<ThesisOfferResponse> getThesisOffers(
            @Query("page") Integer page,
            @Query("pageSize") Integer pageSize
    );

    @POST("thesis-offers")
    Call<ThesisOfferApiModel> createThesisOffer(@Body CreateThesisOfferRequest request);

    @GET("thesis-offers/user/{userId}")
    Call<ThesisOfferResponse> getThesisOffersByUser(
            @Path("userId") UUID userId,
            @Query("page") Integer page,
            @Query("pageSize") Integer pageSize
    );

    @PUT("thesis-offers/{id}")
    Call<ThesisOfferApiModel> updateThesisOffer(
            @Path("id") UUID id,
            @Body UpdateThesisOfferRequest request
    );
}
