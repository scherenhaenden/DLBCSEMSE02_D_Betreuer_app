package com.example.betreuer_app.repository;

import com.example.betreuer_app.model.Thesis;
import java.util.ArrayList;
import java.util.List;

public class ThesisRepository {
    /**
     * Retrieves a list of sample theses.
     */
    public List<Thesis> getTheses() {
        List<Thesis> theses = new ArrayList<>();
        // Fake data
        theses.add(new Thesis(1, "Thesis on AI", Thesis.Status.IN_DISCUSSION, "Computer Science", 101, 201, 301, "/path/expose1.pdf", Thesis.BillingStatus.NONE));
        theses.add(new Thesis(2, "Thesis on Biology", Thesis.Status.REGISTERED, "Biology", 102, 202, 302, "/path/expose2.pdf", Thesis.BillingStatus.ISSUED));
        theses.add(new Thesis(3, "Thesis on Physics", Thesis.Status.SUBMITTED, "Physics", 103, 203, 303, "/path/expose3.pdf", Thesis.BillingStatus.PAID));
        return theses;
    }
}
