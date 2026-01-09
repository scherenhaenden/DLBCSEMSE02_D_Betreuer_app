package com.example.betreuer_app.model;

import java.time.OffsetDateTime;
import java.util.UUID;

public class CreateThesisOfferRequest {
    private String title;
    private UUID subjectAreaId;
    private String description;
    private Integer maxStudents;
    private OffsetDateTime expiresAt;
    private UUID tutorId;

    public CreateThesisOfferRequest(String title, UUID subjectAreaId) {
        this.title = title;
        this.subjectAreaId = subjectAreaId;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public UUID getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(UUID subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
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

    public UUID getTutorId() {
        return tutorId;
    }

    public void setTutorId(UUID tutorId) {
        this.tutorId = tutorId;
    }
}
