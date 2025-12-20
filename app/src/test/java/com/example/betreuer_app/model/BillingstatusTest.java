package com.example.betreuer_app.model;

import org.junit.jupiter.api.Test;

import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;

class BillingstatusTest {

    @Test
    void issueInvoice_changesStatusFromNoneToIssued() {
        // Arrange
        User tutor = new User(
                UUID.randomUUID(),
                "Dr. Meyer",
                "meyer@uni.de",
                Role.TUTOR
        );

        Thesis thesis = new Thesis(
                UUID.randomUUID(),
                "Test Thesis",
                Thesis.Status.COLLOQUIUM_HELD,
                "Computer Science",
                UUID.randomUUID(),
                tutor.getId(),
                UUID.randomUUID(),
                "/expose.pdf",
                BillingStatus.NONE
        );

        // Act
        thesis.issueInvoice(tutor);

        // Assert
        assertEquals(BillingStatus.ISSUED, thesis.getBillingStatus());
    }
}
