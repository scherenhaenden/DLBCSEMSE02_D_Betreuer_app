package com.example.betreuer_app.model;

import java.util.List;

public class ThesisRequestResponsePaginatedResponse {
    private List<ThesisRequestResponse> items;
    private int totalCount;
    private int page;
    private int pageSize;

    public List<ThesisRequestResponse> getItems() {
        return items;
    }

    public int getTotalCount() {
        return totalCount;
    }

    public int getPage() {
        return page;
    }

    public int getPageSize() {
        return pageSize;
    }
}
