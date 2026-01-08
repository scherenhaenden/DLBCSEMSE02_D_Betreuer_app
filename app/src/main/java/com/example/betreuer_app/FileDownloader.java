package com.example.betreuer_app;

import android.content.ContentValues;
import android.content.Context;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import okhttp3.ResponseBody;

public class FileDownloader {

    private static final String TAG = "FileDownloader";

    public boolean writeResponseBodyToDisk(Context context, ResponseBody body, String fileName) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            return saveFileWithMediaStore(context, body, fileName);
        } else {
            return saveFileLegacy(body, fileName);
        }
    }

    private boolean saveFileWithMediaStore(Context context, ResponseBody body, String fileName) {
        ContentValues values = new ContentValues();
        values.put(MediaStore.Downloads.DISPLAY_NAME, fileName);
        values.put(MediaStore.Downloads.MIME_TYPE, "application/octet-stream");
        values.put(MediaStore.Downloads.IS_PENDING, 1);

        Uri uri = context.getContentResolver().insert(MediaStore.Downloads.EXTERNAL_CONTENT_URI, values);
        if (uri == null) {
            Log.e(TAG, "Failed to create new MediaStore record.");
            return false;
        }

        try (InputStream inputStream = body.byteStream();
             OutputStream outputStream = context.getContentResolver().openOutputStream(uri)) {
            if (outputStream == null) {
                Log.e(TAG, "Failed to open output stream for URI: " + uri);
                context.getContentResolver().delete(uri, null, null); // Clean up pending entry
                return false;
            }

            writeStream(inputStream, outputStream);

            values.clear();
            values.put(MediaStore.Downloads.IS_PENDING, 0);
            context.getContentResolver().update(uri, values, null, null);
            return true;
        } catch (IOException e) {
            Log.e(TAG, "Failed to save file.", e);
            // If the download fails, delete the created entry
            context.getContentResolver().delete(uri, null, null);
            return false;
        }
    }

    private boolean saveFileLegacy(ResponseBody body, String fileName) {
        File downloadDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS);
        if (!downloadDir.exists()) {
            if (!downloadDir.mkdirs()) {
                Log.e(TAG, "Failed to create download directory: " + downloadDir.getAbsolutePath());
                return false;
            }
        }

        File file = createUniqueFile(downloadDir, fileName);

        try (InputStream inputStream = body.byteStream();
             OutputStream outputStream = new FileOutputStream(file)) {
            writeStream(inputStream, outputStream);
            return true;
        } catch (IOException e) {
            Log.e(TAG, "Failed to save file.", e);
            return false;
        }
    }

    private File createUniqueFile(File path, String fileName) {
        File file = new File(path, fileName);
        if (!file.exists()) {
            return file;
        }

        int counter = 1;
        String baseName = fileName;
        String extension = "";
        int dotIndex = fileName.lastIndexOf('.');
        if (dotIndex > 0) {
            baseName = fileName.substring(0, dotIndex);
            extension = fileName.substring(dotIndex);
        }

        do {
            String newName = baseName + "_" + counter + extension;
            file = new File(path, newName);
            counter++;
        } while (file.exists());

        return file;
    }

    private void writeStream(InputStream inputStream, OutputStream outputStream) throws IOException {
        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = inputStream.read(buffer)) != -1) {
            outputStream.write(buffer, 0, bytesRead);
        }
    }
}
