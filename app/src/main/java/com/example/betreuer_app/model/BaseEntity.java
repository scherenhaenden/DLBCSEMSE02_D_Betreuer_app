package com.example.betreuer_app.model;

import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.UUID;

/**
 * Base class for all entities in the system.
 * Provides common fields such as ID, creation and update timestamps.
 */
public abstract class BaseEntity {
    private UUID id = UUID.randomUUID();
    private LocalDateTime createdAt = Instant.now().atZone(ZoneId.of("UTC")).toLocalDateTime();
    private LocalDateTime updatedAt = Instant.now().atZone(ZoneId.of("UTC")).toLocalDateTime();

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
     * @return The creation timestamp in UTC.
     */
    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    /**
     * Sets the creation timestamp.
     * @param createdAt The new creation timestamp.
     */
    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    /**
     * Returns the last update timestamp.
     * @return The update timestamp in UTC.
     */
    public LocalDateTime getUpdatedAt() {
        return updatedAt;
    }

    /**
     * Sets the update timestamp.
     * @param updatedAt The new update timestamp.
     */
    public void setUpdatedAt(LocalDateTime updatedAt) {
        this.updatedAt = updatedAt;
    }
}
