package com.example.betreuer_app.model;

public class CreateThesisRequest {
    private String title;
    private String description;
    private String subjectAreaId;

    public CreateThesisRequest(String title, String description, String subjectAreaId) {
        this.title = title;
        this.description = description;
        this.subjectAreaId = subjectAreaId;
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

    public String getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(String subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }
}
