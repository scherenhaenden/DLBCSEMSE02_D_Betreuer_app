package com.example.betreuer_app.model;

public class Thesis {
    // Enums

    public enum Status {
        IN_DISCUSSION, REGISTERED, SUBMITTED, COLLOQUIUM_HELD
    }

    public enum BillingStatus {
        NONE, ISSUED, PAID
    }

    // Attributes

    private int id;
    private String title;
    private Status status;
    private String fieldOfStudy;
    private int studentId;
    private int supervisorId;
    private int secondExaminerId;
    private String exposePath;
    private BillingStatus billingStatus;

    // Constructor

    public Thesis(int id, String title, Status status, String fieldOfStudy, int studentId,
                  int supervisorId, int secondExaminerId, String exposePath,
                  BillingStatus billingStatus) {
        this.id = id;
        this.title = title;
        this.status = status;
        this.fieldOfStudy = fieldOfStudy;
        this.studentId = studentId;
        this.supervisorId = supervisorId;
        this.secondExaminerId = secondExaminerId;
        this.exposePath = exposePath;
        this.billingStatus = billingStatus;
    }

    // Methods

    public int getId() {
        return id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public Status getStatus() {
        return status;
    }

    public void setStatus(Status status) {
        this.status = status;
    }

    public String getFieldOfStudy() {
        return fieldOfStudy;
    }

    public void setFieldOfStudy(String fieldOfStudy) {
        this.fieldOfStudy = fieldOfStudy;
    }

    public int getStudentId() {
        return studentId;
    }

    public void setStudentId(int studentId) {
        this.studentId = studentId;
    }

    public int getSupervisorId() {
        return supervisorId;
    }

    public void setSupervisorId(int supervisorId) {
        this.supervisorId = supervisorId;
    }

    public int getSecondExaminerId() {
        return secondExaminerId;
    }

    public void setSecondExaminerId(int secondExaminerId) {
        this.secondExaminerId = secondExaminerId;
    }

    public String getExposePath() {
        return exposePath;
    }

    public void setExposePath(String exposePath) {
        this.exposePath = exposePath;
    }

    public BillingStatus getBillingStatus() {
        return billingStatus;
    }

    public void setBillingStatus(BillingStatus billingStatus) {
        this.billingStatus = billingStatus;
    }
}
