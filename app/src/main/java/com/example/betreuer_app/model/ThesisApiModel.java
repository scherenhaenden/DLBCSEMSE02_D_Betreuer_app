package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * API model for Thesis, combining fields from Thesis and ThesisApi.
 * Used for API interactions.
 */
public class ThesisApiModel extends BaseEntityApiModel {
    private String title;
    private String status;
    private String billingStatus;
    private String ownerId;
    private String tutorId;
    private String secondSupervisorId;
    private String topicId;
    private String documentFileName;

    /**
     * Default constructor.
     */
    public ThesisApiModel() {
    }

    /**
     * Constructor with basic fields.
     */
    public ThesisApiModel(String id, String title, String status, String billingStatus, String ownerId, String tutorId, String secondSupervisorId, String topicId) {
        super();
        setId(UUID.fromString(id));
        this.title = title;
        this.status = status;
        this.billingStatus = billingStatus;
        this.ownerId = ownerId;
        this.tutorId = tutorId;
        this.secondSupervisorId = secondSupervisorId;
        this.topicId = topicId;
    }

    /**
     * Full constructor.
     */
    public ThesisApiModel(String id, String title, String status, String billingStatus, String ownerId, String tutorId, String secondSupervisorId, String topicId, String documentFileName) {
        super();
        setId(UUID.fromString(id));
        this.title = title;
        this.status = status;
        this.billingStatus = billingStatus;
        this.ownerId = ownerId;
        this.tutorId = tutorId;
        this.secondSupervisorId = secondSupervisorId;
        this.topicId = topicId;
        this.documentFileName = documentFileName;
    }

    // Getters and setters
    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
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

    public String getTopicId() {
        return topicId;
    }

    public void setTopicId(String topicId) {
        this.topicId = topicId;
    }

    public String getDocumentFileName() {
        return documentFileName;
    }

    public void setDocumentFileName(String documentFileName) {
        this.documentFileName = documentFileName;
    }
}
