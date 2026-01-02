package com.example.betreuer_app.model;

import java.util.List;
import java.util.UUID;

public class TutorProfileResponse extends BaseEntityApiModel {
    private String firstName;
    private String lastName;
    private String email;
    private List<TopicApiModel> topics;

    public TutorProfileResponse(String id, String firstName, String lastName, String email, List<TopicApiModel> topics) {
        super();
        if (id != null) {
            setId(UUID.fromString(id));
        }
        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
        this.topics = topics;
    }

    public String getFirstName() {
        return firstName;
    }

    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public List<TopicApiModel> getTopics() {
        return topics;
    }

    public void setTopics(List<TopicApiModel> topics) {
        this.topics = topics;
    }
}
