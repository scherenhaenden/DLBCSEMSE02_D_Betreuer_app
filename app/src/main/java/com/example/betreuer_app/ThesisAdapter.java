package com.example.betreuer_app;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.model.ThesisApiModel;

import java.util.List;

public class ThesisAdapter extends RecyclerView.Adapter<ThesisAdapter.ThesisViewHolder> {

    private List<ThesisApiModel> thesisList;

    public ThesisAdapter(List<ThesisApiModel> thesisList) {
        this.thesisList = thesisList;
    }

    @NonNull
    @Override
    /**
     * Creates a new ThesisViewHolder instance.
     */
    public ThesisViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_thesis, parent, false);
        return new ThesisViewHolder(view);
    }

    @Override
    /**
     * Binds the data of a Thesis object to the views in the ThesisViewHolder.
     */
    public void onBindViewHolder(@NonNull ThesisViewHolder holder, int position) {
        ThesisApiModel thesis = thesisList.get(position);
        holder.textViewTitel.setText(thesis.getTitle());
        holder.textViewFachgebiet.setText("Topic ID: " + thesis.getTopicId());
        holder.textViewStatus.setText("Status: " + thesis.getStatus());
        holder.textViewRechnungsstatus.setText("Rechnung: " + thesis.getBillingStatus());
    }

    @Override
    public int getItemCount() {
        return thesisList.size();
    }

    public static class ThesisViewHolder extends RecyclerView.ViewHolder {
        TextView textViewTitel;
        TextView textViewFachgebiet;
        TextView textViewStatus;
        TextView textViewRechnungsstatus;

        public ThesisViewHolder(@NonNull View itemView) {
            super(itemView);
            textViewTitel = itemView.findViewById(R.id.textViewTitel);
            textViewFachgebiet = itemView.findViewById(R.id.textViewFachgebiet);
            textViewStatus = itemView.findViewById(R.id.textViewStatus);
            textViewRechnungsstatus = itemView.findViewById(R.id.textViewRechnungsstatus);
        }
    }
}
