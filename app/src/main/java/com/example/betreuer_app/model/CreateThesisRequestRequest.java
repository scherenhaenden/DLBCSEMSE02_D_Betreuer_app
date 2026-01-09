package com.example.betreuer_app.model;

import java.util.UUID;

public class CreateThesisRequestRequest {
    private UUID thesisId;
    private UUID receiverId;
    private String requestType;
    private String message;

    public CreateThesisRequestRequest(UUID thesisId, UUID receiverId, String requestType, String message) {
        this.thesisId = thesisId;
        this.receiverId = receiverId;
        this.requestType = requestType;
        this.message = message;
    }

    public UUID getThesisId() {
        return thesisId;
    }

    public void setThesisId(UUID thesisId) {
        this.thesisId = thesisId;
    }

    public UUID getReceiverId() {
        return receiverId;
    }

    public void setReceiverId(UUID receiverId) {
        this.receiverId = receiverId;
    }

    public String getRequestType() {
        return requestType;
    }

    public void setRequestType(String requestType) {
        this.requestType = requestType;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
