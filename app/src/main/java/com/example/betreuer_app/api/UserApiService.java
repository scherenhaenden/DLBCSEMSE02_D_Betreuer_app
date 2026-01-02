package com.example.betreuer_app.api;

import com.example.betreuer_app.model.LoginRequest;
import com.example.betreuer_app.model.LoginResponse;
import com.example.betreuer_app.model.UsersResponse;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Query;

public interface UserApiService {
    @GET("users")
    Call<UsersResponse> getUsers(@Query("page") int page, @Query("pageSize") int pageSize);

    @POST("auth/login")
    Call<LoginResponse> login(@Body LoginRequest request);
}
