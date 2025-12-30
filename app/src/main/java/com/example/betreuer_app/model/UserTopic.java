package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents the assignment of a user to a topic.
 * This is a junction table for the many-to-many relationship between User and Topic.
 */
public class UserTopic {
    private String userId;
    private UserApiModel user;
    private UUID topicId;
    private Topic topic;

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
     * Returns the topic ID.
     * @return The UUID of the topic.
     */
    public UUID getTopicId() {
        return topicId;
    }

    /**
     * Sets the topic ID.
     * @param topicId The new topic ID.
     */
    public void setTopicId(UUID topicId) {
        this.topicId = topicId;
    }

    /**
     * Returns the Topic object.
     * @return The Topic object.
     */
    public Topic getTopic() {
        return topic;
    }

    /**
     * Sets the Topic object.
     * @param topic The new Topic object.
     */
    public void setTopic(Topic topic) {
        this.topic = topic;
    }
}
