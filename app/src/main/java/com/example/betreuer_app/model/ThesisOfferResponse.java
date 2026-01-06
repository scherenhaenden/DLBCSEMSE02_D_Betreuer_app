package com.example.betreuer_app.model;

import java.util.List;

public class ThesisOfferResponse {
    private List<ThesisOfferApiModel> items;
    private int totalCount;
    private int page;
    private int pageSize;

    public List<ThesisOfferApiModel> getItems() {
        return items;
    }

    public void setItems(List<ThesisOfferApiModel> items) {
        this.items = items;
    }

    public int getTotalCount() {
        return totalCount;
    }

    public void setTotalCount(int totalCount) {
        this.totalCount = totalCount;
    }

    public int getPage() {
        return page;
    }

    public void setPage(int page) {
        this.page = page;
    }

    public int getPageSize() {
        return pageSize;
    }

    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }
}
