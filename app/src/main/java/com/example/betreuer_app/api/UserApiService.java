package com.example.betreuer_app.api;

import com.example.betreuer_app.model.UsersResponse;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface UserApiService {
    @GET("users")
    /**
     * Retrieves a list of users based on the specified page and page size.
     */
    Call<UsersResponse> getUsers(@Query("page") int page, @Query("pageSize") int pageSize);
}
