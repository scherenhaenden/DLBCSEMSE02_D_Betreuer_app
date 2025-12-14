package com.example.betreuer_app;

import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;

public class DashboardActivity extends AppCompatActivity {

    @Override
    /**
     * Initializes the activity and sets the welcome message based on the user's role.
     */
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dashboard);

        TextView welcomeTextView = findViewById(R.id.welcomeTextView);
        TextView studentDashboardTitle = findViewById(R.id.studentDashboardTitle);
        TextView lecturerDashboardTitle = findViewById(R.id.lecturerDashboardTitle);
        View studentView = findViewById(R.id.studentDashboardView);
        View lecturerView = findViewById(R.id.lecturerDashboardView);

        String userName = getIntent().getStringExtra("USER_NAME");
        String userRole = getIntent().getStringExtra("USER_ROLE");

        if (userName != null) {
            welcomeTextView.setText("Hallo " + userName + "!");
        }

        if (userRole != null) {
            if (userRole.equalsIgnoreCase("student")) {
                studentDashboardTitle.setText("Dein Dashboard als (Student)");
                studentView.setVisibility(View.VISIBLE);
                lecturerView.setVisibility(View.GONE);
            } else if (userRole.equalsIgnoreCase("tutor")) {
                lecturerDashboardTitle.setText("Dein Dashboard als (Dozent)");
                lecturerView.setVisibility(View.VISIBLE);
                studentView.setVisibility(View.GONE);
            }
        }
    }
}
