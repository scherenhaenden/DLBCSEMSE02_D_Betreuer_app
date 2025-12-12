package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents a user in the application.
 */
public class User {
    private UUID id;
    private String name;
    private String email;
    private String role;

    /**
     * Constructs a new User object.
     * @param id The user's ID.
     * @param name The user's name.
     * @param email The user's email address.
     * @param role The user's role.
     */
    public User(UUID id, String name, String email, String role) {
        this.id = id;
        this.name = name;
        this.email = email;
        this.role = role;
    }

    /**
     * Returns the user's ID.
     * @return The ID.
     */
    public UUID getId() {
        return id;
    }

    /**
     * Sets the user's ID.
     * @param id The new ID.
     */
    public void setId(UUID id) {
        this.id = id;
    }

    /**
     * Returns the user's name.
     * @return The name.
     */
    public String getName() {
        return name;
    }

    /**
     * Sets the user's name.
     * @param name The new name.
     */
    public void setName(String name) {
        this.name = name;
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
     * Returns the user's role.
     * @return The role.
     */
    public String getRole() {
        return role;
    }

    /**
     * Sets the user's role.
     * @param role The new role.
     */
    public void setRole(String role) {
        this.role = role;
    }
}
