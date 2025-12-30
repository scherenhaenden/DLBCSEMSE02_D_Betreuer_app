package com.example.betreuer_app.model;

import java.util.List;
import java.util.UUID;

public class TopicApiModel extends BaseEntityApiModel {
    private String title;
    private String description;
    private String subjectArea;
    private boolean isActive;
    private List<String> tutorIds;

    public TopicApiModel(String id, String title, String description, String subjectArea, boolean isActive, List<String> tutorIds) {
        super();
        if (id != null) {
            setId(UUID.fromString(id));
        }
        this.title = title;
        this.description = description;
        this.subjectArea = subjectArea;
        this.isActive = isActive;
        this.tutorIds = tutorIds;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getSubjectArea() {
        return subjectArea;
    }

    public void setSubjectArea(String subjectArea) {
        this.subjectArea = subjectArea;
    }

    public boolean isActive() {
        return isActive;
    }

    public void setActive(boolean active) {
        isActive = active;
    }

    public List<String> getTutorIds() {
        return tutorIds;
    }

    public void setTutorIds(List<String> tutorIds) {
        this.tutorIds = tutorIds;
    }
}
