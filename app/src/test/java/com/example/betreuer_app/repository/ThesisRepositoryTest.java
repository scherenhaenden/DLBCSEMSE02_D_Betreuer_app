package com.example.betreuer_app.repository;

import android.content.Context;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.MockitoJUnitRunner;

import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.ThesisApiModel;

import retrofit2.Call;
import retrofit2.Callback;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.isNull;
import static org.mockito.Mockito.*;

@RunWith(MockitoJUnitRunner.class)
public class ThesisRepositoryTest {

    @Mock
    private Context context;

    @Mock
    private ThesisApiService apiService;

    @Mock
    private Call<ThesisApiModel> callMock;

    private ThesisRepository repository;

    @Before
    public void setUp() {
        MockitoAnnotations.openMocks(this);
        // Use the new constructor for dependency injection
        repository = new ThesisRepository(context, apiService);
    }

    @Test
    public void createThesis_callsApiWithCorrectParameters() {
        when(apiService.createThesis(any(), any(), any(), any(), any(), any())).thenReturn(callMock);

        Callback<ThesisApiModel> callback = mock(Callback.class);
        repository.createThesis("title", "desc", "topicId", "supervisorId", "coSupervisorId", callback);

        verify(apiService).createThesis(any(RequestBody.class), any(RequestBody.class), any(RequestBody.class), any(RequestBody.class), any(RequestBody.class), isNull());
        verify(callMock).enqueue(callback);
    }
}
