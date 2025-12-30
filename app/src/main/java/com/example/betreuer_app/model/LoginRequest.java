package com.example.betreuer_app.model;

/**
 * Represents a login request containing user credentials.
 * Used for authentication purposes.
 */
public class LoginRequest {
    private String email;
    private String password;

    /**
     * Constructor for a LoginRequest.
     * @param email The user's email address.
     * @param password The user's password.
     */
    public LoginRequest(String email, String password) {
        this.email = email;
        this.password = password;
    }

    /**
     * Returns the email address.
     * @return The email.
     */
    public String getEmail() {
        return email;
    }

    /**
     * Sets the email address.
     * @param email The new email.
     */
    public void setEmail(String email) {
        this.email = email;
    }

    /**
     * Returns the password.
     * @return The password.
     */
    public String getPassword() {
        return password;
    }

    /**
     * Sets the password.
     * @param password The new password.
     */
    public void setPassword(String password) {
        this.password = password;
    }
}
