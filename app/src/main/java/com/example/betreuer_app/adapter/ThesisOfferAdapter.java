package com.example.betreuer_app.adapter;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisOfferApiModel;
import java.util.List;

public class ThesisOfferAdapter extends RecyclerView.Adapter<ThesisOfferAdapter.ViewHolder> {

    private final List<ThesisOfferApiModel> offers;

    public ThesisOfferAdapter(List<ThesisOfferApiModel> offers) {
        this.offers = offers;
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_thesis_offer, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ThesisOfferApiModel offer = offers.get(position);
        holder.tvTitle.setText(offer.getTitle());
        holder.tvDescription.setText(offer.getDescription() != null ? offer.getDescription() : "");
        // holder.chipStatus.setText(offer.getStatus()); // Uncomment if status is available and wanted
    }

    @Override
    public int getItemCount() {
        return offers.size();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvTitle;
        TextView tvDescription;
        // Chip chipStatus;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            tvTitle = itemView.findViewById(R.id.tv_offer_title);
            tvDescription = itemView.findViewById(R.id.tv_offer_description);
            // chipStatus = itemView.findViewById(R.id.chip_status);
        }
    }
}
