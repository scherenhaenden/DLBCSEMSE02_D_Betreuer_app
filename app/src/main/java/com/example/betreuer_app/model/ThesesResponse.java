package com.example.betreuer_app.model;

import java.util.List;

/**
 * Represents a response containing a paginated list of theses.
 * Used for API responses that return multiple thesis items with pagination information.
 */
public class ThesesResponse {
    private List<ThesisApiModel> items;
    private int totalCount;
    private int page;
    private int pageSize;

    /**
     * Constructor for a ThesesResponse.
     * @param items The list of thesis items on the current page.
     * @param totalCount The total number of theses available.
     * @param page The current page number.
     * @param pageSize The number of items per page.
     */
    public ThesesResponse(List<ThesisApiModel> items, int totalCount, int page, int pageSize) {
        this.items = items;
        this.totalCount = totalCount;
        this.page = page;
        this.pageSize = pageSize;
    }

    /**
     * Returns the list of thesis items on the current page.
     * @return The list of ThesisApiModel objects.
     */
    public List<ThesisApiModel> getItems() {
        return items;
    }

    /**
     * Sets the list of thesis items on the current page.
     * @param items The new list of ThesisApiModel objects.
     */
    public void setItems(List<ThesisApiModel> items) {
        this.items = items;
    }

    /**
     * Returns the total number of theses available.
     * @return The total count.
     */
    public int getTotalCount() {
        return totalCount;
    }

    /**
     * Sets the total number of theses available.
     * @param totalCount The new total count.
     */
    public void setTotalCount(int totalCount) {
        this.totalCount = totalCount;
    }

    /**
     * Returns the current page number.
     * @return The page number.
     */
    public int getPage() {
        return page;
    }

    /**
     * Sets the current page number.
     * @param page The new page number.
     */
    public void setPage(int page) {
        this.page = page;
    }

    /**
     * Returns the number of items per page.
     * @return The page size.
     */
    public int getPageSize() {
        return pageSize;
    }

    /**
     * Sets the number of items per page.
     * @param pageSize The new page size.
     */
    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }
}
