package com.example.betreuer_app.viewmodel;

import android.net.Uri;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.example.betreuer_app.model.SubjectAreaResponse;
import com.example.betreuer_app.model.SubjectAreaResponsePaginatedResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisDocumentResponse;
import com.example.betreuer_app.repository.SubjectAreaRepository;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.util.Resource;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

/**
 * ViewModel for EditThesisActivity
 * Handles all business logic related to editing a thesis
 */
public class EditThesisViewModel extends ViewModel {

    private final ThesisApiService thesisApiService;
    private final SubjectAreaRepository subjectAreaRepository;

    // LiveData for thesis details
    private final MutableLiveData<Resource<ThesisApiModel>> thesisDetails = new MutableLiveData<>();

    // LiveData for subject areas
    private final MutableLiveData<Resource<List<SubjectAreaResponse>>> subjectAreas = new MutableLiveData<>();

    // LiveData for save operation
    private final MutableLiveData<Resource<ThesisApiModel>> saveResult = new MutableLiveData<>();

    // LiveData for document upload
    private final MutableLiveData<Resource<ThesisDocumentResponse>> uploadResult = new MutableLiveData<>();

    // LiveData for document download
    private final MutableLiveData<Resource<okhttp3.ResponseBody>> downloadResult = new MutableLiveData<>();

    // Current thesis data
    private ThesisApiModel currentThesis;

    // Subject area mapping (name -> ID)
    private final Map<String, String> subjectAreaMap = new HashMap<>();
    private final List<String> subjectAreaNames = new ArrayList<>();

    public EditThesisViewModel(ThesisApiService thesisApiService, SubjectAreaRepository subjectAreaRepository) {
        this.thesisApiService = thesisApiService;
        this.subjectAreaRepository = subjectAreaRepository;
    }

    // Getters for LiveData
    public LiveData<Resource<ThesisApiModel>> getThesisDetails() {
        return thesisDetails;
    }

    public LiveData<Resource<List<SubjectAreaResponse>>> getSubjectAreas() {
        return subjectAreas;
    }

    public LiveData<Resource<ThesisApiModel>> getSaveResult() {
        return saveResult;
    }

    public LiveData<Resource<ThesisDocumentResponse>> getUploadResult() {
        return uploadResult;
    }

    public LiveData<Resource<okhttp3.ResponseBody>> getDownloadResult() {
        return downloadResult;
    }

    public ThesisApiModel getCurrentThesis() {
        return currentThesis;
    }

    public Map<String, String> getSubjectAreaMap() {
        return subjectAreaMap;
    }

    public List<String> getSubjectAreaNames() {
        return subjectAreaNames;
    }

