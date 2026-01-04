package com.example.betreuer_app.model;

import java.util.List;
import java.util.UUID;

public class UserResponse {
    private UUID id;
    private String firstName;
    private String lastName;
    private String email;
    private List<String> roles;

    public UUID getId() {
        return id;
    }

    public String getFirstName() {
        return firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public String getEmail() {
        return email;
    }

    public List<String> getRoles() {
        return roles;
    }
}
