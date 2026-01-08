package com.example.betreuer_app.model;

public class CreateThesisRequest {
    private String title;
    private String subjectAreaId;

    public CreateThesisRequest(String title, String subjectAreaId) {
        this.title = title;
        this.subjectAreaId = subjectAreaId;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getSubjectAreaId() {
        return subjectAreaId;
    }

    public void setSubjectAreaId(String subjectAreaId) {
        this.subjectAreaId = subjectAreaId;
    }
}
