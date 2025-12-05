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
        thesisList.add(new Thesis(1, "Entwicklung einer mobilen App", Thesis.Status.REGISTERED, "Informatik", 1, 1, 1, "/path/expose1.pdf", Thesis.BillingStatus.ISSUED));
        thesisList.add(new Thesis(2, "Analyse von KI-Algorithmen", Thesis.Status.IN_DISCUSSION, "Mathematik", 1, 1, 1, "/path/expose2.pdf", Thesis.BillingStatus.NONE));
        thesisList.add(new Thesis(3, "Umweltstudie zur Nachhaltigkeit", Thesis.Status.SUBMITTED, "Umweltwissenschaften", 1, 1, 1, "/path/expose3.pdf", Thesis.BillingStatus.PAID));

        // RecyclerView einrichten
        RecyclerView recyclerView = findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        ThesisAdapter adapter = new ThesisAdapter(thesisList);
        recyclerView.setAdapter(adapter);
    }
}