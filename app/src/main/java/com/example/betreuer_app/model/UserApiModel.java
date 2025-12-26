package com.example.betreuer_app.model;

import java.util.List;

/**
 * API model for User, combining fields from User and UserApi.
 * Used for API interactions.
 */
public class UserApiModel {
    private String id;
    private String firstName;
    private String lastName;
    private String email;
    private String passwordHash;
    private List<String> roles;
    private List<UserTopic> userTopics;

    /**
     * Default constructor.
     */
    public UserApiModel() {
    }

    /**
     * Constructor with basic fields.
     */
    public UserApiModel(String id, String firstName, String lastName, String email, List<String> roles) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.roles = roles;
    }

    /**
     * Full constructor.
     */
    public UserApiModel(String id, String firstName, String lastName, String email, String passwordHash, List<String> roles, List<UserTopic> userTopics) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.passwordHash = passwordHash;
        this.roles = roles;
        this.userTopics = userTopics;
    }

    // Getters and setters
    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getPasswordHash() {
        return passwordHash;
    }

    public void setPasswordHash(String passwordHash) {
        this.passwordHash = passwordHash;
    }

    public List<String> getRoles() {
        return roles;
    }

    public void setRoles(List<String> roles) {
        this.roles = roles;
    }

    public List<UserTopic> getUserTopics() {
        return userTopics;
    }

    public void setUserTopics(List<UserTopic> userTopics) {
        this.userTopics = userTopics;
    }
}
