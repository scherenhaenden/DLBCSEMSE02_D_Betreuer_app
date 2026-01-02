package com.example.betreuer_app.model;

import java.util.List;
import java.util.UUID;
import android.util.Log;

/**
 * SubjectAreaResponse represents a subject area entity from the API.
 * It extends BaseEntityApiModel and includes properties like title, description, subject area,
 * active status, and a list of associated tutor IDs.
 * The constructor handles UUID parsing for the ID field, logging errors for invalid UUIDs.
 */
public class SubjectAreaResponse extends BaseEntityApiModel {
    private String title;
    private String description;
    private String subjectArea;
    private boolean isActive;
    private List<String> tutorIds;

    /**
     * Constructs a new SubjectAreaResponse with the specified parameters.
     * Attempts to parse the ID as a UUID; if invalid, logs an error and leaves the ID unset.
     * @param id The unique identifier for the subject area as a string (expected to be a valid UUID).
     * @param title The title of the subject area.
     * @param description A detailed description of the subject area.
     * @param subjectArea The subject area associated with the subject area.
     * @param isActive Indicates whether the subject area is active.
     * @param tutorIds A list of tutor IDs associated with this subject area.
     */
    public SubjectAreaResponse(String id, String title, String description, String subjectArea, boolean isActive, List<String> tutorIds) {
        super();
        if (id != null) {
            try {
                setId(UUID.fromString(id));
            } catch (IllegalArgumentException e) {
                Log.e("SubjectAreaResponse", "Invalid UUID string: " + id, e);
            }
        }
        this.title = title;
        this.description = description;
        this.subjectArea = subjectArea;
        this.isActive = isActive;
        this.tutorIds = tutorIds;
        Log.d("SubjectAreaResponse", "SubjectAreaResponse created with id: " + id);
    }

    /**
     * Gets the title of the subject area.
     * @return The title as a string.
     */
    public String getTitle() {
        return title;
    }

    /**
     * Sets the title of the subject area.
     * @param title The new title to set.
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * Gets the description of the subject area.
     * @return The description as a string.
     */
    public String getDescription() {
        return description;
    }

    /**
     * Sets the description of the subject area.
     * @param description The new description to set.
     */
    public void setDescription(String description) {
        this.description = description;
    }

    /**
     * Gets the subject area of the subject area.
     * @return The subject area as a string.
     */
    public String getSubjectArea() {
        return subjectArea;
    }

    /**
     * Sets the subject area of the subject area.
     * @param subjectArea The new subject area to set.
     */
    public void setSubjectArea(String subjectArea) {
        this.subjectArea = subjectArea;
    }

    /**
     * Checks if the subject area is active.
     * @return true if the subject area is active, false otherwise.
     */
    public boolean isActive() {
        return isActive;
    }

    /**
     * Sets the active status of the subject area.
     * @param active The new active status to set.
     */
    public void setActive(boolean active) {
        isActive = active;
    }

    /**
     * Gets the list of tutor IDs associated with this subject area.
     * @return A list of tutor IDs as strings.
     */
    public List<String> getTutorIds() {
        return tutorIds;
    }

    /**
     * Sets the list of tutor IDs associated with this subject area.
     * @param tutorIds The new list of tutor IDs to set.
     */
    public void setTutorIds(List<String> tutorIds) {
        this.tutorIds = tutorIds;
    }
}
