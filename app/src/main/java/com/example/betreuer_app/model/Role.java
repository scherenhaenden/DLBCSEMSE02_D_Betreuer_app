package com.example.betreuer_app.model;

public enum Role {
    STUDENT(1, "Student", "Ein Student, der eine Abschlussarbeit schreibt."),
    TUTOR(2, "Tutor", "Ein Betreuer, der Abschlussarbeiten betreut.");

    private final int guiId;
    private final String name;
    private final String description;

    Role(int guiId, String name, String description) {
        this.guiId = guiId;
        this.name = name;
        this.description = description;
    }

    public int getGuiId() {
        return guiId;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }
}
