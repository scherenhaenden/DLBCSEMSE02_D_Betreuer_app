package com.example.betreuer_app.model;

import org.junit.Test;
import static org.junit.Assert.*;

public class UserTest {

    @Test
    public void testConstructor() {
        User user = new User(1, "John", "john@example.com", "student");
        assertEquals(1, user.getId());
        assertEquals("John", user.getName());
        assertEquals("john@example.com", user.getEmail());
        assertEquals("student", user.getRole());
    }

    @Test
    public void testSetters() {
        User user = new User(0, "", "", "");
        user.setId(2);
        user.setName("Jane");
        user.setEmail("jane@example.com");
        user.setRole("teacher");
        assertEquals(2, user.getId());
        assertEquals("Jane", user.getName());
        assertEquals("jane@example.com", user.getEmail());
        assertEquals("teacher", user.getRole());
    }
}
