package com.example.betreuer_app.model;

import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

import java.util.UUID;

public class UserTest {

    private User user;
    private UUID userId;

    @Before
    public void setUp() {
        userId = UUID.randomUUID();
        user = new User(userId, "John Doe", "john.doe@example.com", Role.STUDENT);
    }

    @Test
    public void testConstructorAndGetters() {
        assertEquals(userId, user.getId());
        assertEquals("John Doe", user.getName());
        assertEquals("john.doe@example.com", user.getEmail());
        assertEquals(Role.STUDENT, user.getRole());
    }

    @Test
    public void testSetId() {
        UUID newId = UUID.randomUUID();
        user.setId(newId);
        assertEquals(newId, user.getId());
    }

    @Test
    public void testSetName() {
        user.setName("Jane Doe");
        assertEquals("Jane Doe", user.getName());
    }

    @Test
    public void testSetEmail() {
        user.setEmail("jane.doe@example.com");
        assertEquals("jane.doe@example.com", user.getEmail());
    }

    @Test
    public void testSetRole() {
        user.setRole(Role.TUTOR);
        assertEquals(Role.TUTOR, user.getRole());
    }
}
