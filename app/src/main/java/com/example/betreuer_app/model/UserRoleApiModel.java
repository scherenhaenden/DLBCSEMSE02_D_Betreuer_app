package com.example.betreuer_app.model;

import java.util.UUID;

/**
 * Represents the assignment of a user to a role.
 * This is a junction table for the many-to-many relationship between User and Role.
 */
public class UserRoleApiModel extends BaseEntityApiModel {
    private String userId;
    private UserApiModel user;
    private UUID roleId;
    private RoleApiModel role;

    /**
     * Returns the user ID.
     * @return The UUID of the user.
     */
    public String getUserId() {
        return userId;
    }

    /**
     * Sets the user ID.
     * @param userId The new user ID.
     */
    public void setUserId(String userId) {
        this.userId = userId;
    }

    /**
     * Returns the User object.
     * @return The User object.
     */
    public UserApiModel getUser() {
        return user;
    }

    /**
     * Sets the User object.
     * @param user The new User object.
     */
    public void setUser(UserApiModel user) {
        this.user = user;
    }

    /**
     * Returns the role ID.
     * @return The UUID of the role.
     */
    public UUID getRoleId() {
        return roleId;
    }

    /**
     * Sets the role ID.
     * @param roleId The new role ID.
     */
    public void setRoleId(UUID roleId) {
        this.roleId = roleId;
    }

    /**
     * Returns the Role object.
     * @return The Role object.
     */
    public RoleApiModel getRole() {
        return role;
    }

    /**
     * Sets the Role object.
     * @param role The new Role object.
     */
    public void setRole(RoleApiModel role) {
        this.role = role;
    }
}
