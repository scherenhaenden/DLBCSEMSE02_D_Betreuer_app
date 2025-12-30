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

/**
 * CreateThesisActivity provides a user interface for creating a new thesis.
 * Users can input the thesis title and optionally specify a topic ID.
 * Upon successful creation, the activity displays a success message and closes.
 * If creation fails, an error message is shown.
 */
public class CreateThesisActivity extends AppCompatActivity {

    /** Input field for entering the thesis title. */
    private TextInputEditText etTitle;

    /** Input field for entering the topic ID (optional). */
    private TextInputEditText etTopicId;

    /** Button to trigger the thesis creation process. */
    private MaterialButton btnCreate;

    /** Repository for handling thesis-related API operations. */
    private ThesisRepository thesisRepository;

    /**
     * Called when the activity is starting. This method initializes the UI components,
     * sets up the thesis repository, and configures the button click listener to handle
     * thesis creation with input validation.
     * @param savedInstanceState If the activity is being re-initialized after previously being shut down,
     * this Bundle contains the data it most recently supplied in onSaveInstanceState.
     * This value may be null.
     */
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

    /**
     * Initiates the thesis creation process by calling the API with the provided title and topic ID.
     * Handles the API response asynchronously, showing success or error messages accordingly.
     * @param title The title of the thesis to be created. Must not be null or empty.
     * @param topicId The optional topic ID associated with the thesis. Can be null if not specified.
     */
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
