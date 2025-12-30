package com.example.betreuer_app;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

public class SupervisionRequestFragment extends Fragment {

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_supervision_request, container, false);
    }
    
    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        
        // Retrieve tutor data from Activity Intent
        String tutorName = "Unbekannter Tutor";
        if (getActivity() != null && getActivity().getIntent() != null) {
            tutorName = getActivity().getIntent().getStringExtra("TUTOR_NAME");
        }
        
        // Populate the included tutor card view
        TextView nameTextView = view.findViewById(R.id.tutor_name_textview);
        if (nameTextView != null && tutorName != null) {
            nameTextView.setText(tutorName);
        }
        
        // Initialize other views here (e.g. date pickers, dropdowns)
        
        // Setup Toolbar back navigation
        com.google.android.material.appbar.MaterialToolbar toolbar = view.findViewById(R.id.toolbar);
        toolbar.setNavigationIcon(R.drawable.ic_close); // Using close icon or back arrow
        toolbar.setNavigationOnClickListener(v -> {
            if (getActivity() != null) {
                getActivity().finish();
            }
        });
        
        // Re-setup avatar look for the included layout to be consistent
        ImageView avatar = view.findViewById(R.id.tutor_avatar);
        TextView initials = view.findViewById(R.id.tutor_initials);
        
        if (avatar != null && initials != null) {
             avatar.setVisibility(View.GONE);
             initials.setVisibility(View.VISIBLE);
             
             String initialText = "?";
             if (tutorName != null && !tutorName.isEmpty()) {
                 String[] parts = tutorName.split(" ");
                 String first = parts.length > 0 ? parts[0] : "";
                 String last = parts.length > 1 ? parts[parts.length-1] : "";
                 
                 initialText = "";
                 if (!first.isEmpty()) initialText += first.charAt(0);
                 if (!last.isEmpty()) initialText += last.charAt(0);
             }
             initials.setText(initialText);
             
             // Random-like color
             int[] avatarColors = {
                0xFFE57373, 0xFFF06292, 0xFFBA68C8, 0xFF9575CD, 
                0xFF7986CB, 0xFF64B5F6, 0xFF4FC3F7, 0xFF4DD0E1, 
                0xFF4DB6AC, 0xFF81C784, 0xFFAED581, 0xFFFF8A65,
                0xFFA1887F, 0xFF90A4AE
            };
            int colorIndex = Math.abs(tutorName != null ? tutorName.hashCode() : 0) % avatarColors.length;
            
            android.graphics.drawable.GradientDrawable background = new android.graphics.drawable.GradientDrawable();
            background.setShape(android.graphics.drawable.GradientDrawable.OVAL);
            background.setColor(avatarColors[colorIndex]);
            initials.setBackground(background);
        }
    }
}
