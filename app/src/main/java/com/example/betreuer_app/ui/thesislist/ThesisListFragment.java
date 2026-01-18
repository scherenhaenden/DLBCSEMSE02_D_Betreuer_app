package com.example.betreuer_app.ui.thesislist;

import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.ThesisDetailActivity;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.viewmodel.ThesisListViewModel;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.button.MaterialButton;

public class ThesisListFragment extends Fragment {
    private RecyclerView recyclerView;
    private ThesisListViewModel viewModel;
    private ThesisListAdapter adapter;
    private MaterialToolbar toolbar;
    private FloatingActionButton fabAddThesis;
    private MaterialButton buttonPrevious;
    private MaterialButton buttonNext;
    private TextView textPageIndicator;
    private ProgressBar progressBar;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_thesis_list, container, false);
        recyclerView = view.findViewById(R.id.thesesRecyclerView);
        toolbar = view.findViewById(R.id.toolbar);
        fabAddThesis = view.findViewById(R.id.fab_add_thesis);
        buttonPrevious = view.findViewById(R.id.button_thesis_previous);
        buttonNext = view.findViewById(R.id.button_thesis_next);
        textPageIndicator = view.findViewById(R.id.text_thesis_page_indicator);
        progressBar = view.findViewById(R.id.thesis_list_progress);
        recyclerView.setLayoutManager(new LinearLayoutManager(getContext()));
        return view;
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        viewModel = new ViewModelProvider(this).get(ThesisListViewModel.class);

        if (toolbar != null) {
            toolbar.setNavigationOnClickListener(v -> {
                if (getActivity() != null) {
                    getActivity().onBackPressed();
                }
            });
        }

        // TODO: Navigate to CreateThesisActivity if needed, already handled in dashboard or here?
        // The user has a separate activity for creation, usually started from dashboard,
        // but if we want it here:
        // Intent intent = new Intent(getActivity(), com.example.betreuer_app.CreateThesisActivity.class);
        // startActivity(intent);
        if (fabAddThesis != null) {
            fabAddThesis.setVisibility(View.GONE);
            fabAddThesis.setOnClickListener(null);
        }

        viewModel.getTheses().observe(getViewLifecycleOwner(), new Observer<ThesesResponse>() {
            @Override
            public void onChanged(ThesesResponse thesesResponse) {
                if (thesesResponse != null && thesesResponse.getItems() != null) {
                    adapter = new ThesisListAdapter(thesesResponse.getItems());
                    adapter.setOnItemClickListener(new ThesisListAdapter.OnItemClickListener() {
                        @Override
                        public void onItemClick(ThesisApiModel thesis) {
                            Intent intent = new Intent(getContext(), ThesisDetailActivity.class);
                            intent.putExtra("THESIS_ID", thesis.getId().toString());
                            startActivity(intent);
                        }
                    });
                    recyclerView.setAdapter(adapter);
                    updatePaginationUi();
                }
            }
        });

        viewModel.getError().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String error) {
                if (error != null) {
                    Toast.makeText(getContext(), error, Toast.LENGTH_SHORT).show();
                }
            }
        });

        viewModel.getLoading().observe(getViewLifecycleOwner(), new Observer<Boolean>() {
            @Override
            public void onChanged(Boolean isLoading) {
                if (isLoading != null && progressBar != null) {
                    progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
                }
            }
        });

        viewModel.getCurrentPage().observe(getViewLifecycleOwner(), new Observer<Integer>() {
            @Override
            public void onChanged(Integer page) {
                updatePaginationUi();
            }
        });

        viewModel.getTotalPages().observe(getViewLifecycleOwner(), new Observer<Integer>() {
            @Override
            public void onChanged(Integer totalPages) {
                updatePaginationUi();
            }
        });

        if (buttonPrevious != null) {
            buttonPrevious.setOnClickListener(v -> {
                Integer currentPage = viewModel.getCurrentPage().getValue();
                if (currentPage != null && currentPage > 1) {
                    viewModel.loadTheses(currentPage - 1);
                }
            });
        }

        if (buttonNext != null) {
            buttonNext.setOnClickListener(v -> {
                Integer currentPage = viewModel.getCurrentPage().getValue();
                Integer totalPages = viewModel.getTotalPages().getValue();
                if (currentPage != null && totalPages != null && currentPage < totalPages) {
                    viewModel.loadTheses(currentPage + 1);
                }
            });
        }
    }

    private void updatePaginationUi() {
        Integer currentPage = viewModel.getCurrentPage().getValue();
        Integer totalPages = viewModel.getTotalPages().getValue();
        int safeCurrentPage = currentPage != null ? currentPage : 1;
        int safeTotalPages = totalPages != null ? totalPages : 1;

        if (textPageIndicator != null) {
            textPageIndicator.setText(getString(
                R.string.thesis_page_indicator,
                safeCurrentPage,
                safeTotalPages
            ));
        }
        if (buttonPrevious != null) {
            buttonPrevious.setEnabled(safeCurrentPage > 1);
        }
        if (buttonNext != null) {
            buttonNext.setEnabled(safeCurrentPage < safeTotalPages);
        }
    }
}
