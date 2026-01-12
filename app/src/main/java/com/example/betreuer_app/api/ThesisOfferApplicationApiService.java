package com.example.betreuer_app.api;

import com.example.betreuer_app.model.CreateThesisOfferApplicationRequest;
import com.example.betreuer_app.model.ThesisOfferApplicationResponse;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface ThesisOfferApplicationApiService {
    @POST("thesis-offer-applications")
    Call<ThesisOfferApplicationResponse> createApplication(@Body CreateThesisOfferApplicationRequest request);
}
