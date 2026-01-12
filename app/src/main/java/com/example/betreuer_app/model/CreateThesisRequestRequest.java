package com.example.betreuer_app.model;

import java.util.UUID;

public class CreateThesisRequestRequest {
    private UUID thesisId;
    private UUID receiverId;
    private String requestType;
    private String message;
    private String plannedStartOfSupervision;
    private String plannedEndOfSupervision;

    public CreateThesisRequestRequest(UUID thesisId, UUID receiverId, String requestType, String message, String plannedStartOfSupervision, String plannedEndOfSupervision) {
        this.thesisId = thesisId;
        this.receiverId = receiverId;
        this.requestType = requestType;
        this.message = message;
        this.plannedStartOfSupervision = plannedStartOfSupervision;
        this.plannedEndOfSupervision = plannedEndOfSupervision;
    }

    // Getters and setters can be added if needed for serialization or other purposes
}
