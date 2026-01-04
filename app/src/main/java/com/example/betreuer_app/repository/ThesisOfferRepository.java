package com.example.betreuer_app.repository;

import android.content.Context;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisOfferApiService;
import com.example.betreuer_app.model.CreateThesisOfferRequest;
import com.example.betreuer_app.model.ThesisOfferApiModel;
import com.example.betreuer_app.model.ThesisOfferResponse;
import com.example.betreuer_app.model.UpdateThesisOfferRequest;
import retrofit2.Call;
import retrofit2.Callback;

import java.util.UUID;

public class ThesisOfferRepository {
    private final ThesisOfferApiService apiService;

    public ThesisOfferRepository(Context context) {
        this.apiService = ApiClient.getThesisOfferApiService(context);
    }

    public void createThesisOffer(CreateThesisOfferRequest request, Callback<ThesisOfferApiModel> callback) {
        apiService.createThesisOffer(request).enqueue(callback);
    }

    public void getThesisOffersByUser(UUID userId, int page, int pageSize, Callback<ThesisOfferResponse> callback) {
        apiService.getThesisOffersByUser(userId, page, pageSize).enqueue(callback);
    }
    
    public void updateThesisOffer(UUID id, UpdateThesisOfferRequest request, Callback<ThesisOfferApiModel> callback) {
        apiService.updateThesisOffer(id, request).enqueue(callback);
    }
}
