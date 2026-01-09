package com.example.betreuer_app.model;

import java.util.List;

/**
 * Represents a logged-in user in the application.
 * Contains user information and their assigned roles.
 */
public class LoggedInUser {
    private String id;
    private String firstName;
    private String lastName;
    private String email;
    private List<String> roles;

    /**
     * Constructor for a LoggedInUser.
     * @param id The user's ID.
     * @param firstName The user's first name.
     * @param lastName The user's last name.
     * @param email The user's email address.
     * @param roles The list of roles assigned to the user.
     */
    public LoggedInUser(String id, String firstName, String lastName, String email, List<String> roles) {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.roles = roles;
    }

    /**
     * Returns the user's ID.
     * @return The ID.
     */
    public String getId() {
        return id;
    }

    /**
     * Sets the user's ID.
     * @param id The new ID.
     */
    public void setId(String id) {
        this.id = id;
    }

    /**
     * Returns the user's first name.
     * @return The first name.
     */
    public String getFirstName() {
        return firstName;
    }

    /**
     * Sets the user's first name.
     * @param firstName The new first name.
     */
    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    /**
     * Returns the user's last name.
     * @return The last name.
     */
    public String getLastName() {
        return lastName;
    }

    /**
     * Sets the user's last name.
     * @param lastName The new last name.
     */
    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    /**
     * Returns the user's email address.
     * @return The email address.
     */
    public String getEmail() {
        return email;
    }

    /**
     * Sets the user's email address.
     * @param email The new email address.
     */
    public void setEmail(String email) {
        this.email = email;
    }

    /**
     * Returns the list of roles assigned to the user.
     * @return The list of role names.
     */
    public List<String> getRoles() {
        return roles;
    }

    /**
     * Sets the list of roles assigned to the user.
     * @param roles The new list of role names.
     */
    public void setRoles(List<String> roles) {
        this.roles = roles;
    }
}
