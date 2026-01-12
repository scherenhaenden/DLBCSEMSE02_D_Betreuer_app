package com.example.betreuer_app.model;

import org.junit.Test;
import static org.junit.Assert.*;
import java.util.Arrays;

public class LoginResponseTest {

    @Test
    public void constructorAndGetters_work() {
        LoggedInUser user = new LoggedInUser("1", "Alice", "Smith", "alice@example.com", Arrays.asList("STUDENT"));
        LoginResponse resp = new LoginResponse("token123", user);
        assertEquals("token123", resp.getToken());
        assertSame(user, resp.getUser());
    }

    @Test
    public void setters_acceptNull() {
        LoginResponse resp = new LoginResponse(null, null);
        resp.setToken(null);
        resp.setUser(null);
        assertNull(resp.getToken());
        assertNull(resp.getUser());
    }
}

