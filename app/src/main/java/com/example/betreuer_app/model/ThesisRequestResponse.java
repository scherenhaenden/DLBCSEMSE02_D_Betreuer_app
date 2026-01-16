package com.example.betreuer_app.model;

import java.util.UUID;

public class ThesisRequestResponse {
    private UUID id;
    private UUID thesisId;
    private String thesisTitle;
    private UserResponse requester;
    private UserResponse receiver;
    private String requestType;
    private String status;
    private String message;
    private String createdAt; // Changed from OffsetDateTime to String to avoid Gson deserialization issues
    private String plannedStartOfSupervision;
    private String plannedEndOfSupervision;
    private String documentFileName;
    private UUID documentId;

    public UUID getId() {
        return id;
    }

    public UUID getThesisId() {
        return thesisId;
    }

    public String getThesisTitle() {
        return thesisTitle;
    }

    public UserResponse getRequester() {
        return requester;
    }

    public UserResponse getReceiver() {
        return receiver;
    }

    public String getRequestType() {
        return requestType;
    }

    public String getStatus() {
        return status;
    }

    public String getMessage() {
        return message;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public String getPlannedStartOfSupervision() {
        return plannedStartOfSupervision;
    }

    public String getPlannedEndOfSupervision() {
        return plannedEndOfSupervision;
    }

    public String getDocumentFileName() {
        return documentFileName;
    }

    public UUID getDocumentId() {
        return documentId;
    }
}
