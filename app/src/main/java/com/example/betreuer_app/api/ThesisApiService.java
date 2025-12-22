package com.example.betreuer_app.api;

import com.example.betreuer_app.model.Thesis;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.PATCH;
import retrofit2.http.Path;

/**
 * Interface für API-Endpunkte im Bereich der Abschlussarbeiten.
 */
public interface ThesisApiService {

    /**
     * Aktualisiert den Status einer spezifischen Abschlussarbeit.
     * 
     * @param id Die ID der Arbeit.
     * @param request Das Request-Objekt mit dem neuen Status.
     */
    @PATCH("theses/{id}/status")
    Call<Thesis> updateStatus(@Path("id") String id, @Body StatusUpdateRequest request);

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
