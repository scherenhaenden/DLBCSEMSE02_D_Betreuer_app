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
    private String ownerId;
    private String tutorId;
    private String secondSupervisorId;
    private String subjectAreaId;
    private String documentFileName;

    /**
     * Default constructor.
     */
    public ThesisApiModel() {
    }

    /**
     * Constructor with basic fields.
     */
    public ThesisApiModel(String id, String title, String description, String status, String billingStatus, String ownerId, String tutorId, String secondSupervisorId, String subjectAreaId) {
        super();
        setId(UUID.fromString(id));
        this.title = title;
        this.description = description;
        this.status = status;
        this.billingStatus = billingStatus;
        this.ownerId = ownerId;
        this.tutorId = tutorId;
        this.secondSupervisorId = secondSupervisorId;
        this.subjectAreaId = subjectAreaId;
    }

    /**
     * Full constructor.
     */
    public ThesisApiModel(String id, String title, String description, String status, String billingStatus, String ownerId, String tutorId, String secondSupervisorId, String subjectAreaId, String documentFileName) {
        super();
        setId(UUID.fromString(id));
        this.title = title;
        this.description = description;
        this.status = status;
        this.billingStatus = billingStatus;
        this.ownerId = ownerId;
        this.tutorId = tutorId;
        this.secondSupervisorId = secondSupervisorId;
        this.subjectAreaId = subjectAreaId;
        this.documentFileName = documentFileName;
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

    public String getOwnerId() {
        return ownerId;
    }

    public void setOwnerId(String ownerId) {
        this.ownerId = ownerId;
    }

    public String getTutorId() {
        return tutorId;
    }

    public void setTutorId(String tutorId) {
        this.tutorId = tutorId;
    }

    public String getSecondSupervisorId() {
        return secondSupervisorId;
    }

    public void setSecondSupervisorId(String secondSupervisorId) {
        this.secondSupervisorId = secondSupervisorId;
    }

    public String getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(String subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }

    public String getDocumentFileName() {
        return documentFileName;
    }

    public void setDocumentFileName(String documentFileName) {
        this.documentFileName = documentFileName;
    }
}
