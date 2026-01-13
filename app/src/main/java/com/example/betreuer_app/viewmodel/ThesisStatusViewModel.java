package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.betreuer_app.model.RoleApiModel;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisStatus;

/**
 * ViewModel zur Verwaltung des Status einer Abschlussarbeit.
 * Beinhaltet die rollenbasierte Logik für Statusänderungen (Student vs. Tutor).
 */
public class ThesisStatusViewModel extends ViewModel {

    public MutableLiveData<ThesisApiModel> thesisData = new MutableLiveData<>();
    public MutableLiveData<RoleApiModel> currentUserRole = new MutableLiveData<>();

    /**
     * Liefert den passenden Text für den Action-Button basierend auf dem aktuellen Status und der Benutzerrolle.
     * Verwendet deutsche Begriffe für bessere Benutzerfreundlichkeit.
     */
    public String getActionButonText() {
        ThesisApiModel thesis = thesisData.getValue();
        RoleApiModel role = currentUserRole.getValue();

        if (thesis == null || role == null) return "Lädt...";

        String status = thesis.getStatus();
        boolean hasTutor = thesis.getTutorId() != null;

        if ("STUDENT".equals(role.getName())) {
            switch (status) {
                case "IN_DISCUSSION":
                    // Student kann nur anmelden wenn Tutor zugewiesen ist und Anfrage akzeptiert wurde
                    return hasTutor ? "Arbeit anmelden" : "Warte auf Betreuerzusage";
                case "REGISTERED":
                    return "Arbeit jetzt abgeben";
                case "SUBMITTED":
                    return "Warten auf Kolloquium";
                default:
                    return "Warten auf Betreuer";
            }
        } else {
            switch (status) {
                case "IN_DISCUSSION":
                    return "Warten auf Studentenanmeldung";
                case "REGISTERED":
                    return "Warten auf Abgabe";
                case "SUBMITTED":
                    return "Kolloquium bestätigen";
                default:
                    return "Abgeschlossen";
            }
        }
    }

    /**
     * Prüft die Berechtigung zur Statusänderung basierend auf der Rolle.
     */
    public boolean isActionButtonEnabled() {
        ThesisApiModel thesis = thesisData.getValue();
        RoleApiModel role = currentUserRole.getValue();
        if (thesis == null || role == null) return false;

        String status = thesis.getStatus();
        boolean hasTutor = thesis.getTutorId() != null;

        if ("STUDENT".equals(role.getName())) {
            // Student kann nur ändern wenn:
            // 1. Status ist REGISTERED (kann zu SUBMITTED wechseln)
            // 2. Status ist IN_DISCUSSION UND Tutor ist zugewiesen (kann zu REGISTERED wechseln)
            if ("IN_DISCUSSION".equals(status)) {
                return hasTutor; // Nur wenn Tutor zugewiesen ist
            }
            return "REGISTERED".equals(status);
        } else {
            // Tutor kann nur bei SUBMITTED ändern (zu DEFENDED)
            return "SUBMITTED".equals(status);
        }
    }

    /**
     * Ermittelt den nachfolgenden Status für ein Update.
     */
    public ThesisStatus getNextStatus() {
        ThesisApiModel thesis = thesisData.getValue();
        if (thesis == null) return null;

        switch (thesis.getStatus()) {
            case "IN_DISCUSSION": return new ThesisStatus("REGISTERED");
            case "REGISTERED":    return new ThesisStatus("SUBMITTED");
            case "SUBMITTED":     return new ThesisStatus("DEFENDED");
            default:            return new ThesisStatus(thesis.getStatus());
        }
    }
}
