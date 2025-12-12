package com.example.betreuer_app.model;

/**
 * Represents a thesis.
 */
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

    /**
     * Constructs a new Thesis object.
     * @param id The ID of the thesis.
     * @param title The title of the thesis.
     * @param status The current status of the thesis.
     * @param fieldOfStudy The field of study for the thesis.
     * @param studentId The ID of the student.
     * @param supervisorId The ID of the supervisor.
     * @param secondExaminerId The ID of the second examiner.
     * @param exposePath The path to the exposé file.
     * @param billingStatus The billing status of the thesis.
     */
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
     * Returns the ID of the thesis.
     * @return The ID.
     */
    public int getId() {
        return id;
    }

    /**
     * Returns the title of the thesis.
     * @return The title.
     */
    public String getTitle() {
        return title;
    }

    /**
     * Sets the title of the thesis.
     * @param title The new title.
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * Returns the current status of the thesis.
     * @return The status.
     */
    public Status getStatus() {
        return status;
    }

    /**
     * Sets the status of the thesis.
     * @param status The new status.
     */
    public void setStatus(Status status) {
        this.status = status;
    }

    /**
     * Returns the field of study for the thesis.
     * @return The field of study.
     */
    public String getFieldOfStudy() {
        return fieldOfStudy;
    }

    /**
     * Sets the field of study for the thesis.
     * @param fieldOfStudy The new field of study.
     */
    public void setFieldOfStudy(String fieldOfStudy) {
        this.fieldOfStudy = fieldOfStudy;
    }

    /**
     * Returns the student's ID.
     * @return The student ID.
     */
    public int getStudentId() {
        return studentId;
    }

    /**
     * Sets the student's ID.
     * @param studentId The new student ID.
     */
    public void setStudentId(int studentId) {
        this.studentId = studentId;
    }

    /**
     * Returns the supervisor's ID.
     * @return The supervisor ID.
     */
    public int getSupervisorId() {
        return supervisorId;
    }

    /**
     * Sets the supervisor's ID.
     * @param supervisorId The new supervisor ID.
     */
    public void setSupervisorId(int supervisorId) {
        this.supervisorId = supervisorId;
    }

    /**
     * Returns the second examiner's ID.
     * @return The second examiner ID.
     */
    public int getSecondExaminerId() {
        return secondExaminerId;
    }

    /**
     * Sets the second examiner's ID.
     * @param secondExaminerId The new second examiner ID.
     */
    public void setSecondExaminerId(int secondExaminerId) {
        this.secondExaminerId = secondExaminerId;
    }

    /**
     * Returns the path to the exposé file.
     * @return The exposé path.
     */
    public String getExposePath() {
        return exposePath;
    }

    /**
     * Sets the path to the exposé file.
     * @param exposePath The new exposé path.
     */
    public void setExposePath(String exposePath) {
        this.exposePath = exposePath;
    }

    /**
     * Returns the billing status of the thesis.
     * @return The billing status.
     */
    public BillingStatus getBillingStatus() {
        return billingStatus;
    }

    /**
     * Sets the billing status of the thesis.
     * @param billingStatus The new billing status.
     */
    public void setBillingStatus(BillingStatus billingStatus) {
        this.billingStatus = billingStatus;
    }
}
