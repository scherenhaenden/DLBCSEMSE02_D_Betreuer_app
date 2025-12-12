package com.example.betreuer_app.model;

public class Topic {
    
    private int id;
    private String title;
    private String description;
    private String subjectArea;
    private boolean isActive;

    public Topic(int id, String title, String description, String subjectArea, boolean isActive) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.subjectArea = subjectArea;
        this.isActive = isActive;
    }

    /*
    Returns the ID.
    */
    public int getId() {
        return id;
    }

    /*
    Sets the ID.
    */
    public void setId(int id) {
        this.id = id;
    }

    /*
    Returns the title.
    */
    public String getTitle() {
        return title;
    }

    /*
    Sets the title
    */
    public void setTitle(String title) {
        this.title = title;
    }

    /*
    Returns the description.
    */
    public String getDescription() {
        return description;
    }

    /*
    Set the description.
    */
    public void setDescription(String description) {
        this.description = description;
    }

    /*
    Returns the subjectArea.
    */
    public String getSubjectArea() {
        return subjectArea;
    }

    /*
    Sets the subjectArea.
    */
    public void setSubjectArea(String subjectArea) {
        this.subjectArea = subjectArea;
    }

    /*
    Returns the isActive.
    */
    public boolean isActive() {
        return isActive;
    }

    /*
    Sets isActive.
    */
    public void setActive(boolean isActive) {
        this.isActive = isActive;
    }
}
