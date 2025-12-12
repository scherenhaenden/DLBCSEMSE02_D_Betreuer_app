package com.example.betreuer_app.model;

import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

import java.util.UUID;

public class TopicTest {

    private Topic topic;
    private UUID topicId;

    @Before
    public void setUp() {
        topicId = UUID.randomUUID();
        topic = new Topic(topicId, "Initial Topic", "Initial Description", "Computer Science", true);
    }

    @Test
    public void testConstructorAndGetters() {
        assertEquals(topicId, topic.getId());
        assertEquals("Initial Topic", topic.getTitle());
        assertEquals("Initial Description", topic.getDescription());
        assertEquals("Computer Science", topic.getSubjectArea());
        assertTrue(topic.isActive());
    }

    @Test
    public void testSetId() {
        UUID newId = UUID.randomUUID();
        topic.setId(newId);
        assertEquals(newId, topic.getId());
    }

    @Test
    public void testSetTitle() {
        topic.setTitle("New Title");
        assertEquals("New Title", topic.getTitle());
    }

    @Test
    public void testSetDescription() {
        topic.setDescription("New Description");
        assertEquals("New Description", topic.getDescription());
    }

    @Test
    public void testSetSubjectArea() {
        topic.setSubjectArea("Data Science");
        assertEquals("Data Science", topic.getSubjectArea());
    }

    @Test
    public void testSetActive() {
        topic.setActive(false);
        assertFalse(topic.isActive());
    }
}
