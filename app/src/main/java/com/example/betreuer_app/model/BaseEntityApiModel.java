package com.example.betreuer_app.model;

import java.util.Date;
import java.util.UUID;

/**
 * Base class for all entities in the system.
 * Provides common fields such as ID, creation and update timestamps.
 */
public abstract class BaseEntityApiModel {
    private UUID id = UUID.randomUUID();
    private Date createdAt = new Date();
    private Date updatedAt = new Date();

    /**
     * Returns the unique ID of the entity.
     * @return The UUID of the entity.
     */
    public UUID getId() {
        return id;
    }

    /**
     * Sets the ID of the entity.
     * @param id The new UUID.
     */
    public void setId(UUID id) {
        this.id = id;
    }

    /**
     * Returns the creation timestamp.
     * @return The creation timestamp.
     */
    public Date getCreatedAt() {
        return createdAt;
    }

    /**
     * Sets the creation timestamp.
     * @param createdAt The new creation timestamp.
     */
    public void setCreatedAt(Date createdAt) {
        this.createdAt = createdAt;
    }

    /**
     * Returns the last update timestamp.
     * @return The update timestamp.
     */
    public Date getUpdatedAt() {
        return updatedAt;
    }

    /**
     * Sets the update timestamp.
     * @param updatedAt The new update timestamp.
     */
    public void setUpdatedAt(Date updatedAt) {
        this.updatedAt = updatedAt;
    }
}
