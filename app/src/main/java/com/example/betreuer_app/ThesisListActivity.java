package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.ui.thesislist.ThesisListAdapter;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisListActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private ThesisListAdapter adapter;
    private ThesisRepository thesisRepository;
    private FloatingActionButton fabAddThesis;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_list);

        recyclerView = findViewById(R.id.thesesRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        fabAddThesis = findViewById(R.id.fab_add_thesis);

        thesisRepository = new ThesisRepository(this);
        
        fabAddThesis.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisListActivity.this, CreateThesisActivity.class);
            startActivity(intent);
        });

        loadTheses();
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadTheses(); // Reload list when returning from CreateThesisActivity
    }

    private void loadTheses() {
        thesisRepository.getTheses(1, 10, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    adapter = new ThesisListAdapter(response.body().getItems());
                    recyclerView.setAdapter(adapter);
                } else {
                    Toast.makeText(ThesisListActivity.this, "Failed to load theses", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                Toast.makeText(ThesisListActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
