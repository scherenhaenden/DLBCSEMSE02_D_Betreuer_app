package com.example.betreuer_app.model;

import org.junit.Test;

import static org.junit.Assert.assertEquals;

public class BillingStatusTest {

    @Test
    public void noneStatus_hasCorrectValues() {
        // Arrange
        BillingStatus status = BillingStatus.NONE;

        // Assert
        assertEquals(0, status.getGuiId());
        assertEquals("Keine Rechnung", status.getName());
        assertEquals("Es wurde noch keine Rechnung gestellt.", status.getDescription());
    }

    @Test
    public void issuedStatus_hasCorrectValues() {
        BillingStatus status = BillingStatus.ISSUED;

        assertEquals(1, status.getGuiId());
        assertEquals("Rechnung gestellt", status.getName());
        assertEquals("Die Rechnung wurde ausgestellt.", status.getDescription());
    }

    @Test
    public void paidStatus_hasCorrectValues() {
        BillingStatus status = BillingStatus.PAID;

        assertEquals(2, status.getGuiId());
        assertEquals("Rechnung bezahlt", status.getName());
        assertEquals("Die Rechnung wurde beglichen.", status.getDescription());
    }
}
