package com.example.betreuer_app;

import android.os.Bundle;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.model.Thesis;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class MainActivity extends AppCompatActivity {

    @Override
    /**
     * Initializes the activity and sets up the content view with window insets.
     */
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });

        // Dummy-Daten erstellen
        List<Thesis> thesisList = new ArrayList<>();
        thesisList.add(new Thesis(UUID.randomUUID(), "Entwicklung einer mobilen App", Thesis.Status.REGISTERED, "Informatik", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose1.pdf", Thesis.BillingStatus.ISSUED));
        thesisList.add(new Thesis(UUID.randomUUID(), "Analyse von KI-Algorithmen", Thesis.Status.IN_DISCUSSION, "Mathematik", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose2.pdf", Thesis.BillingStatus.NONE));
        thesisList.add(new Thesis(UUID.randomUUID(), "Umweltstudie zur Nachhaltigkeit", Thesis.Status.SUBMITTED, "Umweltwissenschaften", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose3.pdf", Thesis.BillingStatus.PAID));

        // RecyclerView einrichten
        RecyclerView recyclerView = findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        ThesisAdapter adapter = new ThesisAdapter(thesisList);
        recyclerView.setAdapter(adapter);
    }
}
