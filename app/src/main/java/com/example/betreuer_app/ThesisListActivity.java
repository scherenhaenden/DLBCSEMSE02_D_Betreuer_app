package com.example.betreuer_app;

import android.os.Bundle;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.ui.thesislist.ThesisListAdapter;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisListActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private ThesisListAdapter adapter;
    private ThesisRepository thesisRepository;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_list);

        recyclerView = findViewById(R.id.thesesRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        thesisRepository = new ThesisRepository(this);
        loadTheses();
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
