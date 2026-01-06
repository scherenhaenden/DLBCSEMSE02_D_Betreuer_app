package com.example.betreuer_app.ui.tutorlist;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.TutorProfileResponse;
import java.util.List;
import java.util.Random;

public class TutorListAdapter extends RecyclerView.Adapter<TutorListAdapter.TutorViewHolder> {

    private List<TutorProfileResponse> tutorList;
    private OnItemClickListener listener;
    
    // Array of colors for avatar backgrounds
    private final int[] avatarColors = {
        0xFFE57373, 0xFFF06292, 0xFFBA68C8, 0xFF9575CD, 
        0xFF7986CB, 0xFF64B5F6, 0xFF4FC3F7, 0xFF4DD0E1, 
        0xFF4DB6AC, 0xFF81C784, 0xFFAED581, 0xFFFF8A65,
        0xFFA1887F, 0xFF90A4AE
    };

    public interface OnItemClickListener {
        void onItemClick(TutorProfileResponse tutor);
    }

    public TutorListAdapter(List<TutorProfileResponse> tutorList, OnItemClickListener listener) {
        this.tutorList = tutorList;
        this.listener = listener;
    }

    @NonNull
    @Override
    public TutorViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_tutor, parent, false);
        return new TutorViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull TutorViewHolder holder, int position) {
        TutorProfileResponse tutor = tutorList.get(position);
        
        String firstName = tutor.getFirstName() != null ? tutor.getFirstName() : "";
        String lastName = tutor.getLastName() != null ? tutor.getLastName() : "";
        String fullName = firstName + " " + lastName;
        holder.textViewName.setText(fullName.trim());

        StringBuilder specializations = new StringBuilder();
        if (tutor.getSubjectAreas() != null) {
            for (SubjectAreaResponse subjectArea : tutor.getSubjectAreas()) {
                if (specializations.length() > 0) {
                    specializations.append(", ");
                }
                specializations.append(subjectArea.getTitle());
            }
        }
        holder.textViewSpecialization.setText(specializations.toString());
        
        // Hide status indicator for now as it's not in the API response
        holder.statusIndicator.setVisibility(View.GONE);
        
        // Set Initials Avatar
        holder.avatar.setVisibility(View.GONE); // Hide the image view
        holder.initials.setVisibility(View.VISIBLE); // Show the text view
        
        String initials = "";
        if (!firstName.isEmpty()) initials += firstName.charAt(0);
        if (!lastName.isEmpty()) initials += lastName.charAt(0);
        
        if (initials.isEmpty()) initials = "?";
        
        holder.initials.setText(initials.toUpperCase());
        
        // Set random background color based on name hash to be consistent
        int colorIndex = Math.abs(fullName.hashCode()) % avatarColors.length;
        int color = avatarColors[colorIndex];
        
        android.graphics.drawable.GradientDrawable background = new android.graphics.drawable.GradientDrawable();
        background.setShape(android.graphics.drawable.GradientDrawable.OVAL);
        background.setColor(color);
        holder.initials.setBackground(background);
        
        holder.itemView.setOnClickListener(v -> {
            if (listener != null) {
                listener.onItemClick(tutor);
            }
        });
    }

    @Override
    public int getItemCount() {
        return tutorList.size();
    }

    public static class TutorViewHolder extends RecyclerView.ViewHolder {
        TextView textViewName;
        TextView textViewSpecialization;
        ImageView statusIndicator;
        ImageView avatar;
        TextView initials;

        public TutorViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewName = itemView.findViewById(R.id.tutor_name_textview);
            textViewSpecialization = itemView.findViewById(R.id.tutor_specialization_textview);
            statusIndicator = itemView.findViewById(R.id.tutor_status_indicator);
            avatar = itemView.findViewById(R.id.tutor_avatar);
            initials = itemView.findViewById(R.id.tutor_initials);
        }
    }
}
