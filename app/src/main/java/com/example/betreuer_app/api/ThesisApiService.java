package com.example.betreuer_app.api;

import com.example.betreuer_app.model.CreateThesisRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

/**
 * Interface für API-Endpunkte im Bereich der Abschlussarbeiten.
 */
public interface ThesisApiService {

    @POST("theses")
    Call<ThesisApiModel> createThesis(@Body CreateThesisRequest request);

    @GET("theses")
    Call<ThesesResponse> getTheses(@Query("page") int page, @Query("pageSize") int pageSize);

    @GET("theses/{id}")
    Call<ThesisApiModel> getThesis(@Path("id") String id);

    /**
     * Aktualisiert den Status einer spezifischen Abschlussarbeit.
     * 
     * @param id Die ID der Arbeit.
     * @param request Das Request-Objekt mit dem neuen Status.
     */
    @PATCH("theses/{id}/status")
    Call<ThesisApiModel> updateStatus(@Path("id") String id, @Body StatusUpdateRequest request);

    /**
     * Datenmodell für das Status-Update-Request.
     */
    class StatusUpdateRequest {
        public String status;
        public StatusUpdateRequest(String status) {
            this.status = status;
        }
    }
}
