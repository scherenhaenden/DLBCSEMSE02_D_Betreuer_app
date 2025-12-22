package com.example.betreuer_app.api;

import com.example.betreuer_app.model.Thesis;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.PATCH;
import retrofit2.http.Path;

/**
 * Interface für alle API-Aufrufe, die mit Abschlussarbeiten zu tun haben.
 * 
 * Mira-Tipp: Hier definieren wir die "Sprache", die unsere App mit dem 
 * .NET-Backend spricht. 🗣️💻
 */
public interface ThesisApiService {

    /**
     * Aktualisiert nur den Status einer Arbeit.
     * Nutzt PATCH, da wir nur ein einzelnes Feld (den Status) ändern wollen.
     * 
     * @param id Die UUID der Abschlussarbeit
     * @param request Ein kleines Objekt, das den neuen Status enthält
     */
    @PATCH("theses/{id}/status")
    Call<Thesis> updateStatus(@Path("id") String id, @Body StatusUpdateRequest request);

    /**
     * Hilfsklasse für den Request-Body. 
     * Das Backend erwartet ein JSON wie: { "status": "SUBMITTED" }
     */
    class StatusUpdateRequest {
        public String status;
        public StatusUpdateRequest(String status) {
            this.status = status;
        }
    }
}
