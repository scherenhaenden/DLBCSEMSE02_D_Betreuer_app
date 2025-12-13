package com.example.betreuer_app.model;

import java.util.List;

public class UsersResponse {
    private List<UserApi> items;
    private int totalCount;
    private int page;
    private int pageSize;

    public UsersResponse(List<UserApi> items, int totalCount, int page, int pageSize) {
        this.items = items;
        this.totalCount = totalCount;
        this.page = page;
        this.pageSize = pageSize;
    }

    // Getters and setters
    public List<UserApi> getItems() {
        return items;
    }

    public void setItems(List<UserApi> items) {
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
