package com.example.betreuer_app;

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
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import java.util.UUID;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * TutorListActivity displays a list of tutors with search and filtering capabilities.
 * Users can search for tutors by name and filter by topic using chips.
 * The activity implements debounced search to avoid excessive API calls during typing.
 * It loads topics dynamically to populate filter chips and fetches tutors based on search criteria.
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

    /** ChipGroup for displaying topic filter chips. */
    private ChipGroup topicChipGroup;

    /** The currently selected topic ID for filtering tutors. Null if no topic is selected. */
    private String selectedTopicId = null;

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

        recyclerView = findViewById(R.id.tutorsRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        searchInput = findViewById(R.id.search_input);
        topicChipGroup = findViewById(R.id.topic_chip_group);

        tutorRepository = new TutorRepository(getApplicationContext());
        subjectAreaRepository = new SubjectAreaRepository(getApplicationContext());

        loadTopics();
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
     * Loads the list of topics from the API and populates the ChipGroup with filter chips.
     * Each chip represents a topic and allows users to filter tutors by that topic.
     * If the API call fails, an error message is displayed to the user.
     */
    private void loadTopics() {
        subjectAreaRepository.getSubjectAreas(1, 10, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    topicChipGroup.removeAllViews();
                    var items = response.body().getItems();
                    if (items != null) {
                        for (SubjectAreaResponse subjectArea : items) {
                            addTopicChip(subjectArea);
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
    private void addTopicChip(SubjectAreaResponse subjectArea) {
        Chip chip = new Chip(this);
        chip.setText(subjectArea.getTitle());
        chip.setCheckable(true);
        chip.setClickable(true);
        if (subjectArea.getId() == null) {
            Toast.makeText(this, "Ungültige Fachbereichs-Daten empfangen: " + subjectArea.getTitle(), Toast.LENGTH_SHORT).show();
        }
        chip.setTag(subjectArea.getId());

        chip.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                selectedTopicId = subjectArea.getId() != null ? subjectArea.getId().toString() : null;
            } else {
                selectedTopicId = null;
            }
            loadTutors(searchInput.getText().toString());
        });

        topicChipGroup.addView(chip);
    }

    /**
     * Loads the list of tutors from the API based on the selected topic and search name.
     * Updates the RecyclerView with the fetched tutors or displays an error message if the request fails.
     * @param name The search query for tutor names. Can be null or empty for no name filter.
     */
    private void loadTutors(String name) {
        tutorRepository.getTutors(selectedTopicId, null, name, 1, 20, new Callback<TutorsResponse>() {
            @Override
            public void onResponse(Call<TutorsResponse> call, Response<TutorsResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    var items = response.body().getItems();
                    if (items != null) {
                        adapter = new TutorListAdapter(items, tutor -> {
                            // Handle tutor click: Navigate to supervision request
                            SupervisionRequestFragment fragment = new SupervisionRequestFragment();
                            // TODO: Pass tutor data to fragment if needed
                            
                            // For now, just show the fragment in a container or open a new activity
                            // Since this activity seems to be a standalone list, maybe we should start a new activity
                            // But for now let's assume we want to replace the current content or use a container.
                            // However, TutorListActivity uses a specific layout 'activity_tutor_list'.
                            // It might not have a FragmentContainerView.
                            
                            // Let's check layout file activity_tutor_list.xml first to see if we can replace content.
                            // If not, we might need to launch a new activity that hosts the fragment, 
                            // or simple replace the root view if that's acceptable, but usually navigation is better.
                            
                            // Given the prompt "when I click onto one of the tutors ... I have to see something like this",
                            // let's try to launch a new Activity that hosts this fragment.
                            
                            android.content.Intent intent = new android.content.Intent(TutorListActivity.this, SupervisionRequestActivity.class);
                            // We would pass tutor ID here
                            intent.putExtra("TUTOR_ID", tutor.getId().toString());
                             intent.putExtra("TUTOR_NAME", (tutor.getFirstName() != null ? tutor.getFirstName() : "") + " " + (tutor.getLastName() != null ? tutor.getLastName() : ""));
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
