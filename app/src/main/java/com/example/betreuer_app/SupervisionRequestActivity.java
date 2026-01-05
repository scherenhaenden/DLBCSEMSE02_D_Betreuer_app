package com.example.betreuer_app;

import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;

public class SupervisionRequestActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_supervision_request);

        if (savedInstanceState == null) {
            getSupportFragmentManager().beginTransaction()
                .replace(R.id.container, new SupervisionRequestFragment())
                .commitNow();
        }
    }
}
