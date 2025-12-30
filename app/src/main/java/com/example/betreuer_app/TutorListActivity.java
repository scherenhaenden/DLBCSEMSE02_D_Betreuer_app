package com.example.betreuer_app;

import android.os.Bundle;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.model.TutorsResponse;
import com.example.betreuer_app.repository.TutorRepository;
import com.example.betreuer_app.ui.tutorlist.TutorListAdapter;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class TutorListActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private TutorListAdapter adapter;
    private TutorRepository tutorRepository;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tutor_list);

        recyclerView = findViewById(R.id.tutorsRecyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        tutorRepository = new TutorRepository(this);
        loadTutors();
    }

    private void loadTutors() {
        // Fetch first page of tutors with default page size
        tutorRepository.getTutors(null, null, 1, 20, new Callback<TutorsResponse>() {
            @Override
            public void onResponse(Call<TutorsResponse> call, Response<TutorsResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    adapter = new TutorListAdapter(response.body().getItems());
                    recyclerView.setAdapter(adapter);
                } else {
                    Toast.makeText(TutorListActivity.this, "Failed to load tutors", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<TutorsResponse> call, Throwable t) {
                Toast.makeText(TutorListActivity.this, "Request failed: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
