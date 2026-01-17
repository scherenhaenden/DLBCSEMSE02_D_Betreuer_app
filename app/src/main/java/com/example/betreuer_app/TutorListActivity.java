package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.widget.EditText;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.TutorsResponse;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.repository.TutorRepository;
import com.example.betreuer_app.ui.tutorlist.TutorListAdapter;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * TutorListActivity displays a list of tutors with search and filtering capabilities.
 * Users can search for tutors by name and filter by subject area using chips.
 * The activity implements debounced search to avoid excessive API calls during typing.
 * It loads subject areas dynamically to populate filter chips and fetches tutors based on search criteria.
 */
public class TutorListActivity extends AppCompatActivity {

    /** RecyclerView for displaying the list of tutors. */
    private RecyclerView recyclerView;

    /** Adapter for managing the tutor list data in the RecyclerView. */
    private TutorListAdapter adapter;

    /** Repository for handling tutor-related API operations. */
    private TutorRepository tutorRepository;

    /** Repository for handling subject area-related API operations. */
    private SubjectAreaRepository subjectAreaRepository;

    /** EditText for user input to search tutors by name. */
    private EditText searchInput;

    /** ChipGroup for displaying subject area filter chips. */
    private ChipGroup subjectAreaChipGroup;

    /** The currently selected subject area ID for filtering tutors. Null if no subject area is selected. */
    private String selectedSubjectAreaId = null;

    /** The pre-selected subject area ID from intent extras, if any. */
    private String preSelectedSubjectAreaId = null;

    /** Delay in milliseconds for debouncing search requests. */
    private static final long SEARCH_DEBOUNCE_DELAY_MS = 300L;

    /** Handler for posting delayed search runnables on the main thread. */
    private final Handler searchHandler = new Handler(Looper.getMainLooper());

    /** Runnable for the pending debounced search operation. */
    private Runnable pendingSearchRunnable;

    /**
     * Called when the activity is starting. This method initializes the UI components,
     * sets up repositories, loads initial data, and configures the debounced search listener.
     * @param savedInstanceState If the activity is being re-initialized after previously being shut down,
     * this Bundle contains the data it most recently supplied in onSaveInstanceState.
     * This value may be null.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tutor_list);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        recyclerView = findViewById(R.id.tutorsRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        searchInput = findViewById(R.id.search_input);
        subjectAreaChipGroup = findViewById(R.id.subject_area_chip_group);

        tutorRepository = new TutorRepository(getApplicationContext());
        subjectAreaRepository = new SubjectAreaRepository(getApplicationContext());

        // Get pre-selected subject area ID from intent extras, if available
        preSelectedSubjectAreaId = getIntent().getStringExtra("SELECTED_SUBJECT_AREA_ID");

        loadSubjectAreas();
        loadTutors(null);

        searchInput.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (pendingSearchRunnable != null) {
                    searchHandler.removeCallbacks(pendingSearchRunnable);
                }
                final String query = s.toString();
                pendingSearchRunnable = () -> loadTutors(query);
                searchHandler.postDelayed(pendingSearchRunnable, SEARCH_DEBOUNCE_DELAY_MS);
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });
    }

    /**
     * Loads the list of subject areas from the API and populates the ChipGroup with filter chips.
     * Each chip represents a subject area and allows users to filter tutors by that subject area.
     * If the API call fails, an error message is displayed to the user.
     */
    private void loadSubjectAreas() {
        subjectAreaRepository.getSubjectAreas(1, 10, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    subjectAreaChipGroup.removeAllViews();
                    var items = response.body().getItems();
                    if (items != null) {
                        for (SubjectAreaResponse subjectArea : items) {
                            addSubjectAreaChip(subjectArea);
                        }
                    }
                } else {
                    Toast.makeText(TutorListActivity.this, "Failed to load subject areas", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                Toast.makeText(TutorListActivity.this, "Failed to load subject areas: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Creates and adds a Chip to the ChipGroup for the given subject area.
     * The chip is checkable and triggers a tutor reload when its checked state changes.
     * @param subjectArea The subject area model containing the title and ID for the chip.
     */
    private void addSubjectAreaChip(SubjectAreaResponse subjectArea) {
        Chip chip = new Chip(this);
        chip.setText(subjectArea.getTitle());
        chip.setCheckable(true);
        chip.setClickable(true);
        if (subjectArea.getId() == null) {
            Toast.makeText(this, "Ungültige Fachbereichs-Daten empfangen: " + subjectArea.getTitle(), Toast.LENGTH_SHORT).show();
        }
        chip.setTag(subjectArea.getId());

        // Pre-select the chip if it matches the pre-selected subject area ID
        if (preSelectedSubjectAreaId != null && preSelectedSubjectAreaId.equals(subjectArea.getId())) {
            chip.setChecked(true);
            selectedSubjectAreaId = subjectArea.getId() != null ? subjectArea.getId().toString() : null;
        }

        chip.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                selectedSubjectAreaId = subjectArea.getId() != null ? subjectArea.getId().toString() : null;
            } else {
                selectedSubjectAreaId = null;
            }
            loadTutors(searchInput.getText().toString());
        });

        subjectAreaChipGroup.addView(chip);
    }

    /**
     * Loads the list of tutors from the API based on the selected subject area and search name.
     * This method fetches tutors using the tutorRepository and updates the RecyclerView with the
     * fetched tutors. If the request fails or no tutors are found, an appropriate error message
     * is displayed. The method also handles navigation to the TutorProfileActivity with relevant
     * tutor details, including the tutor's ID, name, and email.
     *
     * @param name The search query for tutor names. Can be null or empty for no name filter.
     */
    private void loadTutors(String name) {
        tutorRepository.getTutors(selectedSubjectAreaId, null, name, 1, 20, new Callback<TutorsResponse>() {
            @Override
            /**
             * Handles the response from a Tutors API call.
             *
             * This method checks if the response is successful and contains a non-null body.
             * If valid items are present, it initializes a TutorListAdapter with the items and sets up an intent to navigate to the TutorProfileActivity,
             * passing relevant tutor information. If no items are found or the response is unsuccessful, it displays a toast message indicating the issue.
             */
            public void onResponse(Call<TutorsResponse> call, Response<TutorsResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    var items = response.body().getItems();
                    if (items != null) {
                        adapter = new TutorListAdapter(items, tutor -> {
                            // Changed navigation: Go to TutorProfileActivity instead of SupervisionRequestActivity directly
                            Intent intent = new Intent(TutorListActivity.this, TutorProfileActivity.class);
                            intent.putExtra("TUTOR_ID", tutor.getId().toString());
                            intent.putExtra("TUTOR_NAME", (tutor.getFirstName() != null ? tutor.getFirstName() : "") + " " + (tutor.getLastName() != null ? tutor.getLastName() : ""));
                            intent.putExtra("TUTOR_EMAIL", tutor.getEmail());

                            // Leite Intent-Extras für zweiten Supervisor weiter
                            if (getIntent().getBooleanExtra("SELECTING_SECOND_SUPERVISOR", false)) {
                                intent.putExtra("SELECTING_SECOND_SUPERVISOR", true);
                                intent.putExtra("THESIS_ID", getIntent().getStringExtra("THESIS_ID"));
                            }

                            startActivity(intent);
                        });
                        recyclerView.setAdapter(adapter);
                    } else {
                        Toast.makeText(TutorListActivity.this, "No tutors found", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(TutorListActivity.this, "No tutors found or error loading", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<TutorsResponse> call, Throwable t) {
                Toast.makeText(TutorListActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
