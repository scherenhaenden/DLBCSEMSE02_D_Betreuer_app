package com.example.betreuer_app;

import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;

public class DashboardActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dashboard);

        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        View studentView = findViewById(R.id.studentDashboardView);
        View lecturerView = findViewById(R.id.lecturerDashboardView);

        // This is a placeholder logic.
        // In a real app, you would get the user's name and role after login.
        String userName = "Stefan"; // Placeholder
        String userRole = "lecturer"; // Placeholder: "student" or "lecturer"

        welcomeTextView.setText("Hallo " + userName + "!");

        if ("student".equals(userRole)) {
            studentView.setVisibility(View.VISIBLE);
            lecturerView.setVisibility(View.GONE);
        } else if ("lecturer".equals(userRole)) {
            lecturerView.setVisibility(View.VISIBLE);
            studentView.setVisibility(View.GONE);
        }
    }
}
