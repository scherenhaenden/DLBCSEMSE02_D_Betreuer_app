package com.example.betreuer_app.ui.thesislist;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.model.Thesis;
import com.example.betreuer_app.viewmodel.ThesisListViewModel;
import java.util.List;

public class ThesisListFragment extends Fragment {
    private RecyclerView recyclerView;
    private ThesisListViewModel viewModel;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_thesis_list, container, false);
        recyclerView = view.findViewById(R.id.recyclerView);
        return view;
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        viewModel = new ViewModelProvider(this).get(ThesisListViewModel.class);
        viewModel.getTheses().observe(getViewLifecycleOwner(), new Observer<List<Thesis>>() {
            @Override
            public void onChanged(List<Thesis> theses) {
                // TODO: Set up adapter and display theses
            }
        });
    }
}
