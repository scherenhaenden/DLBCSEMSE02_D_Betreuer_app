package com.example.betreuer_app.viewmodel;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.example.betreuer_app.util.Resource;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * ViewModel for DashboardActivity
 * Handles all business logic related to dashboard data loading
 */
public class DashboardViewModel extends ViewModel {

    private final ThesisRepository thesisRepository;
    private final ThesisRequestApiService thesisRequestApiService;

    // LiveData for thesis count
    private final MutableLiveData<Resource<Integer>> thesisCount = new MutableLiveData<>();

    // LiveData for pending requests count
    private final MutableLiveData<Resource<Integer>> pendingRequestsCount = new MutableLiveData<>();

    // LiveData for session expiration
    private final MutableLiveData<Boolean> sessionExpired = new MutableLiveData<>();

    public DashboardViewModel(ThesisRepository thesisRepository, ThesisRequestApiService thesisRequestApiService) {
        this.thesisRepository = thesisRepository;
        this.thesisRequestApiService = thesisRequestApiService;
    }

    // Getters for LiveData
    public LiveData<Resource<Integer>> getThesisCount() {
        return thesisCount;
    }

    public LiveData<Resource<Integer>> getPendingRequestsCount() {
        return pendingRequestsCount;
    }

    public LiveData<Boolean> getSessionExpired() {
        return sessionExpired;
    }

    /**
     * Load thesis count for the current user
     */
    public void loadThesisCount() {
        thesisCount.setValue(Resource.loading(null));

        thesisRepository.getTheses(1, 1, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    int count = response.body().getTotalCount();
                    thesisCount.setValue(Resource.success(count));
                } else if (response.code() == 401) {
                    sessionExpired.setValue(true);
                    thesisCount.setValue(Resource.error("Session expired", null));
                } else {
                    thesisCount.setValue(Resource.error("Failed to load thesis count. Code: " + response.code(), null));
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                thesisCount.setValue(Resource.error("Request failed: " + t.getMessage(), null));
            }
        });
    }

    /**
     * Load pending requests count (for tutors only)
     */
    public void loadPendingRequestsCount() {
        pendingRequestsCount.setValue(Resource.loading(null));

        thesisRequestApiService.getIncomingRequests("Pending", 1, 1).enqueue(new Callback<ThesisRequestResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponsePaginatedResponse> call, Response<ThesisRequestResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    int count = response.body().getTotalCount();
                    pendingRequestsCount.setValue(Resource.success(count));
                } else if (response.code() == 401) {
                    sessionExpired.setValue(true);
                    pendingRequestsCount.setValue(Resource.error("Session expired", null));
                } else {
                    // Silently fail for requests count - not critical
                    pendingRequestsCount.setValue(Resource.success(0));
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                // Silently fail for requests count - not critical
                pendingRequestsCount.setValue(Resource.success(0));
            }
        });
    }

    /**
     * Load all dashboard data based on user role
     */
    public void loadDashboardData(String userRole) {
        loadThesisCount();

        if (userRole != null && userRole.equalsIgnoreCase("tutor")) {
            loadPendingRequestsCount();
        }
    }
}

