package com.example.betreuer_app;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.SubjectAreaApiService;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.api.UserApiService;
import com.example.betreuer_app.model.BillingStatusResponse;
import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse;
import com.example.betreuer_app.model.UserResponse;
import com.example.betreuer_app.util.BillingStatusDisplayMapper;
import com.example.betreuer_app.util.SessionManager;
import com.example.betreuer_app.util.ThesisStatusDisplayLogic;
import com.example.betreuer_app.util.ThesisStatusHelper;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;

import java.util.List;
import java.util.UUID;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ThesisDetailActivity extends AppCompatActivity {

    private TextView textViewTitle;
    private TextView textViewDescription;
    private TextView textViewStatus;
    private TextView textViewSubjectArea;
    private MaterialButton btnDownloadDocument;
    private MaterialButton btnEditThesis;
    private MaterialButton btnAddSecondSupervisor;
    private Spinner spinnerStatus;
    private Spinner spinnerBillingStatus;
    private TextView textViewBillingStatus;

    // Views for the person details
    private TextView ownerName;
    private TextView tutorName;
    private TextView secondSupervisorName;

    private ThesisApiService thesisApiService;
    private ThesisRequestApiService thesisRequestApiService;
    private UserApiService userApiService;
    private SubjectAreaApiService subjectAreaApiService;

    private FileDownloader fileDownloader;

    private String thesisId;
    private ThesisApiModel currentThesis;
    private ThesisApiModel thesisToDownload;
    private ActivityResultLauncher<String> requestPermissionLauncher;
    private boolean isSpinnerInitializing = false;
    private boolean hasSupervisionRequest = false;
    private boolean isSupervisionRequestAccepted = false;
    private boolean isStatusSpinnerUserInitiated = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thesis_detail);

        fileDownloader = new FileDownloader();

        requestPermissionLauncher =
                registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                    if (isGranted) {
                        if (thesisToDownload != null) {
                            downloadDocument(thesisToDownload);
                        }
                    } else {
                        Toast.makeText(this, "Permission denied to write to storage", Toast.LENGTH_SHORT).show();
                    }
                });

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        toolbar.setNavigationOnClickListener(v -> finish());

        // Initialize views from the main layout
        textViewTitle = findViewById(R.id.textViewTitle);
        textViewDescription = findViewById(R.id.textViewDescription);
        textViewStatus = findViewById(R.id.textViewStatus);
        textViewSubjectArea = findViewById(R.id.textViewSubjectArea);
        btnDownloadDocument = findViewById(R.id.btn_download_document);
        btnEditThesis = findViewById(R.id.btn_edit_thesis);
        btnAddSecondSupervisor = findViewById(R.id.btn_add_second_supervisor);
        spinnerStatus = findViewById(R.id.spinner_status);
        spinnerBillingStatus = findViewById(R.id.spinner_billingstatus);
        textViewBillingStatus = findViewById(R.id.textView_billingstatus);

        // Initialize views from the included person layouts
        View ownerItem = findViewById(R.id.item_owner);
        ((TextView) ownerItem.findViewById(R.id.person_label)).setText("Student");
        ownerName = ownerItem.findViewById(R.id.person_name);

        View tutorItem = findViewById(R.id.item_tutor);
        ((TextView) tutorItem.findViewById(R.id.person_label)).setText("Betreuer");
        tutorName = tutorItem.findViewById(R.id.person_name);

        View secondSupervisorItem = findViewById(R.id.item_second_supervisor);
        ((TextView) secondSupervisorItem.findViewById(R.id.person_label)).setText("Zweitkorrektor");
        secondSupervisorName = secondSupervisorItem.findViewById(R.id.person_name);

        thesisApiService = ApiClient.getThesisApiService(this);
        thesisRequestApiService = ApiClient.getThesisRequestApiService(this);
        userApiService = ApiClient.getUserApiService(this);
        subjectAreaApiService = ApiClient.getSubjectAreaApiService(this);

        if (getIntent().hasExtra("THESIS_ID")) {
            thesisId = getIntent().getStringExtra("THESIS_ID");
            setupThesisStatusSpinner();
            loadBillingStatuses(); // This will load statuses, then thesis details
        } else {
            Toast.makeText(this, "Thesis ID not provided", Toast.LENGTH_SHORT).show();
            finish();
        }

        btnEditThesis.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisDetailActivity.this, EditThesisActivity.class);
            intent.putExtra("THESIS_ID", thesisId);
            startActivity(intent);
        });

        btnAddSecondSupervisor.setOnClickListener(v -> {
            Intent intent = new Intent(ThesisDetailActivity.this, TutorListActivity.class);
            intent.putExtra("SELECTING_SECOND_SUPERVISOR", true);
            intent.putExtra("THESIS_ID", thesisId);
            startActivity(intent);
        });
    }

    private void loadBillingStatuses() {
        thesisApiService.getBillingStatuses().enqueue(new Callback<List<BillingStatusResponse>>() {
            @Override
            public void onResponse(Call<List<BillingStatusResponse>> call, Response<List<BillingStatusResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    setupBillingStatusSpinner(response.body());
                    loadThesisDetails(thesisId);
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to load billing statuses", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<List<BillingStatusResponse>> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error loading billing statuses: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setupThesisStatusSpinner() {
        SessionManager sessionManager = new SessionManager(this);
        boolean isTutor = sessionManager.isTutor();

        // Für Studenten: später dynamisch basierend auf Thesis-Daten
        // Für Tutoren: auch dynamisch
        // Initialisierung erfolgt in displayThesisDetails

        spinnerStatus.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                // CRITICAL DEBUGGING: Log every spinner selection event
                android.util.Log.e("SPINNER_BUG", "=== SPINNER ITEM SELECTED ===");
                android.util.Log.e("SPINNER_BUG", "Position: " + position);
                android.util.Log.e("SPINNER_BUG", "currentThesis: " + (currentThesis != null ? currentThesis.getId() : "null"));
                android.util.Log.e("SPINNER_BUG", "isSpinnerInitializing: " + isSpinnerInitializing);
                android.util.Log.e("SPINNER_BUG", "isTutor: " + isTutor);
                android.util.Log.e("SPINNER_BUG", "isStatusSpinnerUserInitiated: " + isStatusSpinnerUserInitiated);

                if (currentThesis == null || isSpinnerInitializing) {
                    android.util.Log.e("SPINNER_BUG", "RETURN: currentThesis null or initializing");
                    android.util.Log.e("SPINNER_BUG", "=============================");
                    return;
                }
                if (isTutor && !isStatusSpinnerUserInitiated) {
                    android.util.Log.e("SPINNER_BUG", "RETURN: Tutor mode but not user initiated");
                    android.util.Log.e("SPINNER_BUG", "=============================");
                    return;
                }
                if (isTutor) {
                    isStatusSpinnerUserInitiated = false;
                }
                com.example.betreuer_app.model.ThesisStatusResponse selectedStatus =
                    (com.example.betreuer_app.model.ThesisStatusResponse) parent.getItemAtPosition(position);

                android.util.Log.e("SPINNER_BUG", "Selected status: " + selectedStatus.getName());
                android.util.Log.e("SPINNER_BUG", "Current thesis status: " + currentThesis.getStatus());

                if (!selectedStatus.getName().equals(currentThesis.getStatus())) {
                    android.util.Log.e("SPINNER_BUG", "STATUS CHANGE DETECTED! Will update from " +
                        currentThesis.getStatus() + " to " + selectedStatus.getName());
                    if (isTutor) {
                        android.util.Log.e("SPINNER_BUG", "Calling promptTutorStatusChange()");
                        promptTutorStatusChange(selectedStatus);
                    } else {
                        android.util.Log.e("SPINNER_BUG", "Calling updateThesisStatus()");
                        updateThesisStatus(selectedStatus);
                    }
                } else {
                    android.util.Log.e("SPINNER_BUG", "No status change - same status selected");
                }
                if (!isTutor) {
                    isStatusSpinnerUserInitiated = false;
                }
                android.util.Log.e("SPINNER_BUG", "=============================");
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
        spinnerStatus.setOnTouchListener((v, event) -> {
            android.util.Log.e("SPINNER_BUG", "=== SPINNER TOUCHED BY USER ===");
            android.util.Log.e("SPINNER_BUG", "Setting isStatusSpinnerUserInitiated = true");
            android.util.Log.e("SPINNER_BUG", "================================");
            isStatusSpinnerUserInitiated = true;
            return false;
        });
    }

    private void updateThesisStatus(com.example.betreuer_app.model.ThesisStatusResponse newStatus) {
        if (currentThesis == null) return;

        SessionManager sessionManager = new SessionManager(this);
        boolean isStudent = !sessionManager.isTutor();
        boolean isTutor = sessionManager.isTutor();

        if (isStudent && !canStudentSetStatus(currentThesis, newStatus.getName())) {
            revertStatusSpinner();
            return;
        }
        if (isTutor && !canTutorSetStatus(currentThesis, newStatus.getName())) {
            revertStatusSpinner();
            return;
        }

        ThesisApiService.StatusUpdateRequest request = new ThesisApiService.StatusUpdateRequest(newStatus.getName());

        thesisApiService.updateStatus(currentThesis.getId().toString(), request).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(ThesisDetailActivity.this, "Status erfolgreich aktualisiert", Toast.LENGTH_SHORT).show();
                    currentThesis = response.body();
                    if (isStudent && ("REGISTERED".equals(newStatus.getName()) || "SUBMITTED".equals(newStatus.getName()))) {
                        ThesisStatusHelper.markStudentRegistrationConfirmed(ThesisDetailActivity.this, currentThesis);
                    }
                    displayThesisDetails(currentThesis);
                } else {
                    String errorMessage;
                    if (response.code() == 403) {
                        errorMessage = "Sie sind nicht berechtigt, den Status zu ändern";
                    } else {
                        errorMessage = "Fehler beim Aktualisieren des Status";
                        try {
                            if (response.errorBody() != null) {
                                errorMessage += ": " + response.errorBody().string();
                            }
                        } catch (Exception e) {
                            errorMessage += " (Code: " + response.code() + ")";
                        }
                    }
                    Toast.makeText(ThesisDetailActivity.this, errorMessage, Toast.LENGTH_LONG).show();
                    // Revert spinner to previous selection
                    revertStatusSpinner();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
                revertStatusSpinner();
            }
        });
    }

    private void revertStatusSpinner() {
        if (currentThesis == null || spinnerStatus.getAdapter() == null) return;

        ArrayAdapter<com.example.betreuer_app.model.ThesisStatusResponse> adapter =
            (ArrayAdapter<com.example.betreuer_app.model.ThesisStatusResponse>) spinnerStatus.getAdapter();

        for (int i = 0; i < adapter.getCount(); i++) {
            com.example.betreuer_app.model.ThesisStatusResponse status = adapter.getItem(i);
            if (status != null && status.getName().equals(currentThesis.getStatus())) {
                spinnerStatus.setSelection(i);
                break;
            }
        }
    }

    private void setupBillingStatusSpinner(List<BillingStatusResponse> statuses) {
        ArrayAdapter<BillingStatusResponse> adapter = new ArrayAdapter<BillingStatusResponse>(this, android.R.layout.simple_spinner_item, statuses) {
            @Override
            public View getView(int position, View convertView, ViewGroup parent) {
                View view = super.getView(position, convertView, parent);
                BillingStatusResponse status = getItem(position);
                if (view instanceof TextView) {
                    ((TextView) view).setText(BillingStatusDisplayMapper.mapBillingStatusToDisplay(
                        getContext(),
                        status != null ? status.getName() : null
                    ));
                }
                return view;
            }

            @Override
            public View getDropDownView(int position, View convertView, ViewGroup parent) {
                View view = super.getDropDownView(position, convertView, parent);
                BillingStatusResponse status = getItem(position);
                if (view instanceof TextView) {
                    ((TextView) view).setText(BillingStatusDisplayMapper.mapBillingStatusToDisplay(
                        getContext(),
                        status != null ? status.getName() : null
                    ));
                }
                return view;
            }
        };
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinnerBillingStatus.setAdapter(adapter);

        // Zeige nur TextView für Studenten, Spinner für Tutoren
        SessionManager sessionManager = new SessionManager(this);
        boolean isTutor = sessionManager.isTutor();

        if (isTutor) {
            spinnerBillingStatus.setVisibility(View.VISIBLE);
            textViewBillingStatus.setVisibility(View.GONE);
        } else {
            spinnerBillingStatus.setVisibility(View.GONE);
            textViewBillingStatus.setVisibility(View.VISIBLE);
        }

        spinnerBillingStatus.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                if (currentThesis == null || isSpinnerInitializing) {
                    return; // Don't proceed if thesis details are not loaded yet or spinner is being initialized
                }
                BillingStatusResponse selectedStatus = (BillingStatusResponse) parent.getItemAtPosition(position);
                if (!selectedStatus.getName().equals(currentThesis.getBillingStatus())) {
                    updateBillingStatus(selectedStatus);
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
    }

    private void updateBillingStatus(BillingStatusResponse newStatus) {
        if (currentThesis == null) return;

        ThesisApiService.BillingStatusUpdateRequest request = new ThesisApiService.BillingStatusUpdateRequest(newStatus.getId());

        thesisApiService.updateBillingStatus(currentThesis.getId().toString(), request).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(ThesisDetailActivity.this, "Rechnungsstatus erfolgreich aktualisiert", Toast.LENGTH_SHORT).show();
                    currentThesis = response.body();
                    // Refresh the display to show updated thesis data
                    displayThesisDetails(currentThesis);
                } else {
                    String errorMessage;
                    if (response.code() == 403) {
                        errorMessage = "Sie sind nicht berechtigt, den Rechnungsstatus zu ändern. Nur Betreuer oder Zweitkorrektoren dieser Arbeit können dies tun.";
                    } else {
                        errorMessage = "Fehler beim Aktualisieren des Rechnungsstatus";
                        try {
                            if (response.errorBody() != null) {
                                errorMessage += ": " + response.errorBody().string();
                            }
                        } catch (Exception e) {
                            errorMessage += " (Code: " + response.code() + ")";
                        }
                    }
                    Toast.makeText(ThesisDetailActivity.this, errorMessage, Toast.LENGTH_LONG).show();
                    // Revert spinner to previous selection
                    spinnerBillingStatus.setSelection(((ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter()).getPosition(getBillingStatusByName(currentThesis.getBillingStatus())));
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_LONG).show();
                // Revert spinner to previous selection
                spinnerBillingStatus.setSelection(((ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter()).getPosition(getBillingStatusByName(currentThesis.getBillingStatus())));
            }
        });
    }

    private BillingStatusResponse getBillingStatusByName(String name) {
        ArrayAdapter<BillingStatusResponse> adapter = (ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter();
        if (adapter == null) {
            return null;
        }
        for (int i = 0; i < adapter.getCount(); i++) {
            BillingStatusResponse status = adapter.getItem(i);
            if (status != null && status.getName().equals(name)) {
                return status;
            }
        }
        return null;
    }

    private void loadThesisDetails(String id) {
        thesisApiService.getThesis(id).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    currentThesis = response.body();
                    displayThesisDetails(currentThesis);
                    loadAdditionalInfo(currentThesis);
                    loadSupervisionRequestStatus(currentThesis.getId().toString());
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Failed to load thesis details", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Error: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void displayThesisDetails(ThesisApiModel thesis) {
        textViewTitle.setText(thesis.getTitle());
        textViewDescription.setText(thesis.getDescription());

        SessionManager sessionManager = new SessionManager(this);
        boolean isStudent = !sessionManager.isTutor();
        boolean isTutor = sessionManager.isTutor();

        // Setup ThesisStatus Spinner/TextView
        isSpinnerInitializing = true; // Prevent spinner listener from triggering during setup
        setupThesisStatusDisplay(thesis, isStudent, isTutor);

        // Setze Rechnungsstatus für TextView (Studenten) und Spinner (Tutoren)
        String billingStatus = thesis.getBillingStatus();
        textViewBillingStatus.setText(BillingStatusDisplayMapper.mapBillingStatusToDisplay(this, billingStatus));

        isSpinnerInitializing = true; // Prevent spinner listener from triggering during setup
        ArrayAdapter<BillingStatusResponse> adapter = (ArrayAdapter<BillingStatusResponse>) spinnerBillingStatus.getAdapter();
        if (adapter != null) {
            for (int i = 0; i < adapter.getCount(); i++) {
                BillingStatusResponse status = adapter.getItem(i);
                if (status != null && status.getName().equals(billingStatus)) {
                    spinnerBillingStatus.setSelection(i);
                    break;
                }
            }
        }
        isSpinnerInitializing = false; // Re-enable spinner listener

        if (thesis.getDocumentFileName() != null && !thesis.getDocumentFileName().isEmpty()) {
            btnDownloadDocument.setVisibility(View.VISIBLE);
            btnDownloadDocument.setText("Exposé herunterladen (" + thesis.getDocumentFileName() + ")");
            btnDownloadDocument.setOnClickListener(v -> {
                this.thesisToDownload = thesis;
                requestDownloadPermission();
            });
        } else {
            btnDownloadDocument.setVisibility(View.GONE);
        }

        updateAddSecondSupervisorButtonVisibility();
    }

    private void setupThesisStatusDisplay(ThesisApiModel thesis, boolean isStudent, boolean isTutor) {
        // CRITICAL DEBUGGING: Log all status information BEFORE any UI changes
        android.util.Log.e("STATUS_BUG", "=== SETUP THESIS STATUS DISPLAY ===");
        android.util.Log.e("STATUS_BUG", "Thesis ID: " + thesis.getId());
        android.util.Log.e("STATUS_BUG", "Status (from thesis object): " + thesis.getStatus());
        android.util.Log.e("STATUS_BUG", "TutorId: " + thesis.getTutorId());
        android.util.Log.e("STATUS_BUG", "SecondSupervisorId: " + thesis.getSecondSupervisorId());
        android.util.Log.e("STATUS_BUG", "hasSecondSupervisor: " + (thesis.getSecondSupervisorId() != null));
        android.util.Log.e("STATUS_BUG", "isTutor: " + isTutor);
        android.util.Log.e("STATUS_BUG", "isStudent: " + isStudent);

        // Use the extracted business logic class
        ThesisStatusDisplayLogic displayLogic = new ThesisStatusDisplayLogic();
        ThesisStatusDisplayLogic.DisplayResult result = displayLogic.computeStatusDisplay(
            this, thesis, isStudent, isTutor, hasSupervisionRequest, isSupervisionRequestAccepted);

        android.util.Log.e("STATUS_BUG", "DisplayMode: " + result.getDisplayMode());
        android.util.Log.e("STATUS_BUG", "Available Statuses: " + result.getAvailableStatuses().size());
        for (int i = 0; i < result.getAvailableStatuses().size(); i++) {
            android.util.Log.e("STATUS_BUG", "  [" + i + "] " + result.getAvailableStatuses().get(i).getName());
        }

        // Apply the display configuration
        if (result.getDisplayMode() == ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW) {
            // Show text view, hide spinner
            textViewStatus.setText(result.getCurrentStatusText());
            textViewStatus.setVisibility(View.VISIBLE);
            spinnerStatus.setVisibility(View.GONE);
            isSpinnerInitializing = false; // Reset flag
            android.util.Log.e("STATUS_BUG", "Mode: TEXT_VIEW - No spinner interaction");
        } else {
            // Show spinner, hide text view
            android.util.Log.e("STATUS_BUG", "Mode: SPINNER - Setting up spinner...");
            textViewStatus.setVisibility(View.GONE);
            spinnerStatus.setVisibility(View.VISIBLE);
            spinnerStatus.setEnabled(result.isSpinnerEnabled());

            // Temporarily remove listener to prevent it from triggering when we set the adapter
            AdapterView.OnItemSelectedListener currentListener = spinnerStatus.getOnItemSelectedListener();
            spinnerStatus.setOnItemSelectedListener(null);
            android.util.Log.e("STATUS_BUG", "Listener temporarily removed");

            // Setup Spinner Adapter
            ArrayAdapter<com.example.betreuer_app.model.ThesisStatusResponse> statusAdapter =
                new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, result.getAvailableStatuses());
            statusAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
            spinnerStatus.setAdapter(statusAdapter);
            android.util.Log.e("STATUS_BUG", "Adapter set with " + result.getAvailableStatuses().size() + " items");

            // Set current selection
            String currentStatus = thesis.getStatus();
            boolean isStudentRegistrationConfirmed = ThesisStatusHelper.isStudentRegistrationConfirmed(this, thesis);
            if (isStudent && "REGISTERED".equals(currentStatus) && !isStudentRegistrationConfirmed) {
                currentStatus = "IN_DISCUSSION";
            }

            int statusIndex = displayLogic.findStatusIndex(result.getAvailableStatuses(), currentStatus);
            android.util.Log.e("STATUS_BUG", "Current status to find: " + currentStatus);
            android.util.Log.e("STATUS_BUG", "Found status index: " + statusIndex);

            if (statusIndex >= 0) {
                spinnerStatus.setSelection(statusIndex);
                android.util.Log.e("STATUS_BUG", "Spinner selection set to index: " + statusIndex);
            } else {
                android.util.Log.e("STATUS_BUG", "WARNING: Status not found in list! Default selection (index 0) will be used.");
                android.util.Log.e("STATUS_BUG", "THIS IS THE BUG! Current status '" + currentStatus + "' not in available statuses!");
            }

            // Re-attach listener after setting selection
            spinnerStatus.setOnItemSelectedListener(currentListener);
            android.util.Log.e("STATUS_BUG", "Listener re-attached");
            isSpinnerInitializing = false; // Reset flag after everything is set up
        }
        isStatusSpinnerUserInitiated = false;
        android.util.Log.e("STATUS_BUG", "=== SETUP COMPLETE ===");
        android.util.Log.e("STATUS_BUG", "Final status in thesis object: " + thesis.getStatus());
        android.util.Log.e("STATUS_BUG", "======================");
    }

    private void loadSupervisionRequestStatus(String thesisId) {
        SessionManager sessionManager = new SessionManager(this);
        if (sessionManager.isTutor()) {
            hasSupervisionRequest = false;
            isSupervisionRequestAccepted = false;
            return;
        }

        thesisRequestApiService.getMyRequests(1, 100).enqueue(new Callback<ThesisRequestResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponsePaginatedResponse> call, Response<ThesisRequestResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null && response.body().getItems() != null) {
                    boolean requestExists = false;
                    boolean requestAccepted = false;

                    for (ThesisRequestResponse request : response.body().getItems()) {
                        if (request.getThesisId() != null
                            && request.getThesisId().toString().equals(thesisId)
                            && "SUPERVISION".equals(request.getRequestType())) {
                            requestExists = true;
                            if ("ACCEPTED".equals(request.getStatus())) {
                                requestAccepted = true;
                                break;
                            }
                        }
                    }

                    boolean statusChanged = (hasSupervisionRequest != requestExists || isSupervisionRequestAccepted != requestAccepted);
                    hasSupervisionRequest = requestExists;
                    isSupervisionRequestAccepted = requestAccepted;

                    // Only re-setup the display if the supervision request status actually changed
                    // This prevents unnecessary re-initialization that could trigger the spinner listener
                    if (currentThesis != null && statusChanged) {
                        // Set flag BEFORE re-initializing to prevent listener from triggering
                        isSpinnerInitializing = true;
                        SessionManager manager = new SessionManager(ThesisDetailActivity.this);
                        boolean isStudent = !manager.isTutor();
                        boolean isTutor = manager.isTutor();
                        setupThesisStatusDisplay(currentThesis, isStudent, isTutor);
                        // Note: isSpinnerInitializing is set to false inside setupThesisStatusDisplay
                    }
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                // Keep defaults and avoid blocking the UI
            }
        });
    }

    private boolean canStudentSetStatus(ThesisApiModel thesis, String targetStatus) {
        if (thesis == null || targetStatus == null) {
            Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
            return false;
        }

        boolean hasTutor = thesis.getTutorId() != null;
        boolean hasFile = thesis.getDocumentFileName() != null && !thesis.getDocumentFileName().isEmpty();
        String currentStatus = thesis.getStatus();

        if ("REGISTERED".equals(targetStatus)) {
            if ("IN_DISCUSSION".equals(currentStatus) && hasTutor) {
                return true;
            }
        }

        if ("SUBMITTED".equals(targetStatus)) {
            if (!"REGISTERED".equals(currentStatus)) {
                Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
                return false;
            }
            if (!hasFile) {
                Toast.makeText(this, getString(R.string.toast_need_expose), Toast.LENGTH_SHORT).show();
                return false;
            }
            return true;
        }

        Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
        return false;
    }

    private boolean canTutorSetStatus(ThesisApiModel thesis, String targetStatus) {
        if (thesis == null || targetStatus == null) {
            Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
            return false;
        }

        String currentStatus = thesis.getStatus();
        boolean hasSecondSupervisor = thesis.getSecondSupervisorId() != null;

        if ("DEFENDED".equals(targetStatus)) {
            if (!"SUBMITTED".equals(currentStatus)) {
                Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
                return false;
            }
            if (!hasSecondSupervisor) {
                Toast.makeText(this, getString(R.string.toast_need_second_examiner), Toast.LENGTH_SHORT).show();
                return false;
            }
            return true;
        }

        Toast.makeText(this, getString(R.string.toast_not_allowed), Toast.LENGTH_SHORT).show();
        return false;
    }

    private void promptTutorStatusChange(com.example.betreuer_app.model.ThesisStatusResponse newStatus) {
        new AlertDialog.Builder(this)
            .setTitle(getString(R.string.thesis_status))
            .setMessage(getString(R.string.confirm_defended_status_change))
            .setPositiveButton(android.R.string.ok, (dialog, which) -> updateThesisStatus(newStatus))
            .setNegativeButton(android.R.string.cancel, (dialog, which) -> dialog.dismiss())
            .show();
    }

    private void updateAddSecondSupervisorButtonVisibility() {
        if (currentThesis == null) {
            btnAddSecondSupervisor.setVisibility(View.GONE);
            return;
        }

        SessionManager sessionManager = new SessionManager(this);
        String currentUserId = sessionManager.getUserId();
        boolean isTutor = sessionManager.isTutor();

        // Zeige Button nur wenn:
        // 1. User ist Tutor
        // 2. User ist der erste Supervisor (TutorId)
        // 3. Es gibt noch keinen zweiten Supervisor
        boolean shouldShowButton = isTutor
            && currentUserId != null
            && currentThesis.getTutorId() != null
            && currentUserId.equals(currentThesis.getTutorId().toString())
            && currentThesis.getSecondSupervisorId() == null;

        btnAddSecondSupervisor.setVisibility(shouldShowButton ? View.VISIBLE : View.GONE);
    }

    private void requestDownloadPermission() {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.Q && ContextCompat.checkSelfPermission(
                this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.WRITE_EXTERNAL_STORAGE);
        } else {
            if (thesisToDownload != null) {
                downloadDocument(thesisToDownload);
            }
        }
    }

    private void downloadDocument(ThesisApiModel thesis) {
        if (thesis.getDocumentFileName() == null) return;

        Toast.makeText(this, "Download gestartet...", Toast.LENGTH_SHORT).show();

        thesisApiService.downloadThesisDocument(thesis.getId().toString()).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    boolean written = fileDownloader.writeResponseBodyToDisk(ThesisDetailActivity.this, response.body(), thesis.getDocumentFileName());
                    if (written) {
                        Toast.makeText(ThesisDetailActivity.this, "Download erfolgreich: " + thesis.getDocumentFileName(), Toast.LENGTH_LONG).show();
                    } else {
                        Toast.makeText(ThesisDetailActivity.this, "Fehler beim Speichern der Datei", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(ThesisDetailActivity.this, "Download fehlgeschlagen", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                Toast.makeText(ThesisDetailActivity.this, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void loadAdditionalInfo(ThesisApiModel thesis) {
        if (thesis.getSubjectAreaId() != null) {
            loadSubjectArea(thesis.getSubjectAreaId());
        } else {
            textViewSubjectArea.setText("N/A");
        }

        if (thesis.getOwnerId() != null) {
            loadUser(thesis.getOwnerId(), ownerName);
        } else {
            ownerName.setText("Unknown");
        }

        if (thesis.getTutorId() != null) {
            loadUser(thesis.getTutorId(), tutorName);
        } else {
            tutorName.setText("None");
        }

        if (thesis.getSecondSupervisorId() != null) {
            loadUser(thesis.getSecondSupervisorId(), secondSupervisorName);
        } else {
            secondSupervisorName.setText("None");
        }
    }

    private void loadUser(UUID userId, TextView targetView) {
        userApiService.getUser(userId).enqueue(new Callback<UserResponse>() {
            @Override
            public void onResponse(Call<UserResponse> call, Response<UserResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    UserResponse user = response.body();
                    String name = (user.getFirstName() != null ? user.getFirstName() : "") + " " +
                            (user.getLastName() != null ? user.getLastName() : "");
                    targetView.setText(name.trim());
                } else {
                    targetView.setText("Error loading user");
                }
            }

            @Override
            public void onFailure(Call<UserResponse> call, Throwable t) {
                targetView.setText("Error");
            }
        });
    }

    private void loadSubjectArea(UUID subjectAreaId) {
        subjectAreaApiService.getSubjectArea(subjectAreaId).enqueue(new Callback<SubjectAreaResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponse> call, Response<SubjectAreaResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    textViewSubjectArea.setText(response.body().getTitle());
                } else {
                    textViewSubjectArea.setText("N/A");
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponse> call, Throwable t) {
                textViewSubjectArea.setText("N/A");
            }
        });
    }
}
