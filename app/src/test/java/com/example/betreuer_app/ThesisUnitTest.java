package com.example.betreuer_app.model;

import org.junit.Before;
import org.junit.Test;

import static org.junit.Assert.*;

public class ThesisUnitTest {

    private Thesis thesis;

    @Before
    public void setUp() {
        thesis = new Thesis(
                1,
                "Test Thesis",
                Thesis.Status.IN_DISCUSSION,
                "Informatik",
                100,
                200,
                300,
                "/path/expose.pdf",
                Thesis.BillingStatus.NONE
        );
    }

    @Test
    public void testConstructorAndGetters() {
        assertEquals(1, thesis.getId());
        assertEquals("Test Thesis", thesis.getTitle());
        assertEquals(Thesis.Status.IN_DISCUSSION, thesis.getStatus());
        assertEquals("Informatik", thesis.getFieldOfStudy());
        assertEquals(100, thesis.getStudentId());
        assertEquals(200, thesis.getSupervisorId());
        assertEquals(300, thesis.getSecondExaminerId());
        assertEquals("/path/expose.pdf", thesis.getExposePath());
        assertEquals(Thesis.BillingStatus.NONE, thesis.getBillingStatus());
    }

    @Test
    public void testSetTitle() {
        thesis.setTitle("Neue Arbeit");
        assertEquals("Neue Arbeit", thesis.getTitle());
    }

    @Test
    public void testSetStatus() {
        thesis.setStatus(Thesis.Status.SUBMITTED);
        assertEquals(Thesis.Status.SUBMITTED, thesis.getStatus());
    }

    @Test
    public void testSetFieldOfStudy() {
        thesis.setFieldOfStudy("Wirtschaft");
        assertEquals("Wirtschaft", thesis.getFieldOfStudy());
    }

    @Test
    public void testSetStudentId() {
        thesis.setStudentId(555);
        assertEquals(555, thesis.getStudentId());
    }

    @Test
    public void testSetSupervisorId() {
        thesis.setSupervisorId(777);
        assertEquals(777, thesis.getSupervisorId());
    }

    @Test
    public void testSetSecondExaminerId() {
        thesis.setSecondExaminerId(888);
        assertEquals(888, thesis.getSecondExaminerId());
    }

    @Test
    public void testSetExposePath() {
        thesis.setExposePath("/new/path/expose.pdf");
        assertEquals("/new/path/expose.pdf", thesis.getExposePath());
    }

    @Test
    public void testSetBillingStatus() {
        thesis.setBillingStatus(Thesis.BillingStatus.PAID);
        assertEquals(Thesis.BillingStatus.PAID, thesis.getBillingStatus());
    }
}
