package com.example.betreuer_app.model;

public class CreateThesisRequest {
    private String title;
    private String topicId;

    public CreateThesisRequest(String title, String topicId) {
        this.title = title;
        this.topicId = topicId;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getTopicId() {
        return topicId;
    }

    public void setTopicId(String topicId) {
        this.topicId = topicId;
    }
}
