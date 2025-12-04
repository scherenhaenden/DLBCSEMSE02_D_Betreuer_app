package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;
import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.repository.ThesisRepository;
import java.util.List;

public class ThesisListViewModel extends ViewModel {
    private MutableLiveData<List<Thesis>> thesesLiveData;
    private ThesisRepository repository;

    public ThesisListViewModel() {
        repository = new ThesisRepository();
        thesesLiveData = new MutableLiveData<>();
        loadTheses();
    }

    public LiveData<List<Thesis>> getTheses() {
        return thesesLiveData;
    }

    private void loadTheses() {
        List<Thesis> theses = repository.getTheses();
        thesesLiveData.setValue(theses);
    }
}
