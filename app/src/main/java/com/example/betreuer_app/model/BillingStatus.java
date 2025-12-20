package com.example.betreuer_app.model;

public enum BillingStatus {
    NONE(0, "Keine Rechnung", "Es wurde noch keine Rechnung gestellt."),
    ISSUED(1, "Rechnung gestellt", "Die Rechnung wurde ausgestellt."),
    PAID(2, "Rechnung bezahlt", "Die Rechnung wurde beglichen.");

    private final int guiId;
    private final String name;
    private final String description;

    BillingStatus(int guiId, String name, String description) {
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
