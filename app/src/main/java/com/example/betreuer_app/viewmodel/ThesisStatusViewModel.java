package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;
import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.model.Role;

/**
 * Das Gehirn für unsere Status-Seite! 🧠
 * Hier wird entschieden, wer welchen Status sehen und ändern darf.
 * 
 * Mira-Vibe: Keine Panik vor der Logik, ich hab alles ordentlich beschriftet! ✨
 */
public class ThesisStatusViewModel extends ViewModel {

    // Die aktuellen Daten der Arbeit
    public MutableLiveData<Thesis> thesisData = new MutableLiveData<>();
    
    // Wer schaut gerade drauf? Student oder Tutor?
    public MutableLiveData<Role> currentUserRole = new MutableLiveData<>();

    /**
     * Bestimmt den Text für den großen Button unten.
     * Je nach Rolle und aktuellem Status ändert sich die Aufschrift.
     */
    public String getActionButonText() {
        Thesis thesis = thesisData.getValue();
        Role role = currentUserRole.getValue();

        if (thesis == null || role == null) return "Lädt...";

        Thesis.Status status = thesis.getStatus();

        if (role == Role.STUDENT) {
            // Logik für Studierende 🎓
            switch (status) {
                case IN_DISCUSSION: return "In Bearbeitung setzen";
                case REGISTERED:    return "Arbeit jetzt abgeben";
                default:            return "Warten auf Betreuer";
            }
        } else {
            // Logik für Betreuer (Tutor) 👨‍🏫
            switch (status) {
                case IN_DISCUSSION: return "Anmeldung bestätigen";
                case REGISTERED:    return "Warten auf Abgabe";
                case SUBMITTED:     return "Kolloquium bestätigen";
                default:            return "Abgeschlossen";
            }
        }
    }

    /**
     * Prüft, ob der Button überhaupt klickbar sein sollte.
     * (Wir wollen ja nicht, dass man wild rumklickt, wenn es nichts zu tun gibt)
     */
    public boolean isActionButtonEnabled() {
        Thesis thesis = thesisData.getValue();
        Role role = currentUserRole.getValue();
        if (thesis == null || role == null) return false;

        Thesis.Status status = thesis.getStatus();

        if (role == Role.STUDENT) {
            // Studi darf nur von Abstimmung -> Bearbeitung und von Angemeldet -> Abgegeben
            return status == Thesis.Status.IN_DISCUSSION || status == Thesis.Status.REGISTERED;
        } else {
            // Tutor darf Abstimmung bestätigen oder Kolloquium abschließen
            return status == Thesis.Status.IN_DISCUSSION || status == Thesis.Status.SUBMITTED;
        }
    }

    /**
     * Berechnet den nächsten Status, der an das Backend geschickt werden soll.
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
