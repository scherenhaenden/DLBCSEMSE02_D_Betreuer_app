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
    private OnItemClickListener listener;

    public interface OnItemClickListener {
        void onItemClick(ThesisApiModel thesis);
    }

    public void setOnItemClickListener(OnItemClickListener listener) {
        this.listener = listener;
    }

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
        holder.bind(thesis, listener);
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

        public void bind(final ThesisApiModel thesis, final OnItemClickListener listener) {
            textViewTitel.setText(thesis.getTitle());
            textViewFachgebiet.setText(""); 
            textViewStatus.setText("Status: " + thesis.getStatus());
            textViewRechnungsstatus.setText("Rechnung: " + thesis.getBillingStatus());

            itemView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    if (listener != null) {
                        listener.onItemClick(thesis);
                    }
                }
            });
        }
    }
}
