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
import com.example.betreuer_app.model.TopicApiModel;
import com.example.betreuer_app.model.TopicsResponse;
import com.example.betreuer_app.model.TutorsResponse;
import com.example.betreuer_app.repository.TopicRepository;
import com.example.betreuer_app.repository.TutorRepository;
import com.example.betreuer_app.ui.tutorlist.TutorListAdapter;
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import java.util.UUID;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TutorListActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private TutorListAdapter adapter;
    private TutorRepository tutorRepository;
    private TopicRepository topicRepository;
    private EditText searchInput;
    private ChipGroup topicChipGroup;
    private String selectedTopicId = null;

    private static final long SEARCH_DEBOUNCE_DELAY_MS = 300L;
    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private Runnable pendingSearchRunnable;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tutor_list);

        recyclerView = findViewById(R.id.tutorsRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        searchInput = findViewById(R.id.search_input);
        topicChipGroup = findViewById(R.id.topic_chip_group);

        tutorRepository = new TutorRepository(this);
        topicRepository = new TopicRepository(this);

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

    private void loadTopics() {
        topicRepository.getTopics(1, 10, new Callback<TopicsResponse>() {
            @Override
            public void onResponse(Call<TopicsResponse> call, Response<TopicsResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    topicChipGroup.removeAllViews();
                    for (TopicApiModel topic : response.body().getItems()) {
                        addTopicChip(topic);
                    }
                } else {
                    Toast.makeText(TutorListActivity.this, "Failed to load topics", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<TopicsResponse> call, Throwable t) {
                Toast.makeText(TutorListActivity.this, "Failed to load topics: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void addTopicChip(TopicApiModel topic) {
        Chip chip = new Chip(this);
        chip.setText(topic.getTitle());
        chip.setCheckable(true);
        chip.setClickable(true);
        chip.setTag(topic.getId());
        
        chip.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isChecked) {
                selectedTopicId = topic.getId().toString();
            } else {
                selectedTopicId = null;
            }
            loadTutors(searchInput.getText().toString());
        });

        topicChipGroup.addView(chip);
    }

    private void loadTutors(String name) {
        tutorRepository.getTutors(selectedTopicId, null, name, 1, 20, new Callback<TutorsResponse>() {
            @Override
            public void onResponse(Call<TutorsResponse> call, Response<TutorsResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    adapter = new TutorListAdapter(response.body().getItems());
                    recyclerView.setAdapter(adapter);
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
