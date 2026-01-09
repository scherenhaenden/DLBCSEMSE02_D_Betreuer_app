package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * API model for Thesis, combining fields from Thesis and ThesisApi.
 * Used for API interactions.
 */
public class ThesisApiModel extends BaseEntityApiModel {
    private String title;
    private String description;
    private String status;
    private String billingStatus;
    private UUID ownerId;
    private UUID tutorId;
    private UUID secondSupervisorId;
    private UUID subjectAreaId;
    private String documentFileName;
    private UUID documentId;

    /**
     * Default constructor.
     */
    public ThesisApiModel() {
    }

    // Getters and setters
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

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getBillingStatus() {
        return billingStatus;
    }

    public void setBillingStatus(String billingStatus) {
        this.billingStatus = billingStatus;
    }

    public UUID getOwnerId() {
        return ownerId;
    }

    public void setOwnerId(UUID ownerId) {
        this.ownerId = ownerId;
    }

    public UUID getTutorId() {
        return tutorId;
    }

    public void setTutorId(UUID tutorId) {
        this.tutorId = tutorId;
    }

    public UUID getSecondSupervisorId() {
        return secondSupervisorId;
    }

    public void setSecondSupervisorId(UUID secondSupervisorId) {
        this.secondSupervisorId = secondSupervisorId;
    }

    public UUID getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(UUID subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }

    public String getDocumentFileName() {
        return documentFileName;
    }

    public void setDocumentFileName(String documentFileName) {
        this.documentFileName = documentFileName;
    }

    public UUID getDocumentId() {
        return documentId;
    }

    public void setDocumentId(UUID documentId) {
        this.documentId = documentId;
    }
}
