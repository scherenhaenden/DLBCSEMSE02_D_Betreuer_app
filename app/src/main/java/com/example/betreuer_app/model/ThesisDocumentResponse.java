package com.example.betreuer_app.model;

import java.util.UUID;

public class ThesisDocumentResponse {
    private UUID id;
    private String fileName;
    private String contentType;
    private UUID thesisId;
    private UUID userId;

    public ThesisDocumentResponse() {
    }

    public ThesisDocumentResponse(UUID id, String fileName, String contentType, UUID thesisId, UUID userId) {
        this.id = id;
        this.fileName = fileName;
        this.contentType = contentType;
        this.thesisId = thesisId;
        this.userId = userId;
    }

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public String getFileName() {
        return fileName;
    }

    public void setFileName(String fileName) {
        this.fileName = fileName;
    }

    public String getContentType() {
        return contentType;
    }

    public void setContentType(String contentType) {
        this.contentType = contentType;
    }

    public UUID getThesisId() {
        return thesisId;
    }

    public void setThesisId(UUID thesisId) {
        this.thesisId = thesisId;
    }

    public UUID getUserId() {
        return userId;
    }

    public void setUserId(UUID userId) {
        this.userId = userId;
    }
}
