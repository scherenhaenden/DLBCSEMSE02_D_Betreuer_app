package com.example.betreuer_app.model;

import java.util.List;

/**
 * Represents a topic for a thesis.
 * Topics are areas of research that theses can be associated with.
 */
public class Topic extends BaseEntityApiModel {
    private String title;
    private String description;
    private boolean isActive;
    private List<UserTopic> userTopics;

    /**
     * Default constructor.
     */
    public Topic() {
    }

    /**
     * Constructor for a new topic.
     * @param title The title of the topic.
     * @param description The description of the topic.
     * @param isActive Whether the topic is active.
     */
    public Topic(String title, String description, boolean isActive) {
        this.title = title;
        this.description = description;
        this.isActive = isActive;
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
     * Returns whether the topic is active.
     * @return True if active, false otherwise.
     */
    public boolean isActive() {
        return isActive;
    }

    /**
     * Sets whether the topic is active.
     * @param active The new active status.
     */
    public void setActive(boolean active) {
        isActive = active;
    }

    /**
     * Returns the list of user topics.
     * @return The list of UserTopic objects.
     */
    public List<UserTopic> getUserTopics() {
        return userTopics;
    }

    /**
     * Sets the list of user topics.
     * @param userTopics The new list of UserTopic objects.
     */
    public void setUserTopics(List<UserTopic> userTopics) {
        this.userTopics = userTopics;
    }
}
