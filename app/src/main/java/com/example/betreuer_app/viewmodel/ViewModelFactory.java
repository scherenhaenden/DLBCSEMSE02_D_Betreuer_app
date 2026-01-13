package com.example.betreuer_app.viewmodel;

import androidx.annotation.NonNull;
import androidx.lifecycle.ViewModel;
import androidx.lifecycle.ViewModelProvider;

import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.repository.LoginRepository;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.util.SessionManager;

/**
 * Factory class for creating ViewModels with dependencies
 */
public class ViewModelFactory implements ViewModelProvider.Factory {

    private final ThesisApiService thesisApiService;
    private final SubjectAreaRepository subjectAreaRepository;
    private final LoginRepository loginRepository;
    private final SessionManager sessionManager;
    private final ThesisRepository thesisRepository;
    private final ThesisRequestApiService thesisRequestApiService;

    // Constructor for EditThesisViewModel
    public ViewModelFactory(ThesisApiService thesisApiService, SubjectAreaRepository subjectAreaRepository) {
        this.thesisApiService = thesisApiService;
        this.subjectAreaRepository = subjectAreaRepository;
        this.loginRepository = null;
        this.sessionManager = null;
        this.thesisRepository = null;
        this.thesisRequestApiService = null;
    }

    // Constructor for LoginViewModel
    public ViewModelFactory(LoginRepository loginRepository, SessionManager sessionManager) {
        this.loginRepository = loginRepository;
        this.sessionManager = sessionManager;
        this.thesisApiService = null;
        this.subjectAreaRepository = null;
        this.thesisRepository = null;
        this.thesisRequestApiService = null;
    }

    // Constructor for DashboardViewModel
    public ViewModelFactory(ThesisRepository thesisRepository, ThesisRequestApiService thesisRequestApiService) {
        this.thesisRepository = thesisRepository;
        this.thesisRequestApiService = thesisRequestApiService;
        this.thesisApiService = null;
        this.subjectAreaRepository = null;
        this.loginRepository = null;
        this.sessionManager = null;
    }

    @NonNull
    @Override
    @SuppressWarnings("unchecked")
    public <T extends ViewModel> T create(@NonNull Class<T> modelClass) {
        if (modelClass.isAssignableFrom(EditThesisViewModel.class)) {
            if (thesisApiService == null || subjectAreaRepository == null) {
                throw new IllegalStateException("EditThesisViewModel dependencies not provided");
            }
            return (T) new EditThesisViewModel(thesisApiService, subjectAreaRepository);
        } else if (modelClass.isAssignableFrom(LoginViewModel.class)) {
            if (loginRepository == null || sessionManager == null) {
                throw new IllegalStateException("LoginViewModel dependencies not provided");
            }
            return (T) new LoginViewModel(loginRepository, sessionManager);
        } else if (modelClass.isAssignableFrom(DashboardViewModel.class)) {
            if (thesisRepository == null || thesisRequestApiService == null) {
                throw new IllegalStateException("DashboardViewModel dependencies not provided");
            }
            return (T) new DashboardViewModel(thesisRepository, thesisRequestApiService);
        }
        throw new IllegalArgumentException("Unknown ViewModel class: " + modelClass.getName());
    }
}

