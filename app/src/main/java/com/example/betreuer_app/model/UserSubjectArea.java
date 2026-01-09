package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents the assignment of a user to a subject area.
 * This is a junction table for the many-to-many relationship between User and SubjectArea.
 */
public class UserSubjectArea {
    private String userId;
    private UserApiModel user;
    private UUID subjectAreaId;
    private SubjectAreaResponse subjectArea;

    /**
     * Returns the user ID.
     * @return The UUID of the user.
     */
    public String getUserId() {
        return userId;
    }

    /**
     * Sets the user ID.
     * @param userId The new user ID.
     */
    public void setUserId(String userId) {
        this.userId = userId;
    }

    /**
     * Returns the User object.
     * @return The User object.
     */
    public UserApiModel getUser() {
        return user;
    }

    /**
     * Sets the User object.
     * @param user The new User object.
     */
    public void setUser(UserApiModel user) {
        this.user = user;
    }

    /**
     * Returns the subject area ID.
     * @return The UUID of the subject area.
     */
    public UUID getSubjectAreaId() {
        return subjectAreaId;
    }

    /**
     * Sets the subject area ID.
     * @param subjectAreaId The new subject area ID.
     */
    public void setSubjectAreaId(UUID subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }

    /**
     * Returns the SubjectArea object.
     * @return The SubjectArea object.
     */
    public SubjectAreaResponse getSubjectArea() {
        return subjectArea;
    }

    /**
     * Sets the SubjectArea object.
     * @param subjectArea The new SubjectArea object.
     */
    public void setSubjectArea(SubjectAreaResponse subjectArea) {
        this.subjectArea = subjectArea;
    }
}
