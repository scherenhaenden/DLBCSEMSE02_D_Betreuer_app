package com.example.betreuer_app.model;

import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.UUID;

public class UserTest {

    private UserApiModel user;
    private UUID userId;

    @Before
    /**
     * Initializes a new user with a random UUID, name, and email.
     */
    public void setUp() {
        userId = UUID.randomUUID();
        user = new UserApiModel(userId.toString(), "John", "Doe", "john.doe@example.com", new ArrayList<>(Arrays.asList("USER")));
        user.setPasswordHash("password");
    }

    @Test
    /**
     * Tests the constructor and getter methods of the user object.
     */
    public void testConstructorAndGetters() {
        assertEquals(userId.toString(), user.getId());
        assertEquals("John", user.getFirstName());
        assertEquals("Doe", user.getLastName());
        assertEquals("john.doe@example.com", user.getEmail());
        assertEquals("password", user.getPasswordHash());
    }

    @Test
    public void testSetId() {
        String newId = UUID.randomUUID().toString();
        user.setId(newId);
        assertEquals(newId, user.getId());
    }

    @Test
    public void testSetFirstName() {
        user.setFirstName("Jane");
        assertEquals("Jane", user.getFirstName());
    }

    @Test
    public void testSetLastName() {
        user.setLastName("Smith");
        assertEquals("Smith", user.getLastName());
    }

    @Test
    public void testSetEmail() {
        user.setEmail("jane.doe@example.com");
        assertEquals("jane.doe@example.com", user.getEmail());
    }

    @Test
    /**
     * Tests setting the user's roles by adding a new role.
     */
    public void testSetUserRoles() {
        UserRoleApiModel userRole = new UserRoleApiModel();
        userRole.setRole(new RoleApiModel("TUTOR"));
        user.getRoles().add("TUTOR");
        assertEquals(2, user.getRoles().size());
        assertEquals("USER", user.getRoles().get(0));
        assertEquals("TUTOR", user.getRoles().get(1));
    }
}
