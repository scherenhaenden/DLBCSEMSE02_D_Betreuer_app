package com.example.betreuer_app.util;

import android.content.Context;
import android.content.SharedPreferences;

import com.example.betreuer_app.constants.AuthConstants;

/**
 * Centralized session management for authentication and user data.
 * Handles all SharedPreferences operations related to user authentication.
 */
public class SessionManager {
    private static final String TAG = "SessionManager";
    private final SharedPreferences sharedPreferences;
    private final SharedPreferences.Editor editor;

    // Additional keys not in AuthConstants
    private static final String KEY_EMAIL = "email";
    private static final String KEY_IS_LOGGED_IN = "is_logged_in";

    public SessionManager(Context context) {
        this.sharedPreferences = context.getSharedPreferences(AuthConstants.PREFS_NAME, Context.MODE_PRIVATE);
        this.editor = sharedPreferences.edit();
    }

    /**
     * Save user session after successful login
     */
    public void saveUserSession(String token, String userId, String email, String role) {
        editor.putString(AuthConstants.KEY_JWT_TOKEN, token);
        editor.putString(AuthConstants.KEY_USER_ID, userId);
        editor.putString(KEY_EMAIL, email);
        editor.putString(AuthConstants.KEY_USER_ROLE, role);
        editor.putBoolean(KEY_IS_LOGGED_IN, true);
        editor.apply();
    }

    /**
     * Check if user is logged in
     */
    public boolean isLoggedIn() {
        return sharedPreferences.getBoolean(KEY_IS_LOGGED_IN, false);
    }

    /**
     * Get authentication token
     */
    public String getToken() {
        return sharedPreferences.getString(AuthConstants.KEY_JWT_TOKEN, null);
    }

    /**
     * Get current user ID
     */
    public String getUserId() {
        return sharedPreferences.getString(AuthConstants.KEY_USER_ID, null);
    }

    /**
     * Get current user email
     */
    public String getUserEmail() {
        return sharedPreferences.getString(KEY_EMAIL, null);
    }

    /**
     * Get current user role
     */
    public String getUserRole() {
        return sharedPreferences.getString(AuthConstants.KEY_USER_ROLE, null);
    }

    /**
     * Clear all session data (logout)
     */
    public void clearSession() {
        editor.clear();
        editor.apply();
    }

    /**
     * Update specific session data
     */
    public void updateUserRole(String role) {
        editor.putString(AuthConstants.KEY_USER_ROLE, role);
        editor.apply();
    }

    /**
     * Check if user has a specific role
     */
    public boolean hasRole(String role) {
        String userRole = getUserRole();
        return userRole != null && userRole.equalsIgnoreCase(role);
    }

    /**
     * Check if user is a tutor
     */
    public boolean isTutor() {
        return hasRole("Tutor") || hasRole("TUTOR");
    }

    /**
     * Check if user is a student
     */
    public boolean isStudent() {
        return hasRole("Student") || hasRole("STUDENT");
    }
}

