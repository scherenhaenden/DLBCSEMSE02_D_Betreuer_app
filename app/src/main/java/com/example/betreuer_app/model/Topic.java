package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents a topic for a thesis.
 */
public class Topic {
    
    private UUID id;
    private String title;
    private String description;
    private String subjectArea;
    private boolean isActive;

    /**
     * Constructs a new Topic.
     * @param id The ID of the topic.
     * @param title The title of the topic.
     * @param description The description of the topic.
     * @param subjectArea The subject area of the topic.
     * @param isActive The active status of the topic.
     */
    public Topic(UUID id, String title, String description, String subjectArea, boolean isActive) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.subjectArea = subjectArea;
        this.isActive = isActive;
    }

    /**
     * Returns the ID of the topic.
     * @return The ID.
     */
    public UUID getId() {
        return id;
    }

    /**
     * Sets the ID of the topic.
     * @param id The new ID.
     */
    public void setId(UUID id) {
        this.id = id;
    }

    /**
     * Returns the title of the topic.
     * @return The title.
     */
    public String getTitle() {
        return title;
    }

    /**
     * Sets the title of the topic.
     * @param title The new title.
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * Returns the description of the topic.
     * @return The description.
     */
    public String getDescription() {
        return description;
    }

    /**
     * Sets the description of the topic.
     * @param description The new description.
     */
    public void setDescription(String description) {
        this.description = description;
    }

    /**
     * Returns the subject area of the topic.
     * @return The subject area.
     */
    public String getSubjectArea() {
        return subjectArea;
    }

    /**
     * Sets the subject area of the topic.
     * @param subjectArea The new subject area.
     */
    public void setSubjectArea(String subjectArea) {
        this.subjectArea = subjectArea;
    }

    /**
     * Returns whether the topic is active.
     * @return The active status.
     */
    public boolean isActive() {
        return isActive;
    }

    /**
     * Sets the active status of the topic.
     * @param isActive The new active status.
     */
    public void setActive(boolean isActive) {
        this.isActive = isActive;
    }
}
