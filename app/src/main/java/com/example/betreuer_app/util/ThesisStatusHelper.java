package com.example.betreuer_app.util;

import android.content.Context;

import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisApiModel;

import java.util.UUID;

/**
 * Hilfsklasse für Thesis-Status-bezogene Operationen.
 */
public class ThesisStatusHelper {

    /**
     * Übersetzt den englischen Backend-Status in die deutsche Anzeige.
     * Für Studenten: Zeigt "Warte auf Betreuerzusage" wenn kein Tutor zugewiesen ist.
     */
    public static String getDisplayStatus(Context context, ThesisApiModel thesis, boolean isStudent) {
        if (thesis == null || thesis.getStatus() == null) {
            return "";
        }

        String status = thesis.getStatus();
        UUID tutorId = thesis.getTutorId();

        // WICHTIG: Student sieht "Warte auf Betreuerzusage", solange kein Tutor zugewiesen ist
        if (isStudent && tutorId == null) {
            return context.getString(R.string.status_waiting_for_tutor);
        }

        return translateStatus(context, status);
    }

    /**
     * Übersetzt den Backend-Status in die deutsche Bezeichnung.
     */
    public static String translateStatus(Context context, String status) {
        if (status == null) return "";

        switch (status) {
            case "IN_DISCUSSION":
                return context.getString(R.string.status_in_discussion);
            case "REGISTERED":
                return context.getString(R.string.status_registered);
            case "SUBMITTED":
                return context.getString(R.string.status_submitted);
            case "DEFENDED":
                return context.getString(R.string.status_defended);
            default:
                return status; // Fallback auf Original-Status
        }
    }

    /**
     * Prüft ob ein Student den Status ändern darf.
     * Student darf NUR ändern wenn:
     * - Status ist REGISTERED (zu SUBMITTED)
     * - ODER Status ist IN_DISCUSSION UND Tutor ist zugewiesen (zu REGISTERED)
     */
    public static boolean canStudentChangeStatus(ThesisApiModel thesis) {
        if (thesis == null) return false;

        String status = thesis.getStatus();
        UUID tutorId = thesis.getTutorId();

        if ("REGISTERED".equals(status)) {
            return true; // Student kann von REGISTERED zu SUBMITTED wechseln
        }

        if ("IN_DISCUSSION".equals(status) && tutorId != null) {
            return true; // Student kann von IN_DISCUSSION zu REGISTERED wechseln, aber nur wenn Tutor zugewiesen
        }

        return false;
    }

    /**
     * Prüft ob ein Tutor den Status ändern darf.
     * Tutor darf ändern wenn Status SUBMITTED ist (zu DEFENDED).
     */
    public static boolean canTutorChangeStatus(ThesisApiModel thesis) {
        if (thesis == null) return false;
        return "SUBMITTED".equals(thesis.getStatus());
    }
}

