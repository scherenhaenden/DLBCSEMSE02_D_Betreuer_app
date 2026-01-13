package com.example.betreuer_app.viewmodel;

import androidx.annotation.NonNull;
import androidx.lifecycle.ViewModel;
import androidx.lifecycle.ViewModelProvider;

import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.repository.LoginRepository;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.util.SessionManager;

/**
 * Factory class for creating ViewModels with dependencies
 */
public class ViewModelFactory implements ViewModelProvider.Factory {

    private ThesisApiService thesisApiService;
    private SubjectAreaRepository subjectAreaRepository;
    private LoginRepository loginRepository;
    private SessionManager sessionManager;

    // Constructor for EditThesisViewModel
    public ViewModelFactory(ThesisApiService thesisApiService, SubjectAreaRepository subjectAreaRepository) {
        this.thesisApiService = thesisApiService;
        this.subjectAreaRepository = subjectAreaRepository;
    }

    // Constructor for LoginViewModel
    public ViewModelFactory(LoginRepository loginRepository, SessionManager sessionManager) {
        this.loginRepository = loginRepository;
        this.sessionManager = sessionManager;
    }

    @NonNull
    @Override
    public <T extends ViewModel> T create(@NonNull Class<T> modelClass) {
        if (modelClass.isAssignableFrom(EditThesisViewModel.class)) {
            return (T) new EditThesisViewModel(thesisApiService, subjectAreaRepository);
        } else if (modelClass.isAssignableFrom(LoginViewModel.class)) {
            return (T) new LoginViewModel(loginRepository, sessionManager);
        }
        throw new IllegalArgumentException("Unknown ViewModel class: " + modelClass.getName());
    }
}

