package com.example.betreuer_app.model;

public class User {
    private int id;
    private String name;
    private String email;
    private String role;

    public User(int id, String name, String email, String role) {
        this.id = id;
        this.name = name;
        this.email = email;
        this.role = role;
    }

    /**
     * Returns the ID.
     */
    public int getId() {
        return id;
    }

    /**
     * Sets the id of the object.
     */
    public void setId(int id) {
        this.id = id;
    }

    /**
     * Returns the name.
     */
    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    /**
     * Returns the email address.
     */
    public String getEmail() {
        return email;
    }

    /**
     * Sets the email address.
     */
    public void setEmail(String email) {
        this.email = email;
    }

    /**
     * Returns the role of the user.
     */
    public String getRole() {
        return role;
    }

    /**
     * Sets the role of the user.
     */
    public void setRole(String role) {
        this.role = role;
    }
}
