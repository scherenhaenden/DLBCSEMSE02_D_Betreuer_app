package com.example.betreuer_app;

import android.app.DatePickerDialog;
import android.content.Context;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentActivity;

import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.api.ThesisRequestApiService;
import com.example.betreuer_app.model.CreateThesisRequestRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisRequestResponse;
import com.google.android.material.textfield.TextInputEditText;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;
import java.util.UUID;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SupervisionRequestFragment extends Fragment {

    private AutoCompleteTextView thesisTitleInput;
    private TextInputEditText etStartDate;
    private TextInputEditText etEndDate;
    private TextInputEditText etMessage;
    private ThesisApiService thesisApiService;
    private ThesisRequestApiService thesisRequestApiService;
    private List<ThesisApiModel> thesesList = new ArrayList<>();
    private String tutorId;
    private boolean isSelectingSecondSupervisor = false;
    private String thesisIdForSecondSupervisor = null;
    private ThesisRequestResponse firstSupervisionRequest = null;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_supervision_request, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        // --- Initialization ---
        thesisTitleInput = view.findViewById(R.id.thesis_title_input);
        etStartDate = view.findViewById(R.id.et_start_date);
        etEndDate = view.findViewById(R.id.et_end_date);
        etMessage = view.findViewById(R.id.et_message);

        thesisApiService = ApiClient.getThesisApiService(getContext());
        thesisRequestApiService = ApiClient.getThesisRequestApiService(getContext());

        // --- Process Intent ---
        String tutorName = "";
        FragmentActivity activity = getActivity();
        if (activity != null && activity.getIntent() != null) {
            tutorName = activity.getIntent().getStringExtra("TUTOR_NAME");
            tutorId = activity.getIntent().getStringExtra("TUTOR_ID");
            isSelectingSecondSupervisor = activity.getIntent().getBooleanExtra("SELECTING_SECOND_SUPERVISOR", false);
            thesisIdForSecondSupervisor = activity.getIntent().getStringExtra("THESIS_ID");
        }

        // --- Toolbar Setup ---
        com.google.android.material.appbar.MaterialToolbar toolbar = view.findViewById(R.id.toolbar);
        toolbar.setNavigationIcon(R.drawable.ic_close);
        toolbar.setNavigationOnClickListener(v -> {
            if (getActivity() != null) {
                getActivity().finish();
            }
        });

        // --- Tutor Info Card Setup ---
        TextView nameTextView = view.findViewById(R.id.tutor_name_textview);
        if (nameTextView != null && tutorName != null) {
            nameTextView.setText(tutorName);
        }
        ImageView avatar = view.findViewById(R.id.tutor_avatar);
        TextView initials = view.findViewById(R.id.tutor_initials);
        if (avatar != null && initials != null) {
            avatar.setVisibility(View.GONE);
            initials.setVisibility(View.VISIBLE);
            String initialText = "?";
            if (tutorName != null && !tutorName.isEmpty()) {
                String[] parts = tutorName.split(" ");
                String first = parts.length > 0 ? parts[0] : "";
                String last = parts.length > 1 ? parts[parts.length - 1] : "";
                initialText = (first.isEmpty() ? "" : first.substring(0, 1)) + (last.isEmpty() ? "" : last.substring(0, 1));
            }
            initials.setText(initialText);
            int[] avatarColors = {0xFFE57373, 0xFFF06292, 0xFFBA68C8, 0xFF9575CD, 0xFF7986CB, 0xFF64B5F6, 0xFF4FC3F7, 0xFF4DD0E1, 0xFF4DB6AC, 0xFF81C784, 0xFFAED581, 0xFFFF8A65, 0xFFA1887F, 0xFF90A4AE};
            int colorIndex = Math.abs(tutorName != null ? tutorName.hashCode() : 0) % avatarColors.length;
            android.graphics.drawable.GradientDrawable background = new android.graphics.drawable.GradientDrawable();
            background.setShape(android.graphics.drawable.GradientDrawable.OVAL);
            background.setColor(avatarColors[colorIndex]);
            initials.setBackground(background);
        }

        // --- UI Setup for Supervision Request ---
        etStartDate.setOnClickListener(v -> showDatePickerDialog(etStartDate));
        etEndDate.setOnClickListener(v -> showDatePickerDialog(etEndDate));

        // Wenn wir einen zweiten Supervisor auswählen, lade die erste Anfrage
        if (isSelectingSecondSupervisor && thesisIdForSecondSupervisor != null) {
            loadFirstSupervisionRequest(thesisIdForSecondSupervisor);
        }

        fetchTheses();

        // --- Send Button Listener ---
        view.findViewById(R.id.send_request_button).setOnClickListener(v -> sendSupervisionRequest());
    }

    private void showDatePickerDialog(TextInputEditText editText) {
        Context context = getContext();
        if (context == null) return;

        Calendar calendar = Calendar.getInstance();
        DatePickerDialog datePickerDialog = new DatePickerDialog(
                context,
                (v, year, month, dayOfMonth) -> {
                    Calendar newDate = Calendar.getInstance();
                    newDate.set(year, month, dayOfMonth);
                    SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
                    editText.setText(sdf.format(newDate.getTime()));
                },
                calendar.get(Calendar.YEAR),
                calendar.get(Calendar.MONTH),
                calendar.get(Calendar.DAY_OF_MONTH)
        );
        datePickerDialog.show();
    }

    private void loadFirstSupervisionRequest(String thesisId) {
        Context context = getContext();
        if (context == null) return;

        // Lade alle Anfragen und filtere nach der ThesisId
        thesisRequestApiService.getMyRequests(1, 100).enqueue(new Callback<com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse> call, Response<com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null && response.body().getItems() != null) {
                    // Finde die erste (älteste) Betreuungsanfrage für diese Thesis
                    ThesisRequestResponse oldestRequest = null;
                    for (ThesisRequestResponse request : response.body().getItems()) {
                        if (request.getThesisId() != null
                            && request.getThesisId().toString().equals(thesisId)
                            && "SUPERVISION".equals(request.getRequestType())
                            && "ACCEPTED".equals(request.getStatus())) {

                            if (oldestRequest == null ||
                                (request.getCreatedAt() != null &&
                                 (oldestRequest.getCreatedAt() == null ||
                                  request.getCreatedAt().compareTo(oldestRequest.getCreatedAt()) < 0))) {
                                oldestRequest = request;
                            }
                        }
                    }

                    if (oldestRequest != null) {
                        firstSupervisionRequest = oldestRequest;
                        // Setze die Datumsfelder mit den Daten aus der ersten Anfrage
                        if (oldestRequest.getPlannedStartOfSupervision() != null) {
                            etStartDate.setText(oldestRequest.getPlannedStartOfSupervision());
                        }
                        if (oldestRequest.getPlannedEndOfSupervision() != null) {
                            etEndDate.setText(oldestRequest.getPlannedEndOfSupervision());
                        }

                        // Mache die Felder read-only
                        etStartDate.setEnabled(false);
                        etStartDate.setFocusable(false);
                        etEndDate.setEnabled(false);
                        etEndDate.setFocusable(false);
                    }
                }
            }

            @Override
            public void onFailure(Call<com.example.betreuer_app.model.ThesisRequestResponsePaginatedResponse> call, Throwable t) {
                Context ctx = getContext();
                if (ctx != null) {
                    Toast.makeText(ctx, "Fehler beim Laden der ersten Anfrage: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    private void fetchTheses() {
        thesisApiService.getTheses(1, 100).enqueue(new Callback<ThesesResponse>() {
            @Override
            public void onResponse(Call<ThesesResponse> call, Response<ThesesResponse> response) {
                Context context = getContext();
                if (context == null) return; // Prevent crash if fragment is detached

                if (response.isSuccessful() && response.body() != null) {
                    thesesList = response.body().getItems();
                    List<String> thesisTitles = new ArrayList<>();
                    for (ThesisApiModel thesis : thesesList) {
                        thesisTitles.add(thesis.getTitle());
                    }
                    ArrayAdapter<String> adapter = new ArrayAdapter<>(context, android.R.layout.simple_dropdown_item_1line, thesisTitles);
                    thesisTitleInput.setAdapter(adapter);

                    // Wenn wir einen zweiten Supervisor auswählen, wähle die richtige Thesis aus
                    if (isSelectingSecondSupervisor && thesisIdForSecondSupervisor != null) {
                        for (ThesisApiModel thesis : thesesList) {
                            if (thesis.getId() != null && thesis.getId().toString().equals(thesisIdForSecondSupervisor)) {
                                thesisTitleInput.setText(thesis.getTitle(), false);
                                thesisTitleInput.setEnabled(false); // Mache das Feld read-only
                                break;
                            }
                        }
                    }
                } else {
                    Toast.makeText(context, "Fehler beim Laden der Daten", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesesResponse> call, Throwable t) {
                Context context = getContext();
                if (context == null) return;
                Toast.makeText(context, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void sendSupervisionRequest() {
        Context context = getContext();
        if (context == null) return;

        String selectedTitle = thesisTitleInput.getText().toString().trim();
        ThesisApiModel selectedThesis = null;
        for (ThesisApiModel thesis : thesesList) {
            if (thesis.getTitle().equals(selectedTitle)) {
                selectedThesis = thesis;
                break;
            }
        }

        if (selectedThesis == null) {
            Toast.makeText(context, "Bitte wählen Sie einen gültigen Titel aus.", Toast.LENGTH_SHORT).show();
            return;
        }

        if (tutorId == null || tutorId.trim().isEmpty()) {
            Toast.makeText(context, "Tutor-ID fehlt.", Toast.LENGTH_SHORT).show();
            return;
        }

        UUID tutorUuid;
        try {
            tutorUuid = UUID.fromString(tutorId);
        } catch (IllegalArgumentException e) {
            Toast.makeText(context, "Ungültige Tutor-ID.", Toast.LENGTH_SHORT).show();
            return;
        }

        String startDate = etStartDate.getText().toString();
        String endDate = etEndDate.getText().toString();
        String message = etMessage.getText().toString();

        if (startDate.isEmpty()) {
            etStartDate.setError("Startdatum ist erforderlich");
            return;
        }
        if (endDate.isEmpty()) {
            etEndDate.setError("Enddatum ist erforderlich");
            return;
        }

        // Bestimme den Request-Type:
        // - SUPERVISION: Wenn ein Student einen Betreuer sucht
        // - CO_SUPERVISION: Wenn ein Tutor einen zweiten Supervisor sucht
        String requestType = isSelectingSecondSupervisor ? "CO_SUPERVISION" : "SUPERVISION";

        CreateThesisRequestRequest request = new CreateThesisRequestRequest(
                selectedThesis.getId(),
                tutorUuid,
                requestType,
                message,
                startDate,
                endDate
        );

        thesisRequestApiService.createRequest(request).enqueue(new Callback<ThesisRequestResponse>() {
            @Override
            public void onResponse(Call<ThesisRequestResponse> call, Response<ThesisRequestResponse> response) {
                FragmentActivity activity = getActivity();
                if (activity == null) return;

                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(activity, "Anfrage erfolgreich gesendet.", Toast.LENGTH_SHORT).show();
                    activity.finish();
                } else {
                    String errorBodyString = null;
                    if (response.errorBody() != null) {
                        try {
                            errorBodyString = response.errorBody().string();
                        } catch (IOException e) {
                            errorBodyString = null;
                        }
                    }
                    String userMessage = mapReviewRequestErrorToUserMessage(activity, response.code(), errorBodyString);
                    Toast.makeText(activity, userMessage, Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<ThesisRequestResponse> call, Throwable t) {
                FragmentActivity activity = getActivity();
                if (activity == null) return;
                Toast.makeText(activity, "Netzwerkfehler: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private String mapReviewRequestErrorToUserMessage(Context context, int statusCode, @Nullable String errorBodyString) {
        String fallback = context.getString(R.string.review_request_error_generic);
        String messageCandidate = extractErrorMessage(errorBodyString);
        String contentToCheck = messageCandidate != null ? messageCandidate : errorBodyString;
        if (contentToCheck != null) {
            String normalized = contentToCheck.toLowerCase(Locale.ROOT);
            if (normalized.contains("the selected tutor does not cover the subject area of this thesis")
                    || (normalized.contains("fachbereich") && normalized.contains("gehört nicht"))
                    || (normalized.contains("subject area") && normalized.contains("does not cover"))) {
                return context.getString(R.string.review_request_error_subject_area_mismatch);
            }
            if (normalized.contains("a supervision request already exists for this thesis")) {
                return context.getString(R.string.review_request_error_duplicate_request);
            }
            if (normalized.contains("only the thesis owner") && normalized.contains("can request supervision")) {
                return context.getString(R.string.review_request_error_not_owner);
            }
            if (normalized.contains("receiver of a request must be a tutor")) {
                return context.getString(R.string.review_request_error_receiver_not_tutor);
            }
            if (normalized.contains("second supervisor cannot be the same as the main supervisor")) {
                return context.getString(R.string.review_request_error_same_supervisor);
            }
            if (statusCode == 400 || statusCode == 409 || statusCode == 422) {
                return fallback;
            }
        }
        return fallback;
    }

    private @Nullable String extractErrorMessage(@Nullable String errorBodyString) {
        if (errorBodyString == null || errorBodyString.trim().isEmpty()) {
            return null;
        }
        try {
            JSONObject jsonObject = new JSONObject(errorBodyString);
            if (jsonObject.has("message")) {
                return jsonObject.optString("message", null);
            }
            if (jsonObject.has("error")) {
                return jsonObject.optString("error", null);
            }
            if (jsonObject.has("detail")) {
                return jsonObject.optString("detail", null);
            }
        } catch (JSONException e) {
            return null;
        }
        return null;
    }
}
