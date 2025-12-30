package com.example.betreuer_app.ui.thesislist;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisApiModel;
import java.util.List;

public class ThesisListAdapter extends RecyclerView.Adapter<ThesisListAdapter.ThesisViewHolder> {

    private List<ThesisApiModel> thesisList;

    public ThesisListAdapter(List<ThesisApiModel> thesisList) {
        this.thesisList = thesisList;
    }

    @NonNull
    @Override
    public ThesisViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_thesis, parent, false);
        return new ThesisViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ThesisViewHolder holder, int position) {
        ThesisApiModel thesis = thesisList.get(position);
        holder.textViewTitel.setText(thesis.getTitle());
        // ThesesResponse does not contain a field for "Fachgebiet", so we hide it or leave it blank.
        holder.textViewFachgebiet.setText(""); // Or set to View.GONE
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
