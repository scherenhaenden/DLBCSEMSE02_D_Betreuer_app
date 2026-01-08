package com.example.betreuer_app;

import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.UserResponse;
import com.google.android.material.appbar.MaterialToolbar;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisDetailActivity extends AppCompatActivity {

    private TextView textViewTitle;
    private TextView textViewStatus;

    private TextView textViewSubjectArea;
    private TextView textViewOwner;
    private TextView textViewTutor;
    private TextView textViewSecondSupervisor;

    private ThesisApiService thesisApiService;
    private UserApiService userApiService;
    private SubjectAreaApiService subjectAreaApiService;

    private String thesisId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_detail);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        textViewTitle = findViewById(R.id.textViewTitle);
        textViewStatus = findViewById(R.id.textViewStatus);

        textViewSubjectArea = findViewById(R.id.textViewSubjectArea);
        textViewOwner = findViewById(R.id.textViewOwner);
        textViewTutor = findViewById(R.id.textViewTutor);
        textViewSecondSupervisor = findViewById(R.id.textViewSecondSupervisor);

        thesisApiService = ApiClient.getThesisApiService(this);
        userApiService = ApiClient.getUserApiService(this);
        subjectAreaApiService = ApiClient.getSubjectAreaApiService(this);

        //Rechungsstatus mit Dropdown-Spinner

        Spinner spinner_billingstatus = findViewById(R.id.spinner_billingstatus);
        spinner_billingstatus.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {

            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                String selectedItem = parent.getItemAtPosition(position).toString();


                Toast.makeText(ThesisDetailActivity.this,
                        "Ausgewählt: " + selectedItem,
                        Toast.LENGTH_SHORT).show();
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
                // optional
            }
        });
        List<String> items = new ArrayList<>();
        items.add("NONE");
        items.add("ISSUED");
        items.add("PAID");

        ArrayAdapter<String> billingstatus_adapter = new ArrayAdapter<>(
                this,
                android.R.layout.simple_spinner_item,
                items
        );

        billingstatus_adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner_billingstatus.setAdapter(billingstatus_adapter);

        if (getIntent().hasExtra("THESIS_ID")) {
            thesisId = getIntent().getStringExtra("THESIS_ID");
            loadThesisDetails(thesisId);
        } else {
            Toast.makeText(this, "Thesis ID not provided", Toast.LENGTH_SHORT).show();
            finish();
        }
    }

    private void loadThesisDetails(String id) {
        thesisApiService.getThesis(id).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ThesisApiModel thesis = response.body();
                    displayThesisDetails(thesis);
                    loadAdditionalInfo(thesis);
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to load thesis details", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void displayThesisDetails(ThesisApiModel thesis) {
        textViewTitle.setText(thesis.getTitle());
        textViewStatus.setText(thesis.getStatus());

    }

    private void loadAdditionalInfo(ThesisApiModel thesis) {
        // Load Subject Area
        if (thesis.getTopicId() != null) {
            // Note: If thesis has topicId, usually that topic has a subject area.
            // The ThesisApiModel might not directly give subject area ID, so we might need to rely on what we have.
            // The prompt asks to show SubjectArea. If not directly available in ThesisApiModel, we might skip or try to fetch it if possible.
            // But usually the thesis itself doesn't hold subjectAreaId directly if it comes from a Topic.
            // Let's assume for now we don't have direct access unless we fetch the topic first. 
            // However, the user mentioned "SubjectArea" is there (probably meant in the list or generally available).
            // Let's check ThesisApiModel again. It has topicId.
            // If the model doesn't have subjectAreaId, we can't fetch it directly without fetching the Topic first.
            // For now, I will leave SubjectArea blank or "Loading..." if we can't get it easily.
            // Wait, the user said "Es kann sein dass man nicht alles auf einmal bekommt".
            // Let's focus on Users first as requested.
            textViewSubjectArea.setText("N/A"); // Placeholder until we can fetch topic -> subject area
        } else {
             textViewSubjectArea.setText("N/A");
        }

        // Load Owner (Student)
        if (thesis.getOwnerId() != null) {
            loadUser(thesis.getOwnerId(), textViewOwner);
        } else {
            textViewOwner.setText("Unknown");
        }

        // Load Tutor
        if (thesis.getTutorId() != null) {
            loadUser(thesis.getTutorId(), textViewTutor);
        } else {
            textViewTutor.setText("None");
        }

        // Load Second Supervisor
        if (thesis.getSecondSupervisorId() != null) {
            loadUser(thesis.getSecondSupervisorId(), textViewSecondSupervisor);
        } else {
            textViewSecondSupervisor.setText("None");
        }
    }

    private void loadUser(String userId, TextView targetView) {
        try {
            UUID uuid = UUID.fromString(userId);
            userApiService.getUser(uuid).enqueue(new Callback<UserResponse>() {
                @Override
                public void onResponse(Call<UserResponse> call, Response<UserResponse> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        UserResponse user = response.body();
                        String name = (user.getFirstName() != null ? user.getFirstName() : "") + " " + 
                                      (user.getLastName() != null ? user.getLastName() : "");
                        targetView.setText(name.trim());
                    } else {
                        targetView.setText("Error loading user");
                    }
                }

                @Override
                public void onFailure(Call<UserResponse> call, Throwable t) {
                    targetView.setText("Error");
                }
            });
        } catch (IllegalArgumentException e) {
            targetView.setText("Invalid ID");
        }
    }


}
