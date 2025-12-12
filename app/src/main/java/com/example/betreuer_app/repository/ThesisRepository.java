package com.example.betreuer_app.repository;

import com.example.betreuer_app.model.Thesis;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class ThesisRepository {
    /**
     * Retrieves a list of sample theses.
     */
    public List<Thesis> getTheses() {
        List<Thesis> theses = new ArrayList<>();
        // Fake data
        theses.add(new Thesis(UUID.randomUUID(), "Thesis on AI", Thesis.Status.IN_DISCUSSION, "Computer Science", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose1.pdf", Thesis.BillingStatus.NONE));
        theses.add(new Thesis(UUID.randomUUID(), "Thesis on Biology", Thesis.Status.REGISTERED, "Biology", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose2.pdf", Thesis.BillingStatus.ISSUED));
        theses.add(new Thesis(UUID.randomUUID(), "Thesis on Physics", Thesis.Status.SUBMITTED, "Physics", UUID.randomUUID(), UUID.randomUUID(), UUID.randomUUID(), "/path/expose3.pdf", Thesis.BillingStatus.PAID));
        return theses;
    }
}
