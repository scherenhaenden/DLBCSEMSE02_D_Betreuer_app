package com.example.betreuer_app.repository;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.OpenableColumns;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.UUID;

public class ThesisRepository {
    private final ThesisApiService apiService;
    private final Context context;

    public ThesisRepository(Context context) {
        this.context = context;
        apiService = ApiClient.getThesisApiService(context);
    }

    public void getTheses(int page, int pageSize, Callback<ThesesResponse> callback) {
        Call<ThesesResponse> call = apiService.getTheses(page, pageSize);
        call.enqueue(callback);
    }

    public void createThesis(String title, String description, String topicId, Callback<ThesisApiModel> callback) {
        executeCreateThesis(title, description, topicId, null, callback);
    }

    public void createThesisWithFile(String title, String description, String topicId, Uri fileUri, Callback<ThesisApiModel> callback) {
        executeCreateThesis(title, description, topicId, fileUri, callback);
    }

    private void executeCreateThesis(String title, String description, String topicId, Uri fileUri, Callback<ThesisApiModel> callback) {
        RequestBody titlePart = RequestBody.create(MediaType.parse("text/plain"), title);
        RequestBody descriptionPart = RequestBody.create(MediaType.parse("text/plain"), description != null ? description : "");
        RequestBody subjectAreaIdPart = topicId != null
                ? RequestBody.create(MediaType.parse("text/plain"), topicId)
                : null;

        MultipartBody.Part documentPart = null;
        File fileToDelete = null;

        if (fileUri != null) {
            File file = prepareFilePart(fileUri);
            if (file == null) {
                callback.onFailure(null, new Exception("Konnte Datei nicht verarbeiten"));
                return;
            }
            fileToDelete = file;

            String mimeType = context.getContentResolver().getType(fileUri);
            if (mimeType == null) mimeType = "application/octet-stream";

            RequestBody requestFile = RequestBody.create(MediaType.parse(mimeType), file);
            documentPart = MultipartBody.Part.createFormData("Document", file.getName(), requestFile);
        }

        Call<ThesisApiModel> call = apiService.createThesis(titlePart, descriptionPart, subjectAreaIdPart, documentPart);

        final File finalFileToDelete = fileToDelete;
        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(Call<ThesisApiModel> call, retrofit2.Response<ThesisApiModel> response) {
                if (finalFileToDelete != null) {
                    finalFileToDelete.delete();
                }
                callback.onResponse(call, response);
            }

            @Override
            public void onFailure(Call<ThesisApiModel> call, Throwable t) {
                if (finalFileToDelete != null) {
                    finalFileToDelete.delete();
                }
                callback.onFailure(call, t);
            }
        });
    }

    private File prepareFilePart(Uri fileUri) {
        try {
            InputStream inputStream = context.getContentResolver().openInputStream(fileUri);
            if (inputStream == null) return null;

            String originalName = getFileNameFromUri(fileUri);
            String uniqueName = (originalName != null ? originalName : "upload_temp_file") + "_" + UUID.randomUUID().toString();

            File file = new File(context.getCacheDir(), uniqueName);

            try (FileOutputStream outputStream = new FileOutputStream(file)) {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = inputStream.read(buffer)) != -1) {
                    outputStream.write(buffer, 0, bytesRead);
                }
            }
            return file;
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    private String getFileNameFromUri(Uri uri) {
        String result = null;
        if (uri.getScheme() != null && uri.getScheme().equals("content")) {
            try (Cursor cursor = context.getContentResolver().query(uri, null, null, null, null)) {
                if (cursor != null && cursor.moveToFirst()) {
                    int index = cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME);
                    if (index >= 0) {
                        result = cursor.getString(index);
                    }
                }
            }
        }
        if (result == null) {
            result = uri.getPath();
            if (result != null) {
                int cut = result.lastIndexOf('/');
                if (cut != -1) {
                    result = result.substring(cut + 1);
                }
            }
        }
        return result;
    }
}