    /**
     * Load thesis details by ID
     */
    public void loadThesisDetails(String thesisId) {
        thesisDetails.setValue(Resource.loading(null));

        thesisApiService.getThesis(thesisId).enqueue(new Callback<ThesisApiModel>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                if (response.isSuccessful() && response.body() != null) {
                    currentThesis = response.body();
                    thesisDetails.setValue(Resource.success(currentThesis));
                } else {
                    thesisDetails.setValue(Resource.error("Failed to load thesis details", null));
                }
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                thesisDetails.setValue(Resource.error(t.getMessage(), null));
            }
        });
    }

    /**
     * Load initial subject areas
     */
    public void loadSubjectAreas() {
        subjectAreas.setValue(Resource.loading(null));

        subjectAreaRepository.getSubjectAreas(1, 100, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<SubjectAreaResponse> areas = response.body().getItems();
                    updateSubjectAreaMap(areas);
                    subjectAreas.setValue(Resource.success(areas));
                } else {
                    subjectAreas.setValue(Resource.error("Failed to load subject areas", null));
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                subjectAreas.setValue(Resource.error(t.getMessage(), null));
            }
        });
    }

    /**
     * Search subject areas by query
     */
    public void searchSubjectAreas(String query) {
        subjectAreaRepository.searchSubjectAreas(query, 1, 20, new Callback<SubjectAreaResponsePaginatedResponse>() {
            @Override
            public void onResponse(Call<SubjectAreaResponsePaginatedResponse> call, Response<SubjectAreaResponsePaginatedResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<SubjectAreaResponse> areas = response.body().getItems();
                    updateSubjectAreaMap(areas);
                    subjectAreas.setValue(Resource.success(areas));
                }
            }

            @Override
            public void onFailure(Call<SubjectAreaResponsePaginatedResponse> call, Throwable t) {
                // Silently fail for search suggestions
            }
        });
    }

    /**
     * Update subject area map with new areas
     */
    private void updateSubjectAreaMap(List<SubjectAreaResponse> areas) {
        if (areas != null) {
            for (SubjectAreaResponse area : areas) {
                String name = area.getTitle();
                java.util.UUID id = area.getId();

                if (name != null && id != null && !subjectAreaMap.containsKey(name)) {
                    subjectAreaNames.add(name);
                    subjectAreaMap.put(name, id.toString());
                }
            }
        }
    }

    /**
     * Get subject area name by ID
     */
    public String getSubjectAreaNameById(String id) {
        for (Map.Entry<String, String> entry : subjectAreaMap.entrySet()) {
            if (entry.getValue().equals(id)) {
                return entry.getKey();
            }
        }
        return null;
    }

    /**
     * Validate thesis input
     */
    public ValidationResult validateThesisInput(String title, String subjectAreaName) {
        if (title == null || title.trim().isEmpty()) {
            return new ValidationResult(false, "Titel ist erforderlich");
        }

        if (subjectAreaName != null && !subjectAreaName.isEmpty() && !subjectAreaMap.containsKey(subjectAreaName)) {
            return new ValidationResult(false, "Bitte wählen Sie ein gültiges Fachgebiet aus der Suche");
        }

        return new ValidationResult(true, null);
    }

    /**
     * Save thesis details
     */
    public void saveThesisDetails(String thesisId, String title, String description, String subjectAreaName) {
        saveResult.setValue(Resource.loading(null));

        String subjectAreaId = null;
        if (subjectAreaName != null && !subjectAreaName.isEmpty()) {
            subjectAreaId = subjectAreaMap.get(subjectAreaName);
        }

        RequestBody titlePart = RequestBody.create(MediaType.parse("text/plain"), title);
        RequestBody descriptionPart = RequestBody.create(MediaType.parse("text/plain"), description);
        RequestBody subjectAreaIdPart = subjectAreaId != null
                ? RequestBody.create(MediaType.parse("text/plain"), subjectAreaId)
                : null;

        thesisApiService.updateThesis(thesisId, titlePart, descriptionPart, subjectAreaIdPart, null)
                .enqueue(new Callback<ThesisApiModel>() {
                    @Override
                    public void onResponse(Call<ThesisApiModel> call, Response<ThesisApiModel> response) {
                        if (response.isSuccessful() && response.body() != null) {
                            saveResult.setValue(Resource.success(response.body()));
                        } else {
                            saveResult.setValue(Resource.error("Fehler beim Speichern der Änderungen", null));
                        }
                    }

                    @Override
                    public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                        saveResult.setValue(Resource.error("Netzwerkfehler: " + t.getMessage(), null));
                    }
                });
    }

    /**
     * Upload document for thesis
     */
    public void uploadDocument(String thesisId, File file, String mimeType) {
        uploadResult.setValue(Resource.loading(null));

        RequestBody requestFile = RequestBody.create(MediaType.parse(mimeType), file);
        MultipartBody.Part body = MultipartBody.Part.createFormData("document", file.getName(), requestFile);

        thesisApiService.updateThesisDocument(thesisId, body).enqueue(new Callback<ThesisDocumentResponse>() {
            @Override
            public void onResponse(Call<ThesisDocumentResponse> call, Response<ThesisDocumentResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    uploadResult.setValue(Resource.success(response.body()));
                } else {
                    String errorMsg = "Fehler beim Hochladen";
                    try {
                        if (response.errorBody() != null) {
                            errorMsg = response.errorBody().string();
                        }
                    } catch (Exception e) {
                        errorMsg = "Fehler beim Hochladen (Code: " + response.code() + ")";
                    }
                    uploadResult.setValue(Resource.error(errorMsg, null));
                }
            }

            @Override
            public void onFailure(Call<ThesisDocumentResponse> call, Throwable t) {
                uploadResult.setValue(Resource.error("Netzwerkfehler: " + t.getMessage(), null));
            }
        });
    }

    /**
     * Download thesis document
     */
    public void downloadDocument(String thesisId) {
        downloadResult.setValue(Resource.loading(null));

        thesisApiService.downloadThesisDocument(thesisId).enqueue(new Callback<okhttp3.ResponseBody>() {
            @Override
            public void onResponse(Call<okhttp3.ResponseBody> call, Response<okhttp3.ResponseBody> response) {
                if (response.isSuccessful() && response.body() != null) {
                    downloadResult.setValue(Resource.success(response.body()));
                } else {
                    downloadResult.setValue(Resource.error("Fehler beim Herunterladen des Dokuments", null));
                }
            }

            @Override
            public void onFailure(Call<okhttp3.ResponseBody> call, Throwable t) {
                downloadResult.setValue(Resource.error("Netzwerkfehler: " + t.getMessage(), null));
            }
        });
    }

    /**
     * Check if thesis has a document
     */
    public boolean hasDocument() {
        return currentThesis != null &&
               currentThesis.getDocumentFileName() != null &&
               !currentThesis.getDocumentFileName().isEmpty();
    }

    /**
     * Get document file name
     */
    public String getDocumentFileName() {
        return currentThesis != null ? currentThesis.getDocumentFileName() : null;
    }

    /**
     * Check if thesis has subject area
     */
    public boolean hasSubjectArea() {
        return currentThesis != null && currentThesis.getSubjectAreaId() != null;
    }

    /**
     * Get subject area ID from current thesis
     */
    public String getThesisSubjectAreaId() {
        return currentThesis != null && currentThesis.getSubjectAreaId() != null
                ? currentThesis.getSubjectAreaId().toString()
                : null;
    }

    /**
     * Validation result class
     */
    public static class ValidationResult {
        public final boolean isValid;
        public final String errorMessage;

        public ValidationResult(boolean isValid, String errorMessage) {
            this.isValid = isValid;
            this.errorMessage = errorMessage;
        }
    }
}

