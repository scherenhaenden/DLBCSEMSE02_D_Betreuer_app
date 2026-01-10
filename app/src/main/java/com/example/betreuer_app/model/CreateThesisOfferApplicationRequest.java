package com.example.betreuer_app.model;

import java.util.UUID;

public class CreateThesisOfferApplicationRequest {
    private UUID thesisOfferId;
    private String message;
    private UUID studentId;

    public CreateThesisOfferApplicationRequest(UUID thesisOfferId, String message, UUID studentId) {
        this.thesisOfferId = thesisOfferId;
        this.message = message;
        this.studentId = studentId;
    }
}
