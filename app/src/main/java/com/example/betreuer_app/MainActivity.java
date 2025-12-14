package com.example.betreuer_app;

import android.os.Bundle;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.model.UserApi;
import com.example.betreuer_app.model.UsersResponse;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity {

    @Override
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

        // API Call to test
        TextView apiResponseText = findViewById(R.id.apiResponseText);
        UserApiService apiService = ApiClient.getUserApiService(this);
        Call<UsersResponse> call = apiService.getUsers(1, 10);
        call.enqueue(new Callback<UsersResponse>() {
            @Override
            public void onResponse(Call<UsersResponse> call, Response<UsersResponse> response) {
                if (response.isSuccessful()) {
                    UsersResponse usersResponse = response.body();
                    StringBuilder sb = new StringBuilder();
                    sb.append("Total Count: ").append(usersResponse.getTotalCount()).append("\n");
                    sb.append("Page: ").append(usersResponse.getPage()).append("\n");
                    sb.append("Page Size: ").append(usersResponse.getPageSize()).append("\n");
                    sb.append("Users:\n");
                    for (UserApi user : usersResponse.getItems()) {
                        sb.append("- ID: ").append(user.getId()).append(", Name: ").append(user.getFirstName()).append(" ").append(user.getLastName()).append(", Email: ").append(user.getEmail()).append("\n");
                    }
                    apiResponseText.setText(sb.toString());
                }
            }

            @Override
            public void onFailure(Call<UsersResponse> call, Throwable t) {
                apiResponseText.setText("API call failed: " + t.getMessage());
            }
        });
    }
}
