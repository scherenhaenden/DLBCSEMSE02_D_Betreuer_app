package com.example.betreuer_app.model;

import java.util.List;

/**
 * Represents a role in the system.
 * Roles define the permissions of users.
 */
public class Role extends BaseEntityApiModel {
    private String name;
    private List<UserRole> userRoles;

    /**
     * Default constructor.
     */
    public Role() {
    }

    /**
     * Constructor for a new role.
     * @param name The name of the role.
     */
    public Role(String name) {
        this.name = name;
    }

    /**
     * Returns the name of the role.
     * @return The role name.
     */
    public String getName() {
        return name;
    }

    /**
     * Sets the name of the role.
     * @param name The new role name.
     */
    public void setName(String name) {
        this.name = name;
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
}
