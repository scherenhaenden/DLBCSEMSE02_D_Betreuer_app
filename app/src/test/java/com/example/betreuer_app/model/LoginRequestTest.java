package com.example.betreuer_app.model;

import org.junit.Test;
import static org.junit.Assert.*;

public class LoginRequestTest {

    @Test
    public void constructorAndGetters_work() {
        LoginRequest req = new LoginRequest("alice@example.com", "secret");
        assertEquals("alice@example.com", req.getEmail());
        assertEquals("secret", req.getPassword());
    }

    @Test
    public void setters_acceptNullAndEmpty() {
        LoginRequest req = new LoginRequest("", "");
        req.setEmail(null);
        req.setPassword("");
        assertNull(req.getEmail());
        assertEquals("", req.getPassword());
    }
}

