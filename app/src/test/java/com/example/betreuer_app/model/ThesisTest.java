package com.example.betreuer_app.model;

import org.junit.Before;
import org.junit.Test;

import java.util.UUID;

import static org.junit.Assert.*;

public class ThesisTest {

    private Thesis thesis;
    private UUID thesisId;
    private UUID studentId;
    private UUID supervisorId;
    private UUID secondExaminerId;

    @Before
    public void setUp() {
        thesisId = UUID.randomUUID();
        studentId = UUID.randomUUID();
        supervisorId = UUID.randomUUID();
        secondExaminerId = UUID.randomUUID();

        thesis = new Thesis(
                thesisId,
                "Test Thesis",
                Thesis.Status.IN_DISCUSSION,
                "Informatik",
                studentId,
                supervisorId,
                secondExaminerId,
                "/path/expose.pdf",
                Thesis.BillingStatus.NONE
        );
    }

    @Test
    public void testConstructorAndGetters() {
        assertEquals(thesisId, thesis.getId());
        assertEquals("Test Thesis", thesis.getTitle());
        assertEquals(Thesis.Status.IN_DISCUSSION, thesis.getStatus());
        assertEquals("Informatik", thesis.getFieldOfStudy());
        assertEquals(studentId, thesis.getStudentId());
        assertEquals(supervisorId, thesis.getSupervisorId());
        assertEquals(secondExaminerId, thesis.getSecondExaminerId());
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
        UUID newStudentId = UUID.randomUUID();
        thesis.setStudentId(newStudentId);
        assertEquals(newStudentId, thesis.getStudentId());
    }

    @Test
    public void testSetSupervisorId() {
        UUID newSupervisorId = UUID.randomUUID();
        thesis.setSupervisorId(newSupervisorId);
        assertEquals(newSupervisorId, thesis.getSupervisorId());
    }

    @Test
    public void testSetSecondExaminerId() {
        UUID newSecondExaminerId = UUID.randomUUID();
        thesis.setSecondExaminerId(newSecondExaminerId);
        assertEquals(newSecondExaminerId, thesis.getSecondExaminerId());
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
