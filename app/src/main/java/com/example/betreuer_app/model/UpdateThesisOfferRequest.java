package com.example.betreuer_app.model;

import java.time.OffsetDateTime;
import java.util.UUID;

public class UpdateThesisOfferRequest {
    private String title;
    private String description;
    private UUID subjectAreaId;
    private Integer maxStudents;
    private OffsetDateTime expiresAt;

    public UpdateThesisOfferRequest() {
    }

    public UpdateThesisOfferRequest(String title, String description, UUID subjectAreaId) {
        this.title = title;
        this.description = description;
        this.subjectAreaId = subjectAreaId;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public UUID getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(UUID subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }

    public Integer getMaxStudents() {
        return maxStudents;
    }

    public void setMaxStudents(Integer maxStudents) {
        this.maxStudents = maxStudents;
    }

    public OffsetDateTime getExpiresAt() {
        return expiresAt;
    }

    public void setExpiresAt(OffsetDateTime expiresAt) {
        this.expiresAt = expiresAt;
    }
}
