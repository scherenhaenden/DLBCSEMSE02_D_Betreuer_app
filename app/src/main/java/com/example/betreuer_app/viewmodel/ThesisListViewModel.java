package com.example.betreuer_app.viewmodel;

import android.app.Application;
import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisListViewModel extends AndroidViewModel {
    private MutableLiveData<ThesesResponse> thesesLiveData;
    private MutableLiveData<String> errorLiveData;
    private ThesisRepository repository;

    public ThesisListViewModel(@NonNull Application application) {
        super(application);
        repository = new ThesisRepository(application);
        thesesLiveData = new MutableLiveData<>();
        errorLiveData = new MutableLiveData<>();
        loadTheses();
    }

    public LiveData<ThesesResponse> getTheses() {
        return thesesLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    private void loadTheses() {
        // For now, we fetch the first page with a default page size.
        repository.getTheses(1, 10, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful()) {
                    thesesLiveData.setValue(response.body());
                } else {
                    errorLiveData.setValue("Failed to load theses. Code: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                errorLiveData.setValue("Failed to load theses: " + t.getMessage());
            }
        });
    }
}
