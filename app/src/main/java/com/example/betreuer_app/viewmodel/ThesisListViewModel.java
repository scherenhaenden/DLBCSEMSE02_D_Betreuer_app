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
    private static final int DEFAULT_PAGE_SIZE = 10;
    private MutableLiveData<ThesesResponse> thesesLiveData;
    private MutableLiveData<String> errorLiveData;
    private MutableLiveData<Boolean> loadingLiveData;
    private MutableLiveData<Integer> currentPageLiveData;
    private MutableLiveData<Integer> totalPagesLiveData;
    private ThesisRepository repository;
    private int pendingPage = 1;
    private int pageSize = DEFAULT_PAGE_SIZE;

    public ThesisListViewModel(@NonNull Application application) {
        super(application);
        repository = new ThesisRepository(application);
        thesesLiveData = new MutableLiveData<>();
        errorLiveData = new MutableLiveData<>();
        loadingLiveData = new MutableLiveData<>(false);
        currentPageLiveData = new MutableLiveData<>(1);
        totalPagesLiveData = new MutableLiveData<>(1);
        loadTheses(1);
    }

    public LiveData<ThesesResponse> getTheses() {
        return thesesLiveData;
    }

    public LiveData<String> getError() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoading() {
        return loadingLiveData;
    }

    public LiveData<Integer> getCurrentPage() {
        return currentPageLiveData;
    }

    public LiveData<Integer> getTotalPages() {
        return totalPagesLiveData;
    }

    public void loadTheses(int page) {
        pendingPage = page;
        loadingLiveData.setValue(true);
        repository.getTheses(page, pageSize, new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                loadingLiveData.setValue(false);
                if (response.isSuccessful()) {
                    ThesesResponse body = response.body();
                    thesesLiveData.setValue(body);
                    if (body != null) {
                        pageSize = body.getPageSize() > 0 ? body.getPageSize() : pageSize;
                        int totalCount = body.getTotalCount();
                        int computedTotalPages = pageSize > 0
                                ? (int) Math.ceil((double) totalCount / pageSize)
                                : 1;
                        if (computedTotalPages < 1) {
                            computedTotalPages = 1;
                        }
                        totalPagesLiveData.setValue(computedTotalPages);
                        currentPageLiveData.setValue(body.getPage() > 0 ? body.getPage() : pendingPage);
                    } else {
                        totalPagesLiveData.setValue(1);
                        currentPageLiveData.setValue(pendingPage);
                    }
                } else {
                    errorLiveData.setValue(getApplication().getString(
                            com.example.betreuer_app.R.string.thesis_list_load_error_with_code,
                            response.code()
                    ));
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                loadingLiveData.setValue(false);
                errorLiveData.setValue(getApplication().getString(
                        com.example.betreuer_app.R.string.thesis_list_load_error_network
                ));
            }
        });
    }
}
