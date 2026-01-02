package com.example.betreuer_app.model;

import java.util.List;

public class ThesesResponse {
    private List<ThesisApiModel> items;
    private int totalCount;
    private int page;
    private int pageSize;

    public ThesesResponse(List<ThesisApiModel> items, int totalCount, int page, int pageSize) {
        this.items = items;
        this.totalCount = totalCount;
        this.page = page;
        this.pageSize = pageSize;
    }

    public List<ThesisApiModel> getItems() {
        return items;
    }

    public void setItems(List<ThesisApiModel> items) {
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
