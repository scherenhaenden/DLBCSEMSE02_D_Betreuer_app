package com.example.betreuer_app.model;

/**
 * Represents a login response containing authentication token and user information.
 * Returned after successful login.
 */
public class LoginResponse {
    private String token;
    private LoggedInUser user;

    /**
     * Constructor for a LoginResponse.
     * @param token The authentication token.
     * @param user The logged-in user information.
     */
    public LoginResponse(String token, LoggedInUser user) {
        this.token = token;
        this.user = user;
    }

    /**
     * Returns the authentication token.
     * @return The token.
     */
    public String getToken() {
        return token;
    }

    /**
     * Sets the authentication token.
     * @param token The new token.
     */
    public void setToken(String token) {
        this.token = token;
    }

    /**
     * Returns the logged-in user information.
     * @return The LoggedInUser object.
     */
    public LoggedInUser getUser() {
        return user;
    }

    /**
     * Sets the logged-in user information.
     * @param user The new LoggedInUser object.
     */
    public void setUser(LoggedInUser user) {
        this.user = user;
    }
}
