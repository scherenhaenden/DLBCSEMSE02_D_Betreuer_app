package com.example.betreuer_app;

import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;
import com.example.betreuer_app.ui.thesislist.ThesisListFragment;

public class ThesisListActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_list_container);

        if (savedInstanceState == null) {
            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.fragment_container, new ThesisListFragment())
                    .commit();
        }
    }
}
