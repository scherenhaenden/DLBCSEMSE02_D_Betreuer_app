package com.example.betreuer_app.model;

import java.util.List;
import java.util.UUID;

/**
 * Represents a user in the system.
 * Contains personal information and relationships to roles and topics.
 */
public class User extends BaseEntity {
    private String firstName;
    private String lastName;
    private String email;
    private String passwordHash;
    private List<UserRole> userRoles;
    private List<UserTopic> userTopics;

    /**
     * Default constructor.
     */
    public User() {
    }

    /**
     * Constructor for a new user.
     * @param firstName The user's first name.
     * @param lastName The user's last name.
     * @param email The user's email address.
     * @param passwordHash The hash of the password.
     */
    public User(String firstName, String lastName, String email, String passwordHash) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.passwordHash = passwordHash;
    }

    /**
     * Returns the first name.
     * @return The first name.
     */
    public String getFirstName() {
        return firstName;
    }

    /**
     * Sets the first name.
     * @param firstName The new first name.
     */
    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    /**
     * Returns the last name.
     * @return The last name.
     */
    public String getLastName() {
        return lastName;
    }

    /**
     * Sets the last name.
     * @param lastName The new last name.
     */
    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    /**
     * Returns the email address.
     * @return The email address.
     */
    public String getEmail() {
        return email;
    }

    /**
     * Sets the email address.
     * @param email The new email address.
     */
    public void setEmail(String email) {
        this.email = email;
    }

    /**
     * Returns the password hash.
     * @return The password hash.
     */
    public String getPasswordHash() {
        return passwordHash;
    }

    /**
     * Sets the password hash.
     * @param passwordHash The new password hash.
     */
    public void setPasswordHash(String passwordHash) {
        this.passwordHash = passwordHash;
    }

    /**
     * Returns the list of user roles.
     * @return The list of UserRole objects.
     */
    public List<UserRole> getUserRoles() {
        return userRoles;
    }

    /**
     * Sets the list of user roles.
     * @param userRoles The new list of UserRole objects.
     */
    public void setUserRoles(List<UserRole> userRoles) {
        this.userRoles = userRoles;
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
