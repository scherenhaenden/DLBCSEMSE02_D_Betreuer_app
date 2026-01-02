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

public class TutorListAdapter extends RecyclerView.Adapter<TutorListAdapter.TutorViewHolder> {

    private List<TutorProfileResponse> tutorList;

    public TutorListAdapter(List<TutorProfileResponse> tutorList) {
        this.tutorList = tutorList;
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
        
        String fullName = (tutor.getFirstName() != null ? tutor.getFirstName() : "") + " " + 
                          (tutor.getLastName() != null ? tutor.getLastName() : "");
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
        // Avatar is static/sample for now
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

        public TutorViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewName = itemView.findViewById(R.id.tutor_name_textview);
            textViewSpecialization = itemView.findViewById(R.id.tutor_specialization_textview);
            statusIndicator = itemView.findViewById(R.id.tutor_status_indicator);
            avatar = itemView.findViewById(R.id.tutor_avatar);
        }
    }
}
