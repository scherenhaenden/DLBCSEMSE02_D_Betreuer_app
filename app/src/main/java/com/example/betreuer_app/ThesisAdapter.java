package com.example.betreuer_app;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.util.BillingStatusDisplayMapper;
import com.example.betreuer_app.util.SessionManager;
import com.example.betreuer_app.util.ThesisStatusHelper;

import java.util.List;

public class ThesisAdapter extends RecyclerView.Adapter<ThesisAdapter.ThesisViewHolder> {

    private List<ThesisApiModel> thesisList;
    private static final String TAG = "ThesisAdapter";

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
        try {
            ThesisApiModel thesis = thesisList.get(position);

            // Prüfe ob Benutzer Student ist
            SessionManager sessionManager = new SessionManager(holder.itemView.getContext());
            boolean isStudent = !sessionManager.isTutor();

            // Zeige übersetzten Status
            String displayStatus = ThesisStatusHelper.getDisplayStatus(
                holder.itemView.getContext(),
                thesis,
                isStudent
            );

            holder.textViewTitel.setText(thesis.getTitle());
            holder.textViewFachgebiet.setText("Fachgebiet: " + thesis.getSubjectAreaId());
            holder.textViewStatus.setText("Status: " + displayStatus);
            holder.textViewRechnungsstatus.setText("Rechnung: " + BillingStatusDisplayMapper.mapBillingStatusToDisplay(
                holder.itemView.getContext(),
                thesis.getBillingStatus()
            ));
        } catch (Exception e) {
            Log.e(TAG, "Error binding view for position " + position, e);
            // Setzt die Views auf einen Fehlerzustand, um den Benutzer zu informieren
            holder.textViewTitel.setText("Fehler beim Laden der Daten");
            holder.textViewFachgebiet.setText("");
            holder.textViewStatus.setText("");
            holder.textViewRechnungsstatus.setText("");
        }
    }

    @Override
    public int getItemCount() {
        return thesisList != null ? thesisList.size() : 0;
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
