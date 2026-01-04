package com.example.betreuer_app.model;

public class RespondToThesisRequestRequest {
    private boolean accepted;
    private String message;

    public RespondToThesisRequestRequest(boolean accepted, String message) {
        this.accepted = accepted;
        this.message = message;
    }

    public boolean isAccepted() {
        return accepted;
    }

    public void setAccepted(boolean accepted) {
        this.accepted = accepted;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
