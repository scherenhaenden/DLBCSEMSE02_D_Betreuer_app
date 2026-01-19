package com.example.betreuer_app.util;

import android.content.Context;

import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisStatusResponse;

import java.util.ArrayList;
import java.util.List;

/**
 * Encapsulates the business logic for displaying thesis status based on user role,
 * thesis state, and supervision request status.
 *
 * This class determines:
 * - Whether to show a spinner or text view for status display
 * - Which status options are available to the user
 * - Whether status changes are allowed
 */
public class ThesisStatusDisplayLogic {

    /**
     * Result of status display logic computation
     */
    public static class DisplayResult {
        private final DisplayMode displayMode;
        private final List<ThesisStatusResponse> availableStatuses;
        private final String currentStatusText;
        private final boolean spinnerEnabled;

        public DisplayResult(DisplayMode displayMode,
                           List<ThesisStatusResponse> availableStatuses,
                           String currentStatusText,
                           boolean spinnerEnabled) {
            this.displayMode = displayMode;
            this.availableStatuses = availableStatuses;
            this.currentStatusText = currentStatusText;
            this.spinnerEnabled = spinnerEnabled;
        }

        /**
         * Returns the current display mode.
         */
        public DisplayMode getDisplayMode() {
            return displayMode;
        }

        /**
         * Returns a list of available thesis statuses.
         */
        public List<ThesisStatusResponse> getAvailableStatuses() {
            return availableStatuses;
        }

        /**
         * Returns the current status text.
         */
        public String getCurrentStatusText() {
            return currentStatusText;
        }

        public boolean isSpinnerEnabled() {
            return spinnerEnabled;
        }
    }

    /**
     * Determines how the status should be displayed
     */
    public enum DisplayMode {
        TEXT_VIEW,  // Show only text, no editing
        SPINNER     // Show spinner for status selection
    }

    /**
     * Compute how to display the thesis status based on user role and thesis state
     *
     * @param context Android context for string resources
     * @param thesis The thesis to display
     * @param isStudent Whether the current user is a student
     * @param isTutor Whether the current user is a tutor
     * @param hasSupervisionRequest Whether student has created a supervision request
     * @param isSupervisionRequestAccepted Whether the supervision request was accepted
     * @return DisplayResult containing display configuration
     */
    public DisplayResult computeStatusDisplay(
            Context context,
            ThesisApiModel thesis,
            boolean isStudent,
            boolean isTutor,
            boolean hasSupervisionRequest,
            boolean isSupervisionRequestAccepted) {

        String currentStatus = thesis.getStatus();
        boolean isStudentRegistrationConfirmed =
            ThesisStatusHelper.isStudentRegistrationConfirmed(context, thesis);

        // Adjust current status display for students with unconfirmed registration
        if (isStudent && "REGISTERED".equals(currentStatus) && !isStudentRegistrationConfirmed) {
            currentStatus = "IN_DISCUSSION";
        }

        boolean hasTutor = thesis.getTutorId() != null;

        if (isStudent) {
            return computeStudentDisplay(context, currentStatus, hasTutor,
                hasSupervisionRequest, isSupervisionRequestAccepted);
        } else if (isTutor) {
            boolean hasSecondSupervisor = thesis.getSecondSupervisorId() != null;
            return computeTutorDisplay(context, currentStatus, hasSecondSupervisor);
        }

        // Default fallback: show text view
        return new DisplayResult(
            DisplayMode.TEXT_VIEW,
            new ArrayList<>(),
            ThesisStatusHelper.translateStatus(context, currentStatus),
            false
        );
    }

