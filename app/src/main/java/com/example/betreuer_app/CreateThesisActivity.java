package com.example.betreuer_app;

import android.os.Bundle;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CreateThesisActivity extends AppCompatActivity {

    private TextInputEditText etTitle;
    private TextInputEditText etTopicId;
    private MaterialButton btnCreate;
    private ThesisRepository thesisRepository;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_thesis);

        etTitle = findViewById(R.id.et_thesis_title);
        etTopicId = findViewById(R.id.et_topic_id);
        btnCreate = findViewById(R.id.btn_create_thesis);

        thesisRepository = new ThesisRepository(this);

        btnCreate.setOnClickListener(v -> {
            String title = String.valueOf(etTitle.getText()).trim();
            String topicId = String.valueOf(etTopicId.getText()).trim();

            if (title.isEmpty()) {
                etTitle.setError("Titel ist erforderlich");
                return;
            }

            // Optional: validate topicId format if needed

            createThesis(title, topicId.isEmpty() ? null : topicId);
        });
    }

    private void createThesis(String title, String topicId) {
        thesisRepository.createThesis(title, topicId, new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful()) {
                    Toast.makeText(CreateThesisActivity.this, "Thesis erfolgreich erstellt", Toast.LENGTH_SHORT).show();
                    finish(); // Close activity and go back
                } else {
                    Toast.makeText(CreateThesisActivity.this, "Fehler beim Erstellen: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(CreateThesisActivity.this, "Fehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
