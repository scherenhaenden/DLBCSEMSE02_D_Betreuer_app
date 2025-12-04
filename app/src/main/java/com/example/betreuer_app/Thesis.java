package com.example.betreuer_app;

public class Thesis {
    private int id;
    private String titel;
    private String fachgebiet;
    private String exposePfad;
    private String status;
    private String rechnungsstatus;

    public Thesis(int id, String titel, String fachgebiet, String exposePfad, String status, String rechnungsstatus) {
        this.id = id;
        this.titel = titel;
        this.fachgebiet = fachgebiet;
        this.exposePfad = exposePfad;
        this.status = status;
        this.rechnungsstatus = rechnungsstatus;
    }

    // Getter und Setter
    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTitel() {
        return titel;
    }

    public void setTitel(String titel) {
        this.titel = titel;
    }

    public String getFachgebiet() {
        return fachgebiet;
    }

    public void setFachgebiet(String fachgebiet) {
        this.fachgebiet = fachgebiet;
    }

    public String getExposePfad() {
        return exposePfad;
    }

    public void setExposePfad(String exposePfad) {
        this.exposePfad = exposePfad;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getRechnungsstatus() {
        return rechnungsstatus;
    }

    public void setRechnungsstatus(String rechnungsstatus) {
        this.rechnungsstatus = rechnungsstatus;
    }
}