    /**
     * Compute display configuration for student users.
     *
     * This method determines the appropriate display settings based on the student's current status,
     * whether they have a tutor, and the status of their supervision request. It handles different
     * scenarios, such as when a supervision request has not been created, when a request is pending
     * tutor assignment, and when the student has a tutor, providing the relevant status transitions
     * for the user interface.
     *
     * @param context the context used to retrieve string resources
     * @param currentStatus the current status of the student's thesis
     * @param hasTutor indicates if the student has been assigned a tutor
     * @param hasSupervisionRequest indicates if the student has created a supervision request
     * @param isSupervisionRequestAccepted indicates if the supervision request has been accepted
     */
    private DisplayResult computeStudentDisplay(
            Context context,
            String currentStatus,
            boolean hasTutor,
            boolean hasSupervisionRequest,
            boolean isSupervisionRequestAccepted) {

        // Student has not created a supervision request yet
        if (!hasSupervisionRequest) {
            return new DisplayResult(
                DisplayMode.TEXT_VIEW,
                new ArrayList<>(),
                context.getString(R.string.status_created),
                false
            );
        }

        // Student has request but no tutor assigned yet
        if (!isSupervisionRequestAccepted || !hasTutor) {
            return new DisplayResult(
                DisplayMode.TEXT_VIEW,
                new ArrayList<>(),
                context.getString(R.string.status_in_coordination),
                false
            );
        }

        // Student has tutor: determine available status transitions
        List<ThesisStatusResponse> availableStatuses = new ArrayList<>();

        if ("IN_DISCUSSION".equals(currentStatus)) {
            availableStatuses.add(new ThesisStatusResponse(
                "IN_DISCUSSION", context.getString(R.string.status_in_coordination)));
            availableStatuses.add(new ThesisStatusResponse(
                "REGISTERED", context.getString(R.string.status_registered)));

            return new DisplayResult(
                DisplayMode.SPINNER,
                availableStatuses,
                null,
                true
            );
        } else if ("REGISTERED".equals(currentStatus)) {
            availableStatuses.add(new ThesisStatusResponse(
                "REGISTERED", context.getString(R.string.status_registered)));
            availableStatuses.add(new ThesisStatusResponse(
                "SUBMITTED", context.getString(R.string.status_submitted)));

            return new DisplayResult(
                DisplayMode.SPINNER,
                availableStatuses,
                null,
                true
            );
        } else {
            // SUBMITTED or DEFENDED: Student cannot change anymore
            return new DisplayResult(
                DisplayMode.TEXT_VIEW,
                new ArrayList<>(),
                ThesisStatusHelper.translateStatus(context, currentStatus),
                false
            );
        }
    }

    /**
     * Computes the display configuration for tutor users based on the current status.
     *
     * CRITICAL FIX: When the thesis is SUBMITTED and has a second supervisor, the tutor
     * can change the status to DEFENDED. However, the spinner MUST include BOTH statuses:
     * - SUBMITTED (current status) - to maintain the current state
     * - DEFENDED (target status) - to allow manual transition
     *
     * This prevents automatic status changes when the spinner is initialized.
     * The status can ONLY be changed by explicit tutor action.
     */
    private DisplayResult computeTutorDisplay(Context context, String currentStatus, boolean hasSecondSupervisor) {
        if ("SUBMITTED".equals(currentStatus) && hasSecondSupervisor) {
            List<ThesisStatusResponse> availableStatuses = new ArrayList<>();
            // CRITICAL: Include current status FIRST to prevent automatic selection of DEFENDED
            availableStatuses.add(new ThesisStatusResponse(
                "SUBMITTED", context.getString(R.string.status_submitted)));
            availableStatuses.add(new ThesisStatusResponse(
                "DEFENDED", context.getString(R.string.status_defended)));

            return new DisplayResult(
                DisplayMode.SPINNER,
                availableStatuses,
                null,
                true
            );
        }

        return new DisplayResult(
            DisplayMode.TEXT_VIEW,
            new ArrayList<>(),
            ThesisStatusHelper.translateStatus(context, currentStatus),
            false
        );
    }

    /**
     * Find the index of a status in the available statuses list
     *
     * @param availableStatuses List of available statuses
     * @param statusName Name of the status to find
     * @return Index of the status, or -1 if not found
     */
    public int findStatusIndex(List<ThesisStatusResponse> availableStatuses, String statusName) {
        if (availableStatuses == null || statusName == null) {
            return -1;
        }

        for (int i = 0; i < availableStatuses.size(); i++) {
            if (availableStatuses.get(i).getName().equals(statusName)) {
                return i;
            }
        }

        return -1;
    }
}
