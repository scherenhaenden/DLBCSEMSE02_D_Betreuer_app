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
import com.example.betreuer_app.constants.RequestStatuses;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.example.betreuer_app.model.UserResponse;

import java.util.ArrayList;
import java.util.List;

public class ThesisRequestAdapter extends RecyclerView.Adapter<ThesisRequestAdapter.ViewHolder> {

    private List<ThesisRequestResponse> requests = new ArrayList<>();
    private OnRequestActionClickListener actionListener;
    private OnItemClickListener itemClickListener;
    private String currentUserId;

    public interface OnRequestActionClickListener {
        void onAccept(ThesisRequestResponse request);
        void onReject(ThesisRequestResponse request);
        void onCancel(ThesisRequestResponse request);
        void onDelete(ThesisRequestResponse request);
    }

    public interface OnItemClickListener {
        void onItemClick(ThesisRequestResponse request);
    }

    public void setOnRequestActionClickListener(OnRequestActionClickListener listener) {
        this.actionListener = listener;
    }

    public void setOnItemClickListener(OnItemClickListener listener) {
        this.itemClickListener = listener;
    }

    public void setRequests(List<ThesisRequestResponse> requests) {
        this.requests = requests;
        notifyDataSetChanged();
    }

    public void setCurrentUserId(String userId) {
        this.currentUserId = userId;
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

            itemView.setOnClickListener(v -> {
                int pos = getAdapterPosition();
                if (pos != RecyclerView.NO_POSITION && itemClickListener != null) {
                    itemClickListener.onItemClick(requests.get(pos));
                }
            });
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

            // Check if user is the requester or receiver
            boolean isRequester = currentUserId != null && request.getRequester() != null &&
                                currentUserId.equals(request.getRequester().getId().toString());
            boolean isReceiver = currentUserId != null && request.getReceiver() != null &&
                               currentUserId.equals(request.getReceiver().getId().toString());

            if (RequestStatuses.ACCEPTED.equalsIgnoreCase(statusText) || RequestStatuses.REJECTED.equalsIgnoreCase(statusText)) {
                actionsLayout.setVisibility(View.GONE);
            } else {
                actionsLayout.setVisibility(View.VISIBLE);

                if (isReceiver) {
                    // User is receiver (tutor) - show Accept/Reject
                    btnAccept.setVisibility(View.VISIBLE);
                    btnReject.setVisibility(View.VISIBLE);
                    btnAccept.setText("Annehmen");
                    btnReject.setText("Ablehnen");
                } else if (isRequester) {
                    // User is requester (student) - show only Delete
                    btnAccept.setVisibility(View.GONE);
                    btnReject.setVisibility(View.VISIBLE);
                    btnReject.setText("Löschen");
                } else {
                    // Fallback - hide actions
                    actionsLayout.setVisibility(View.GONE);
                }
            }

            btnAccept.setOnClickListener(v -> {
                if (actionListener != null) actionListener.onAccept(request);
            });

            btnReject.setOnClickListener(v -> {
                if (actionListener != null) {
                    if (isRequester) {
                        actionListener.onDelete(request);
                    } else {
                        actionListener.onReject(request);
                    }
                }
            });
        }
    }
}
