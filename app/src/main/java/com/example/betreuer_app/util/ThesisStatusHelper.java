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
     * Für Studenten: Zeigt "Erstellt" wenn keine Betreuungsanfrage vorliegt,
     * und "In Abstimmung" solange kein Tutor zugewiesen ist.
     */
    public static String getDisplayStatus(Context context, ThesisApiModel thesis, boolean isStudent) {
        if (thesis == null) {
            return "";
        }

        String status = thesis.getStatus();

        if (isStudent) {
            if (status == null || status.isEmpty()) {
                return context.getString(R.string.status_created);
            }
            if ("IN_DISCUSSION".equals(status)) {
                return context.getString(R.string.status_in_coordination);
            }
            if ("REGISTERED".equals(status) && !isStudentRegistrationConfirmed(context, thesis)) {
                return context.getString(R.string.status_in_coordination);
            }
        }

        return translateStatus(context, status);
    }

    /**
     * Übersetzt den englischen Backend-Status in die deutsche Anzeige mit Betreuungsanfrage-Infos.
     */
    public static String getDisplayStatus(Context context, ThesisApiModel thesis, boolean isStudent,
                                          boolean hasSupervisionRequest, boolean isRequestAccepted) {
        if (thesis == null) {
            return "";
        }

        if (isStudent) {
            if (!hasSupervisionRequest) {
                return context.getString(R.string.status_created);
            }
            if ("IN_DISCUSSION".equals(thesis.getStatus())) {
                return context.getString(R.string.status_in_coordination);
            }
            if ("REGISTERED".equals(thesis.getStatus()) && !isStudentRegistrationConfirmed(context, thesis)) {
                return context.getString(R.string.status_in_coordination);
            }
        }

        return translateStatus(context, thesis.getStatus());
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

    public static void markStudentRegistrationConfirmed(Context context, ThesisApiModel thesis) {
        if (context == null || thesis == null || thesis.getId() == null) return;
        context.getSharedPreferences("thesis_status_prefs", Context.MODE_PRIVATE)
                .edit()
                .putBoolean("student_registered_" + thesis.getId(), true)
                .apply();
    }

    public static boolean isStudentRegistrationConfirmed(Context context, ThesisApiModel thesis) {
        if (context == null || thesis == null || thesis.getId() == null) return false;
        String status = thesis.getStatus();
        if ("SUBMITTED".equals(status) || "DEFENDED".equals(status)) {
            return true;
        }
        return context.getSharedPreferences("thesis_status_prefs", Context.MODE_PRIVATE)
                .getBoolean("student_registered_" + thesis.getId(), false);
    }
}
