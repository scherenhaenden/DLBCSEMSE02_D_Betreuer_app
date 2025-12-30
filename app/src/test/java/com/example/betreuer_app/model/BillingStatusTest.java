package com.example.betreuer_app.model;

import org.junit.Test;

import static org.junit.Assert.assertEquals;

public class BillingStatusTest {

    @Test
    public void testConstructorAndGetters() {
        BillingStatusApiModel status = new BillingStatusApiModel("Keine Rechnung");
        assertEquals("Keine Rechnung", status.getName());
    }

    @Test
    public void testSetName() {
        BillingStatusApiModel status = new BillingStatusApiModel();
        status.setName("Rechnung gestellt");
        assertEquals("Rechnung gestellt", status.getName());
    }
}
