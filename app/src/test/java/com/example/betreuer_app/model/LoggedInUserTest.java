package com.example.betreuer_app.model;

import org.junit.Test;
import static org.junit.Assert.*;
import java.util.Arrays;
import java.util.List;

public class LoggedInUserTest {

    @Test
    public void constructorAndGetters_work() {
        List<String> roles = Arrays.asList("STUDENT", "USER");
        LoggedInUser u = new LoggedInUser("42", "Bob", "Jones", "bob@example.com", roles);
        assertEquals("42", u.getId());
        assertEquals("Bob", u.getFirstName());
        assertEquals("Jones", u.getLastName());
        assertEquals("bob@example.com", u.getEmail());
        assertEquals(roles, u.getRoles());
    }

    @Test
    public void setters_modifyFields() {
        LoggedInUser u = new LoggedInUser(null, null, null, null, null);
        u.setFirstName("Jane");
        u.setLastName("Doe");
        u.setEmail("jane@example.com");
        assertEquals("Jane", u.getFirstName());
        assertEquals("Doe", u.getLastName());
        assertEquals("jane@example.com", u.getEmail());
    }
}

