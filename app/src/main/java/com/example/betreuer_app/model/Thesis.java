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

    /**
     * Returns the ID.
     */
    public int getId() {
        return id;
    }

    /**
     * Returns the title.
     */
    public String getTitle() {
        return title;
    }

    /**
     * Sets the title of the object.
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * Returns the current status.
     */
    public Status getStatus() {
        return status;
    }

    /**
     * Sets the status.
     */
    public void setStatus(Status status) {
        this.status = status;
    }

    /**
     * Returns the field of study.
     */
    public String getFieldOfStudy() {
        return fieldOfStudy;
    }

    /**
     * Sets the field of study.
     */
    public void setFieldOfStudy(String fieldOfStudy) {
        this.fieldOfStudy = fieldOfStudy;
    }

    /**
     * Returns the student ID.
     */
    public int getStudentId() {
        return studentId;
    }

    /**
     * Sets the student ID.
     */
    public void setStudentId(int studentId) {
        this.studentId = studentId;
    }

    /**
     * Returns the supervisor ID.
     */
    public int getSupervisorId() {
        return supervisorId;
    }

    /**
     * Sets the supervisor ID.
     */
    public void setSupervisorId(int supervisorId) {
        this.supervisorId = supervisorId;
    }

    /**
     * Returns the ID of the second examiner.
     */
    public int getSecondExaminerId() {
        return secondExaminerId;
    }

    /**
     * Sets the ID of the second examiner.
     */
    public void setSecondExaminerId(int secondExaminerId) {
        this.secondExaminerId = secondExaminerId;
    }

    /**
     * Returns the expose path.
     */
    public String getExposePath() {
        return exposePath;
    }

    /**
     * Sets the expose path.
     */
    public void setExposePath(String exposePath) {
        this.exposePath = exposePath;
    }

    /**
     * Returns the current billing status.
     */
    public BillingStatus getBillingStatus() {
        return billingStatus;
    }

    public void setBillingStatus(BillingStatus billingStatus) {
        this.billingStatus = billingStatus;
    }
}
