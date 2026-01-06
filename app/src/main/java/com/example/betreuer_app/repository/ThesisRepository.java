package com.example.betreuer_app.repository;

import android.content.Context;
import android.net.Uri;
import android.webkit.MimeTypeMap;
import com.example.betreuer_app.api.ApiClient;
import com.example.betreuer_app.api.ThesisApiService;
import com.example.betreuer_app.model.CreateThesisRequest;
import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.model.ThesisApiModel;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class ThesisRepository {
    private ThesisApiService apiService;
    private Context context;

    public ThesisRepository(Context context) {
        this.context = context;
        apiService = ApiClient.getThesisApiService(context);
    }

    public void getTheses(int page, int pageSize, Callback<ThesesResponse> callback) {
        Call<ThesesResponse> call = apiService.getTheses(page, pageSize);
        call.enqueue(callback);
    }

    public void createThesis(String title, String topicId, Callback<ThesisApiModel> callback) {
        CreateThesisRequest request = new CreateThesisRequest(title, topicId);
        Call<ThesisApiModel> call = apiService.createThesis(request);
        call.enqueue(callback);
    }

    public void createThesisWithFile(String title, String topicId, Uri fileUri, Callback<ThesisApiModel> callback) {
        File file = prepareFilePart(fileUri);
        if (file == null) {
            callback.onFailure(null, new Exception("Konnte Datei nicht verarbeiten"));
            return;
        }

        RequestBody titlePart = RequestBody.create(MediaType.parse("text/plain"), title);
        RequestBody subjectAreaIdPart = topicId != null 
            ? RequestBody.create(MediaType.parse("text/plain"), topicId) 
            : null;

        String mimeType = context.getContentResolver().getType(fileUri);
        if (mimeType == null) mimeType = "application/octet-stream";
        
        RequestBody requestFile = RequestBody.create(MediaType.parse(mimeType), file);
        MultipartBody.Part body = MultipartBody.Part.createFormData("Document", file.getName(), requestFile);

        Call<ThesisApiModel> call = apiService.createThesisWithFile(titlePart, subjectAreaIdPart, body);
        call.enqueue(callback);
    }

    private File prepareFilePart(Uri fileUri) {
        try {
            InputStream inputStream = context.getContentResolver().openInputStream(fileUri);
            if (inputStream == null) return null;

            File file = new File(context.getCacheDir(), "upload_temp_file");
            FileOutputStream outputStream = new FileOutputStream(file);
            
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.read(buffer)) != -1) {
                outputStream.write(buffer, 0, bytesRead);
            }
            outputStream.close();
            inputStream.close();
            return file;
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }
}
