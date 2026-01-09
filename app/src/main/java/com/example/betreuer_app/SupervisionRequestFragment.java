package com.example.betreuer_app;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.model.CreateThesisRequestRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisRequestResponse;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SupervisionRequestFragment extends Fragment {

    private AutoCompleteTextView thesisTitleInput;
    private ThesisApiService thesisApiService;
    private ThesisRequestApiService thesisRequestApiService;
    private List<ThesisApiModel> thesesList = new ArrayList<>();
    private String tutorId;

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
            tutorId = getActivity().getIntent().getStringExtra("TUTOR_ID");
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

        // Setup AutoCompleteTextView for thesis titles
        thesisTitleInput = view.findViewById(R.id.thesis_title_input);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(getContext(), android.R.layout.simple_dropdown_item_1line, new ArrayList<>());
        thesisTitleInput.setAdapter(adapter);

        // Initialize API services
        thesisApiService = ApiClient.getThesisApiService(getContext());
        thesisRequestApiService = ApiClient.getThesisRequestApiService(getContext());

        // Fetch theses for the logged-in student
        fetchTheses();

        // Setup submit button
        view.findViewById(R.id.send_request_button).setOnClickListener(v -> sendSupervisionRequest());
    }

    private void fetchTheses() {
        thesisApiService.getTheses(1, 100).enqueue(new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    thesesList = response.body().getItems();
                    List<String> thesisTitles = new ArrayList<>();
                    for (ThesisApiModel thesis : thesesList) {
                        thesisTitles.add(thesis.getTitle());
                    }
                    ArrayAdapter<String> adapter = new ArrayAdapter<>(getContext(), android.R.layout.simple_dropdown_item_1line, thesisTitles);
                    thesisTitleInput.setAdapter(adapter);
                } else {
                    Toast.makeText(getContext(), "Fehler beim Laden der Daten", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                Toast.makeText(getContext(), "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void sendSupervisionRequest() {
        // Validate inputs
        String selectedTitle = thesisTitleInput.getText().toString().trim();
        ThesisApiModel selectedThesis = null;
        for (ThesisApiModel thesis : thesesList) {
            if (thesis.getTitle().equals(selectedTitle)) {
                selectedThesis = thesis;
                break;
            }
        }

        if (selectedThesis == null) {
            Toast.makeText(getContext(), "Bitte wählen Sie einen gültigen Titel aus.", Toast.LENGTH_SHORT).show();
            return;
        }

        if (tutorId == null) {
            Toast.makeText(getContext(), "Tutor-ID fehlt.", Toast.LENGTH_SHORT).show();
            return;
        }

        // Create request object
        CreateThesisRequestRequest request = new CreateThesisRequestRequest(
            selectedThesis.getId(),
            java.util.UUID.fromString(tutorId),
            "SUPERVISION",
            null // message can be null
        );

        // Send request
        thesisRequestApiService.createRequest(request).enqueue(new Callback<ThesisRequestResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponse> call, Response<ThesisRequestResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(getContext(), "Anfrage erfolgreich gesendet.", Toast.LENGTH_SHORT).show();
                    if (getActivity() != null) {
                        getActivity().finish();
                    }
                } else {
                    Toast.makeText(getContext(), "Fehler beim Senden der Anfrage", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponse> call, Throwable t) {
                Toast.makeText(getContext(), "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
