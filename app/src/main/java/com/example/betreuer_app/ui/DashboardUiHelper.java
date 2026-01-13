package com.example.betreuer_app.ui;

import android.content.Intent;
import android.view.View;
import android.view.ViewStub;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.R;
import com.example.betreuer_app.StudentCreateThesisActivity;
import com.example.betreuer_app.ThesisListActivity;
import com.example.betreuer_app.ThesisOfferDashboardActivity;
import com.example.betreuer_app.ThesisRequestActivity;
import com.example.betreuer_app.TutorListActivity;
import com.google.android.material.card.MaterialCardView;

/**
 * Helper class to set up dashboard UI components for different user roles
 * Separates UI initialization logic from the Activity
 */
public class DashboardUiHelper {

    private final AppCompatActivity activity;

    // Student UI components
    private TextView studentThesisCountTextView;
    private MaterialCardView studentThesisCard;
    private Button btnCreateNewThesis;
    private Button btnFindTutor;
    private Button btnPendingRequests;

    // Lecturer UI components
    private TextView lecturerThesisCountTextView;
    private TextView lecturerRequestsCountTextView;
    private MaterialCardView lecturerThesisCard;
    private MaterialCardView lecturerRequestsCard;
    private Button btnManageThesisOffers;

    // Dynamic views
    private View studentView;
    private View lecturerView;

    public DashboardUiHelper(AppCompatActivity activity) {
        this.activity = activity;
    }

    /**
     * Set up student dashboard UI
     */
    public void setupStudentDashboard(ViewStub stubStudent, ViewStub stubLecturer) {
        if (stubStudent != null) {
            if (studentView == null) {
                studentView = stubStudent.inflate();
            } else {
                studentView.setVisibility(View.VISIBLE);
            }

            if (stubLecturer != null) stubLecturer.setVisibility(View.GONE);
            if (lecturerView != null) lecturerView.setVisibility(View.GONE);

            initializeStudentComponents();
            setupStudentListeners();
        }
    }

    /**
     * Set up lecturer dashboard UI
     */
    public void setupLecturerDashboard(ViewStub stubStudent, ViewStub stubLecturer) {
        if (stubLecturer != null) {
            if (lecturerView == null) {
                lecturerView = stubLecturer.inflate();
            } else {
                lecturerView.setVisibility(View.VISIBLE);
            }

            if (stubStudent != null) stubStudent.setVisibility(View.GONE);
            if (studentView != null) studentView.setVisibility(View.GONE);

            initializeLecturerComponents();
            setupLecturerListeners();
        }
    }

    /**
     * Initialize student UI components
     */
    private void initializeStudentComponents() {
        studentThesisCountTextView = studentView.findViewById(R.id.studentThesisCountTextView);
        studentThesisCard = studentView.findViewById(R.id.student_thesis_card);
        btnCreateNewThesis = studentView.findViewById(R.id.btn_create_new_thesis);
        btnFindTutor = studentView.findViewById(R.id.btn_find_tutor);
        btnPendingRequests = studentView.findViewById(R.id.btn_pending_requests);
    }

    /**
     * Initialize lecturer UI components
     */
    private void initializeLecturerComponents() {
        lecturerThesisCountTextView = lecturerView.findViewById(R.id.lecturerThesisCountTextView);
        lecturerRequestsCountTextView = lecturerView.findViewById(R.id.lecturerRequestsCountTextView);
        lecturerThesisCard = lecturerView.findViewById(R.id.lecturer_thesis_card);
        lecturerRequestsCard = lecturerView.findViewById(R.id.lecturer_requests_card);
        btnManageThesisOffers = lecturerView.findViewById(R.id.btn_manage_thesis_offers);
    }

    /**
     * Set up student click listeners
     */
    private void setupStudentListeners() {
        studentThesisCard.setOnClickListener(v -> openThesisList());

        btnCreateNewThesis.setOnClickListener(v -> {
            Intent intent = new Intent(activity, StudentCreateThesisActivity.class);
            activity.startActivity(intent);
        });

        btnFindTutor.setOnClickListener(v -> {
            Intent intent = new Intent(activity, TutorListActivity.class);
            activity.startActivity(intent);
        });

        btnPendingRequests.setOnClickListener(v -> {
            Intent intent = new Intent(activity, ThesisRequestActivity.class);
            activity.startActivity(intent);
        });
    }

    /**
     * Set up lecturer click listeners
     */
    private void setupLecturerListeners() {
        lecturerThesisCard.setOnClickListener(v -> openThesisList());

        btnManageThesisOffers.setOnClickListener(v -> {
            Intent intent = new Intent(activity, ThesisOfferDashboardActivity.class);
            activity.startActivity(intent);
        });

        lecturerRequestsCard.setOnClickListener(v -> {
            Intent intent = new Intent(activity, ThesisRequestActivity.class);
            activity.startActivity(intent);
        });
    }

    /**
     * Open thesis list activity
     */
    private void openThesisList() {
        Intent intent = new Intent(activity, ThesisListActivity.class);
        activity.startActivity(intent);
    }

    /**
     * Update student thesis count text
     */
    public void updateStudentThesisCount(int count) {
        if (studentThesisCountTextView != null) {
            String thesisText = (count == 1) ? "Abschlussarbeit" : "Abschlussarbeiten";
            studentThesisCountTextView.setText("Du hast " + count + " " + thesisText + " im System.");
        }
    }

    /**
     * Update lecturer thesis count text
     */
    public void updateLecturerThesisCount(int count) {
        if (lecturerThesisCountTextView != null) {
            String thesisText = (count == 1) ? "Abschlussarbeit" : "Abschlussarbeiten";
            lecturerThesisCountTextView.setText("Du betreust " + count + " " + thesisText + ".");
        }
    }

    /**
     * Update lecturer requests count text
     */
    public void updateLecturerRequestsCount(int count) {
        if (lecturerRequestsCountTextView != null) {
            String requestText = (count == 1) ? "neue Betreuungsanfrage" : "neue Betreuungsanfragen";
            lecturerRequestsCountTextView.setText("Du hast " + count + " " + requestText + ".");
        }
    }

    /**
     * Get student view (for visibility checks)
     */
    public View getStudentView() {
        return studentView;
    }

    /**
     * Get lecturer view (for visibility checks)
     */
    public View getLecturerView() {
        return lecturerView;
    }
}

