package com.example.betreuer_app.ui.requests;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.example.betreuer_app.model.UserResponse;

import java.util.ArrayList;
import java.util.List;

public class ThesisRequestAdapter extends RecyclerView.Adapter<ThesisRequestAdapter.ViewHolder> {

    private List<ThesisRequestResponse> requests = new ArrayList<>();
    private OnRequestActionClickListener actionListener;

    public interface OnRequestActionClickListener {
        void onAccept(ThesisRequestResponse request);
        void onReject(ThesisRequestResponse request);
    }

    public void setOnRequestActionClickListener(OnRequestActionClickListener listener) {
        this.actionListener = listener;
    }

    public void setRequests(List<ThesisRequestResponse> requests) {
        this.requests = requests;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_thesis_request, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ThesisRequestResponse request = requests.get(position);
        holder.bind(request);
    }

    @Override
    public int getItemCount() {
        return requests.size();
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private TextView title;
        private TextView requesterName;
        private TextView message;
        private TextView status;
        private LinearLayout actionsLayout;
        private Button btnAccept;
        private Button btnReject;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            title = itemView.findViewById(R.id.textViewThesisTitle);
            requesterName = itemView.findViewById(R.id.textViewRequesterName);
            message = itemView.findViewById(R.id.textViewMessage);
            status = itemView.findViewById(R.id.textViewStatus);
            actionsLayout = itemView.findViewById(R.id.layoutActions);
            btnAccept = itemView.findViewById(R.id.buttonAccept);
            btnReject = itemView.findViewById(R.id.buttonReject);
        }

        public void bind(ThesisRequestResponse request) {
            title.setText(request.getThesisTitle() != null ? request.getThesisTitle() : "No Title");
            
            String name = "Unknown";
            if (request.getRequester() != null) {
                name = request.getRequester().getFirstName() + " " + request.getRequester().getLastName();
            }
            requesterName.setText("Requested by: " + name);

            if (request.getMessage() != null && !request.getMessage().isEmpty()) {
                message.setText(request.getMessage());
                message.setVisibility(View.VISIBLE);
            } else {
                message.setVisibility(View.GONE);
            }

            String statusText = request.getStatus() != null ? request.getStatus() : "PENDING";
            status.setText("Status: " + statusText);

            if ("ACCEPTED".equalsIgnoreCase(statusText) || "REJECTED".equalsIgnoreCase(statusText)) {
                actionsLayout.setVisibility(View.GONE);
            } else {
                actionsLayout.setVisibility(View.VISIBLE);
            }

            btnAccept.setOnClickListener(v -> {
                if (actionListener != null) actionListener.onAccept(request);
            });

            btnReject.setOnClickListener(v -> {
                if (actionListener != null) actionListener.onReject(request);
            });
        }
    }
}
