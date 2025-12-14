package com.example.betreuer_app.ui.thesislist;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.viewmodel.ThesisListViewModel;

public class ThesisListFragment extends Fragment {
    private RecyclerView recyclerView;
    private ThesisListViewModel viewModel;
    private ThesisListAdapter adapter;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_thesis_list, container, false);
        recyclerView = view.findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(getContext()));
        return view;
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        viewModel = new ViewModelProvider(this).get(ThesisListViewModel.class);

        viewModel.getTheses().observe(getViewLifecycleOwner(), new Observer<ThesesResponse>() {
            @Override
            public void onChanged(ThesesResponse thesesResponse) {
                if (thesesResponse != null) {
                    adapter = new ThesisListAdapter(thesesResponse.getItems());
                    recyclerView.setAdapter(adapter);
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
    }
}
