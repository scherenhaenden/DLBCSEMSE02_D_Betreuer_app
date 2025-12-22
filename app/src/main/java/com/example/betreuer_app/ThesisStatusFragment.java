package com.example.betreuer_app;

import android.content.res.ColorStateList;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.viewmodel.ThesisStatusViewModel;
import com.google.android.material.button.MaterialButton;

/**
 * Fragment zur Anzeige und Steuerung des Arbeitsstatus.
 * Bindet die UI-Komponenten an das ThesisStatusViewModel an.
 */
public class ThesisStatusFragment extends Fragment {

    private ThesisStatusViewModel viewModel;
    
    private ImageView iconRegistered, iconInProgress, iconSubmitted, iconGraded;
    private TextView titleRegistered, titleInProgress, titleSubmitted, titleGraded;
    private MaterialButton actionButton;

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

        viewModel.thesisData.observe(getViewLifecycleOwner(), this::updateUi);
        
        actionButton.setOnClickListener(v -> {
            Thesis.Status next = viewModel.getNextStatus();
            if (next != null) {
                Thesis current = viewModel.thesisData.getValue();
                if (current != null) {
                    current.setStatus(next);
                    viewModel.thesisData.setValue(current);
                    Toast.makeText(getContext(), "Status aktualisiert auf: " + next.name(), Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    private void updateUi(Thesis thesis) {
        if (thesis == null) return;

        Thesis.Status s = thesis.getStatus();

        actionButton.setText(viewModel.getActionButonText());
        actionButton.setEnabled(viewModel.isActionButtonEnabled());

        boolean isRegistered = (s != Thesis.Status.IN_DISCUSSION);
        setStepVisuals(iconRegistered, titleRegistered, isRegistered, true);

        boolean inProgress = (s == Thesis.Status.REGISTERED || s == Thesis.Status.SUBMITTED || s == Thesis.Status.COLLOQUIUM_HELD);
        setStepVisuals(iconInProgress, titleInProgress, inProgress, s == Thesis.Status.REGISTERED);

        boolean isSubmitted = (s == Thesis.Status.SUBMITTED || s == Thesis.Status.COLLOQUIUM_HELD);
        setStepVisuals(iconSubmitted, titleSubmitted, isSubmitted, s == Thesis.Status.SUBMITTED);

        boolean isGraded = (s == Thesis.Status.COLLOQUIUM_HELD);
        setStepVisuals(iconGraded, titleGraded, isGraded, isGraded);
    }

    private void setStepVisuals(ImageView icon, TextView text, boolean completed, boolean isActive) {
        int color;
        int iconRes;

        if (completed) {
            color = ContextCompat.getColor(requireContext(), R.color.design_default_color_primary);
            iconRes = R.drawable.ic_status_complete;
        } else if (isActive) {
            color = ContextCompat.getColor(requireContext(), R.color.design_default_color_primary);
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
