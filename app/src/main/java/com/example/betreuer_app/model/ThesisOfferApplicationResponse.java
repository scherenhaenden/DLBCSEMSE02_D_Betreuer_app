package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents the response object received after creating a thesis offer application.
 */
public class ThesisOfferApplicationResponse {
    private UUID id;

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }
}
