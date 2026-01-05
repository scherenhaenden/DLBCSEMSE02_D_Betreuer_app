package com.example.betreuer_app.model;

import java.time.OffsetDateTime;
import java.util.UUID;

public class ThesisOfferApiModel extends BaseEntityApiModel {
    private String title;
    private String description;
    private UUID subjectAreaId;
    private UUID tutorId;
    private String status;
    private Integer maxStudents;
    private OffsetDateTime expiresAt;

    public ThesisOfferApiModel() {
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

    public UUID getTutorId() {
        return tutorId;
    }

    public void setTutorId(UUID tutorId) {
        this.tutorId = tutorId;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
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
