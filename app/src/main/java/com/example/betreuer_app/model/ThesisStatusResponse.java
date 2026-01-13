package com.example.betreuer_app.model;

import androidx.annotation.NonNull;

/**
 * Response-Model für ThesisStatus vom API.
 * Verwendet für Dropdown-Anzeige.
 */
public class ThesisStatusResponse {
    private String name;
    private String displayName;

    public ThesisStatusResponse(String name, String displayName) {
        this.name = name;
        this.displayName = displayName;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDisplayName() {
        return displayName;
    }

    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    @NonNull
    @Override
    public String toString() {
        return displayName != null ? displayName : name;
    }
}

