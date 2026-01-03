package com.example.betreuer_app;

import android.content.Intent;
import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

public class ThesisOfferDashboardActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_offer_dashboard);

        // Setup Floating Action Button to create new offer
        FloatingActionButton fab = findViewById(R.id.fab_add_thesis_offer);
        fab.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisOfferDashboardActivity.this, CreateThesisOfferActivity.class);
            startActivity(intent);
        });
        
        // Setup Toolbar back navigation
        com.google.android.material.appbar.MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());
    }
}
