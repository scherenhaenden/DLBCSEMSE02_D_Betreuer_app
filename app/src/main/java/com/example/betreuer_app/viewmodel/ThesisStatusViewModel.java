package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;
import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.model.Role;

/**
 * ViewModel zur Verwaltung des Status einer Abschlussarbeit.
 * Beinhaltet die rollenbasierte Logik für Statusänderungen (Student vs. Tutor).
 */
public class ThesisStatusViewModel extends ViewModel {

    public MutableLiveData<Thesis> thesisData = new MutableLiveData<>();
    public MutableLiveData<Role> currentUserRole = new MutableLiveData<>();

    /**
     * Liefert den passenden Text für den Action-Button basierend auf dem aktuellen Status und der Benutzerrolle.
     */
    public String getActionButonText() {
        Thesis thesis = thesisData.getValue();
        Role role = currentUserRole.getValue();

        if (thesis == null || role == null) return "Lädt...";

        Thesis.Status status = thesis.getStatus();

        if (role == Role.STUDENT) {
            switch (status) {
                case IN_DISCUSSION: return "In Bearbeitung setzen";
                case REGISTERED:    return "Arbeit jetzt abgeben";
                default:            return "Warten auf Betreuer";
            }
        } else {
            switch (status) {
                case IN_DISCUSSION: return "Anmeldung bestätigen";
                case REGISTERED:    return "Warten auf Abgabe";
                case SUBMITTED:     return "Kolloquium bestätigen";
                default:            return "Abgeschlossen";
            }
        }
    }

    /**
     * Prüft die Berechtigung zur Statusänderung basierend auf der Rolle.
     */
    public boolean isActionButtonEnabled() {
        Thesis thesis = thesisData.getValue();
        Role role = currentUserRole.getValue();
        if (thesis == null || role == null) return false;

        Thesis.Status status = thesis.getStatus();

        if (role == Role.STUDENT) {
            return status == Thesis.Status.IN_DISCUSSION || status == Thesis.Status.REGISTERED;
        } else {
            return status == Thesis.Status.IN_DISCUSSION || status == Thesis.Status.SUBMITTED;
        }
    }

    /**
     * Ermittelt den nachfolgenden Status für ein Update.
     */
    public Thesis.Status getNextStatus() {
        Thesis thesis = thesisData.getValue();
        if (thesis == null) return null;

        switch (thesis.getStatus()) {
            case IN_DISCUSSION: return Thesis.Status.REGISTERED;
            case REGISTERED:    return Thesis.Status.SUBMITTED;
            case SUBMITTED:     return Thesis.Status.COLLOQUIUM_HELD;
            default:            return thesis.getStatus();
        }
    }
}
