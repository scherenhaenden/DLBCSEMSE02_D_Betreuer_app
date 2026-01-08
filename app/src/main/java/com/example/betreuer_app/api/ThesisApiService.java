package com.example.betreuer_app.api;

import com.example.betreuer_app.model.BillingStatusResponse;
import com.example.betreuer_app.model.CreateThesisRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Multipart;
import retrofit2.http.PATCH;
import retrofit2.http.POST;
import retrofit2.http.Part;
import retrofit2.http.Path;
import retrofit2.http.Query;
import retrofit2.http.Streaming;

import java.util.List;

/**
 * Interface für API-Endpunkte im Bereich der Abschlussarbeiten.
 */
public interface ThesisApiService {

    @POST("theses")
    Call<ThesisApiModel> createThesis(@Body CreateThesisRequest request);

    @Multipart
    @POST("theses")
    Call<ThesisApiModel> createThesisWithFile(
        @Part("Title") RequestBody title,
        @Part("SubjectAreaId") RequestBody subjectAreaId,
        @Part MultipartBody.Part document
    );

    @GET("theses")
    Call<ThesesResponse> getTheses(@Query("page") int page, @Query("pageSize") int pageSize);

    @GET("theses/{id}")
    Call<ThesisApiModel> getThesis(@Path("id") String id);

    @Streaming
    @GET("theses/{id}/document")
    Call<ResponseBody> downloadThesisDocument(@Path("id") String id);

    /**
     * Ruft alle möglichen Abrechnungsstatus ab.
     */
    @GET("theses/billing-statuses")
    Call<List<BillingStatusResponse>> getBillingStatuses();

    /**
     * Aktualisiert den Status einer spezifischen Abschlussarbeit.
     * 
     * @param id Die ID der Arbeit.
     * @param request Das Request-Objekt mit dem neuen Status.
     */
    @PATCH("theses/{id}/status")
    Call<ThesisApiModel> updateStatus(@Path("id") String id, @Body StatusUpdateRequest request);

    /**
     * Aktualisiert den Abrechnungsstatus einer spezifischen Abschlussarbeit.
     *
     * @param id Die ID der Arbeit.
     * @param request Das Request-Objekt mit der neuen Status-ID.
     */
    @PATCH("theses/{id}/billing-status")
    Call<Void> updateBillingStatus(@Path("id") String id, @Body BillingStatusUpdateRequest request);

    /**
     * Datenmodell für das Status-Update-Request.
     */
    class StatusUpdateRequest {
        public String status;
        public StatusUpdateRequest(String status) {
            this.status = status;
        }
    }

    /**
     * Datenmodell für das Billing-Status-Update-Request.
     */
    class BillingStatusUpdateRequest {
        public String billingStatusId;
        public BillingStatusUpdateRequest(String billingStatusId) {
            this.billingStatusId = billingStatusId;
        }
    }
}
