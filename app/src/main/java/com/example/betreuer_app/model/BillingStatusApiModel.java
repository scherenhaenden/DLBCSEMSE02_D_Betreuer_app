package com.example.betreuer_app.model;

import java.util.List;

/**
 * Represents a billing status for a thesis.
 * Billing statuses indicate the payment state of a thesis.
 */
public class BillingStatusApiModel extends BaseEntityApiModel {
    private String name;
    private List<ThesisApiModel> theses;

    /**
     * Default constructor.
     */
    public BillingStatusApiModel() {
    }

    /**
     * Constructor for a new billing status.
     * @param name The name of the billing status.
     */
    public BillingStatusApiModel(String name) {
        this.name = name;
    }

    /**
     * Returns the name of the billing status.
     * @return The billing status name.
     */
    public String getName() {
        return name;
    }

    /**
     * Sets the name of the billing status.
     * @param name The new billing status name.
     */
    public void setName(String name) {
        this.name = name;
    }

    /**
     * Returns the list of theses with this billing status.
     * @return The list of ThesisApiModel objects.
     */
    public List<ThesisApiModel> getTheses() {
        return theses;
    }

    /**
     * Sets the list of theses with this billing status.
     * @param theses The new list of ThesisApiModel objects.
     */
    public void setTheses(List<ThesisApiModel> theses) {
        this.theses = theses;
    }
}
