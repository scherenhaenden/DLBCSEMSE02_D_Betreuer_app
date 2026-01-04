package com.example.betreuer_app.model;

import java.time.OffsetDateTime;
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
    private OffsetDateTime createdAt;

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

    public OffsetDateTime getCreatedAt() {
        return createdAt;
    }
}
