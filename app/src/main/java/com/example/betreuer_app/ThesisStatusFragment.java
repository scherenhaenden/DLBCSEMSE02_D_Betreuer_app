package com.example.betreuer_app;

import android.content.res.ColorStateList;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.RoleApiModel;
import com.example.betreuer_app.model.ThesisStatus;
import com.example.betreuer_app.viewmodel.ThesisStatusViewModel;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * Fragment zur Anzeige und Steuerung des Arbeitsstatus.
 * Bindet die UI-Komponenten an das ThesisStatusViewModel an.
 */
public class ThesisStatusFragment extends Fragment {

    private ThesisStatusViewModel viewModel;
    private ThesisApiService thesisApiService;

    private ImageView iconRegistered, iconInProgress, iconSubmitted, iconGraded;
    private TextView titleRegistered, titleInProgress, titleSubmitted, titleGraded;
    private Button actionButton;

    public ThesisStatusFragment() {
        super(R.layout.fragment_thesis_status);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        iconRegistered = view.findViewById(R.id.icon_registered);
        iconInProgress = view.findViewById(R.id.icon_in_progress);
        iconSubmitted = view.findViewById(R.id.icon_submitted);
        iconGraded = view.findViewById(R.id.icon_graded);

        titleRegistered = view.findViewById(R.id.title_registered);
        titleInProgress = view.findViewById(R.id.title_in_progress);
        titleSubmitted = view.findViewById(R.id.title_submitted);
        titleGraded = view.findViewById(R.id.title_graded);

        actionButton = view.findViewById(R.id.action_button);

        viewModel = new ViewModelProvider(this).get(ThesisStatusViewModel.class);
        thesisApiService = ApiClient.getThesisApiService(requireContext());

        viewModel.thesisData.observe(getViewLifecycleOwner(), this::updateUi);
        
        actionButton.setOnClickListener(v -> {
            ThesisStatus next = viewModel.getNextStatus();
            if (next != null) {
                ThesisApiModel current = viewModel.thesisData.getValue();
                if (current != null) {
                    RoleApiModel role = viewModel.currentUserRole.getValue();
                    if (role != null && !isStatusChangeAllowed(current, next.getName(), role)) {
                        return;
                    }
                    updateThesisStatus(current.getId().toString(), next.getName());
                }
            }
        });
    }

    /**
     * Sendet den neuen Status an die API.
     */
    private void updateThesisStatus(String thesisId, String newStatus) {
        actionButton.setEnabled(false); // Deaktiviere Button während API-Call

        ThesisApiService.StatusUpdateRequest request = new ThesisApiService.StatusUpdateRequest(newStatus);

        thesisApiService.updateStatus(thesisId, request).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<ThesisApiModel> call, @NonNull Response<ThesisApiModel> response) {
                actionButton.setEnabled(true);

                if (response.isSuccessful() && response.body() != null) {
                    // Aktualisiere das ViewModel mit der Antwort vom Server
                    viewModel.thesisData.setValue(response.body());
                    Toast.makeText(getContext(), "Status erfolgreich aktualisiert", Toast.LENGTH_SHORT).show();
                } else {
                    String errorMessage = "Fehler beim Aktualisieren des Status";
                    if (response.code() == 403) {
                        errorMessage = "Sie sind nicht berechtigt, den Status zu ändern";
                    }
                    Toast.makeText(getContext(), errorMessage, Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<ThesisApiModel> call, @NonNull Throwable t) {
                actionButton.setEnabled(true);
                Toast.makeText(getContext(), "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
            }
        });
    }

    private void updateUi(ThesisApiModel thesis) {
        if (thesis == null) return;

        String s = thesis.getStatus();

        actionButton.setText(viewModel.getActionButonText());
        actionButton.setEnabled(viewModel.isActionButtonEnabled());

        boolean isRegistered = !s.equals("IN_DISCUSSION");
        setStepVisuals(iconRegistered, titleRegistered, isRegistered, true);

        boolean inProgress = s.equals("REGISTERED") || s.equals("SUBMITTED") || s.equals("DEFENDED");
        setStepVisuals(iconInProgress, titleInProgress, inProgress, s.equals("REGISTERED"));

        boolean isSubmitted = s.equals("SUBMITTED") || s.equals("DEFENDED");
        setStepVisuals(iconSubmitted, titleSubmitted, isSubmitted, s.equals("SUBMITTED"));

        boolean isGraded = s.equals("DEFENDED");
        setStepVisuals(iconGraded, titleGraded, isGraded, isGraded);
    }

    private boolean isStatusChangeAllowed(ThesisApiModel thesis, String targetStatus, RoleApiModel role) {
        if ("STUDENT".equals(role.getName())) {
            return canStudentSetStatus(thesis, targetStatus);
        }
        return canTutorSetStatus(thesis, targetStatus);
    }

    private boolean canStudentSetStatus(ThesisApiModel thesis, String targetStatus) {
        if (thesis == null || targetStatus == null) {
            Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
            return false;
        }

        boolean hasTutor = thesis.getTutorId() != null;
        boolean hasFile = thesis.getDocumentFileName() != null && !thesis.getDocumentFileName().isEmpty();
        String currentStatus = thesis.getStatus();

        if ("REGISTERED".equals(targetStatus)) {
            if ("IN_DISCUSSION".equals(currentStatus) && hasTutor) {
                return true;
            }
        }

        if ("SUBMITTED".equals(targetStatus)) {
            if (!"REGISTERED".equals(currentStatus)) {
                Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
                return false;
            }
            if (!hasFile) {
                Toast.makeText(getContext(), getString(R.string.toast_need_expose), Toast.LENGTH_SHORT).show();
                return false;
            }
            return true;
        }

        Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
        return false;
    }

    private boolean canTutorSetStatus(ThesisApiModel thesis, String targetStatus) {
        if (thesis == null || targetStatus == null) {
            Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
            return false;
        }

        String currentStatus = thesis.getStatus();
        boolean hasSecondSupervisor = thesis.getSecondSupervisorId() != null;

        if ("DEFENDED".equals(targetStatus)) {
            if (!"SUBMITTED".equals(currentStatus)) {
                Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
                return false;
            }
            if (!hasSecondSupervisor) {
                Toast.makeText(getContext(), getString(R.string.toast_need_second_examiner), Toast.LENGTH_SHORT).show();
                return false;
            }
            return true;
        }

        Toast.makeText(getContext(), getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
        return false;
    }

    private void setStepVisuals(ImageView icon, TextView text, boolean completed, boolean isActive) {
        int color;
        int iconRes;

        if (completed) {
            color = ContextCompat.getColor(requireContext(), android.R.color.holo_blue_dark);
            iconRes = R.drawable.ic_status_complete;
        } else if (isActive) {
            color = ContextCompat.getColor(requireContext(), android.R.color.holo_orange_light);
            iconRes = R.drawable.ic_status_current;
        } else {
            color = ContextCompat.getColor(requireContext(), android.R.color.darker_gray);
            iconRes = R.drawable.ic_status_pending;
        }

        icon.setImageResource(iconRes);
        icon.setImageTintList(ColorStateList.valueOf(color));
        text.setTextColor(color);
    }
}
