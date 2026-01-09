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
     */
    public String getActionButonText() {
        ThesisApiModel thesis = thesisData.getValue();
        RoleApiModel role = currentUserRole.getValue();

        if (thesis == null || role == null) return "Lädt...";

        String status = thesis.getStatus();

        if ("STUDENT".equals(role.getName())) {
            switch (status) {
                case "IN_DISCUSSION": return "In Bearbeitung setzen";
                case "REGISTERED":    return "Arbeit jetzt abgeben";
                default:            return "Warten auf Betreuer";
            }
        } else {
            switch (status) {
                case "IN_DISCUSSION": return "Anmeldung bestätigen";
                case "REGISTERED":    return "Warten auf Abgabe";
                case "SUBMITTED":     return "Kolloquium bestätigen";
                default:            return "Abgeschlossen";
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

        if ("STUDENT".equals(role.getName())) {
            return "IN_DISCUSSION".equals(status) || "REGISTERED".equals(status);
        } else {
            return "IN_DISCUSSION".equals(status) || "SUBMITTED".equals(status);
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
