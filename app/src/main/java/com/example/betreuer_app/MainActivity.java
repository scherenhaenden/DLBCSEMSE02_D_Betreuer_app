package com.example.betreuer_app;

import android.os.Bundle;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;
import java.util.List;

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
        thesisList.add(new Thesis(1, "Entwicklung einer mobilen App", "Informatik", "/path/expose1.pdf", "ANGEMELDET", "GESTELLT"));
        thesisList.add(new Thesis(2, "Analyse von KI-Algorithmen", "Mathematik", "/path/expose2.pdf", "IN_ABSTIMMUNG", "KEINE"));
        thesisList.add(new Thesis(3, "Umweltstudie zur Nachhaltigkeit", "Umweltwissenschaften", "/path/expose3.pdf", "ABGEGEBEN", "BEGLICHEN"));

        // RecyclerView einrichten
        RecyclerView recyclerView = findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        ThesisAdapter adapter = new ThesisAdapter(thesisList);
        recyclerView.setAdapter(adapter);
    }
}