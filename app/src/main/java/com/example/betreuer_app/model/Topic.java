package com.example.betreuer_app.model;

public class Topic {
    
    private int id;
    private String title;
    private String description;
    private String subjectArea;
    private boolean isActive;

    public topic(int id, String title, String description, String subjectArea, boolean isActive) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.subjectArea = subjectArea;
        this.isActive = isActive;
    }

    /*
    Returns the ID.
    */
    public int getID() {
        return id;
    }

    /*
    Sets the ID.
    */
    public void setID(int id) {
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
        this.title = tile;
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
    public void setSubjectAre(String subjectArea) {
        this.subjectArea = subjectArea;
    }

    /*
    Returns the isActive.
    */
    public boolean getIsActive() {
        return isActive;
    }

    /*
    Sets isActive.
    */
    public void setIsActive(boolean isActive) {
        this.isActive = isActive;
    }
}
