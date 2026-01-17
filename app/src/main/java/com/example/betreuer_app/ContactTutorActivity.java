package com.example.betreuer_app;

import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.text.TextUtils;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;

public class ContactTutorActivity extends AppCompatActivity {

    private String tutorName;
    private String tutorEmail;
    private String thesisId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_contact_tutor);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        toolbar.setNavigationOnClickListener(v -> onBackPressed());

        if (getIntent() != null) {
            tutorName = getIntent().getStringExtra("TUTOR_NAME");
            tutorEmail = getIntent().getStringExtra("TUTOR_EMAIL");
            thesisId = getIntent().getStringExtra("THESIS_ID");
        }

        TextView tvTutorName = findViewById(R.id.tv_contact_tutor_name);
        TextView tvTutorEmail = findViewById(R.id.tv_contact_tutor_email);

        String displayName = !TextUtils.isEmpty(tutorName) ? tutorName : getString(R.string.contact_tutor_fallback_name);
        tvTutorName.setText(displayName);

        if (!TextUtils.isEmpty(tutorEmail)) {
            tvTutorEmail.setText(tutorEmail);
        } else {
            tvTutorEmail.setText(R.string.contact_tutor_missing_email);
        }

        MaterialButton btnEmailWrite = findViewById(R.id.btn_contact_email_write);
        MaterialButton btnEmailCopy = findViewById(R.id.btn_contact_email_copy);

        btnEmailWrite.setOnClickListener(v -> handleEmailWrite(displayName));
        btnEmailCopy.setOnClickListener(v -> handleEmailCopy());
    }

    /**
     * Handles the process of composing and sending an email to the tutor.
     */
    private void handleEmailWrite(String displayName) {
        if (TextUtils.isEmpty(tutorEmail)) {
            Toast.makeText(this, R.string.contact_tutor_no_email_toast, Toast.LENGTH_SHORT).show();
            return;
        }

        String subject = getString(R.string.contact_tutor_email_subject);
        StringBuilder body = new StringBuilder();
        body.append(getString(R.string.contact_tutor_email_body_intro, displayName));
        if (!TextUtils.isEmpty(thesisId)) {
            body.append(getString(R.string.contact_tutor_email_thesis_id, thesisId)).append("\n\n");
        }
        body.append(getString(R.string.contact_tutor_email_body_outro));

        Intent emailIntent = new Intent(Intent.ACTION_SENDTO);
        emailIntent.setData(Uri.parse("mailto:" + tutorEmail));
        emailIntent.putExtra(Intent.EXTRA_SUBJECT, subject);
        emailIntent.putExtra(Intent.EXTRA_TEXT, body.toString());

        if (emailIntent.resolveActivity(getPackageManager()) != null) {
            startActivity(emailIntent);
        } else {
            Toast.makeText(this, R.string.contact_tutor_no_email_app, Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Copies the tutor's email to the clipboard if it is not empty and shows a toast message.
     */
    private void handleEmailCopy() {
        if (TextUtils.isEmpty(tutorEmail)) {
            Toast.makeText(this, R.string.contact_tutor_no_email_toast, Toast.LENGTH_SHORT).show();
            return;
        }

        ClipboardManager clipboard = (ClipboardManager) getSystemService(CLIPBOARD_SERVICE);
        if (clipboard != null) {
            ClipData clip = ClipData.newPlainText(getString(R.string.contact_tutor_email_label), tutorEmail);
            clipboard.setPrimaryClip(clip);
        }
        Toast.makeText(this, R.string.contact_tutor_email_copied, Toast.LENGTH_SHORT).show();
    }
}
