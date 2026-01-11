package com.example.betreuer_app.repository;

import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.LoginRequest;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.LoggedInUser;

import org.junit.Before;
import org.junit.Test;
import org.mockito.ArgumentCaptor;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;

import java.util.Arrays;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

import static org.junit.Assert.*;
import static org.mockito.Mockito.*;

public class LoginRepositoryTest {

    private UserApiService apiService;
    private Call<LoginResponse> callMock;
    private LoginRepository repository;

    @Before
    public void setUp() {
        MockitoAnnotations.initMocks(this);
        apiService = mock(UserApiService.class);
        callMock = mock(Call.class);
        repository = new LoginRepository(apiService);
    }

    @Test
    public void login_callsApiAndEnqueuesCall() {
        when(apiService.login(any(LoginRequest.class))).thenReturn(callMock);

        Callback<LoginResponse> callback = mock(Callback.class);

        repository.login("me@example.com", "pw", callback);

        ArgumentCaptor<LoginRequest> captor = ArgumentCaptor.forClass(LoginRequest.class);
        verify(apiService).login(captor.capture());
        LoginRequest sent = captor.getValue();
        assertEquals("me@example.com", sent.getEmail());
        assertEquals("pw", sent.getPassword());

        verify(callMock).enqueue(callback);
    }

    @Test
    public void login_whenCallFails_invokesCallbackOnFailure() {
        when(apiService.login(any(LoginRequest.class))).thenReturn(callMock);

        // Capture the callback passed to enqueue and simulate onFailure
        doAnswer(invocation -> {
            Callback<LoginResponse> cb = invocation.getArgument(0);
            cb.onFailure(callMock, new Throwable("network"));
            return null;
        }).when(callMock).enqueue(any());

        Callback<LoginResponse> callback = mock(Callback.class);

        repository.login("x@x.com", "x", callback);

        verify(callback).onFailure(eq(callMock), any(Throwable.class));
    }

    @Test
    public void login_whenCallSucceeds_invokesCallbackOnResponse() {
        when(apiService.login(any(LoginRequest.class))).thenReturn(callMock);

        LoggedInUser user = new LoggedInUser("1", "A", "B", "a@b", Arrays.asList("STUDENT"));
        LoginResponse resp = new LoginResponse("tok", user);
        Response<LoginResponse> response = Response.success(resp);

        doAnswer(invocation -> {
            Callback<LoginResponse> cb = invocation.getArgument(0);
            cb.onResponse(callMock, response);
            return null;
        }).when(callMock).enqueue(any());

        Callback<LoginResponse> callback = mock(Callback.class);

        repository.login("a@b", "pw", callback);

        verify(callback).onResponse(eq(callMock), eq(response));
    }
}
