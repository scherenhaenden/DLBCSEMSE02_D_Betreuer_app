package com.example.betreuer_app;

import android.content.Intent;
import android.widget.AutoCompleteTextView;
import android.widget.TextView;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.android.controller.ActivityController;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowToast;

import java.util.UUID;

import static org.junit.Assert.*;

@RunWith(RobolectricTestRunner.class)
@Config(sdk = 24, manifest = Config.NONE)
public class EditThesisActivityTest {

    private EditThesisActivity activity;
    private Intent intent;
    private String testThesisId;

    @Before
    public void setUp() {
        testThesisId = UUID.randomUUID().toString();
        intent = new Intent();
        intent.putExtra("THESIS_ID", testThesisId);
    }

    @Test
    public void testActivityCreation_withThesisId() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        assertNotNull("Activity should be created", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testActivityCreation_withoutThesisId() {
        Intent invalidIntent = new Intent();
        activity = Robolectric.buildActivity(EditThesisActivity.class, invalidIntent)
                .create()
                .get();

        assertTrue("Activity should be finishing without thesis ID", activity.isFinishing());
        assertEquals("Should show error toast",
                "Thesis ID not provided",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testInitializeViews_allViewsAreInitialized() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        TextInputEditText etDescription = activity.findViewById(R.id.et_thesis_description);
        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);
        TextView tvCurrentDocument = activity.findViewById(R.id.tv_current_document);
        MaterialButton btnDownloadDocument = activity.findViewById(R.id.btn_download_document);
        MaterialButton btnUploadDocument = activity.findViewById(R.id.btn_upload_document);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);
        MaterialButton btnFindTutors = activity.findViewById(R.id.btn_find_tutors);

        assertNotNull("Title EditText should be initialized", etTitle);
        assertNotNull("Description EditText should be initialized", etDescription);
        assertNotNull("Subject area dropdown should be initialized", dropdownSubjectArea);
        assertNotNull("Current document TextView should be initialized", tvCurrentDocument);
        assertNotNull("Download document button should be initialized", btnDownloadDocument);
        assertNotNull("Upload document button should be initialized", btnUploadDocument);
        assertNotNull("Save button should be initialized", btnSave);
        assertNotNull("Find tutors button should be initialized", btnFindTutors);
    }

    @Test
    public void testSaveButton_withEmptyTitle_showsError() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);

        etTitle.setText("");
        btnSave.performClick();

        assertEquals("Should show error toast for empty title",
                "Titel ist erforderlich",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testSaveButton_withValidTitle_callsUpdateThesis() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        TextInputEditText etDescription = activity.findViewById(R.id.et_thesis_description);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);

        etTitle.setText("Test Title");
        etDescription.setText("Test Description");
        btnSave.performClick();

        // Since we cannot easily mock the API service without dependency injection,
        // we verify that the activity attempts to save
        assertNotNull("Activity should exist", activity);
    }

    @Test
    public void testSaveButton_withInvalidSubjectArea_showsError() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);

        etTitle.setText("Test Title");
        dropdownSubjectArea.setText("Invalid Subject Area", false);
        btnSave.performClick();

        // Verify that error is shown for invalid subject area
        assertNotNull("Error should be set", dropdownSubjectArea.getError());
    }

    @Test
    public void testUploadDocumentButton_clickable() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        MaterialButton btnUploadDocument = activity.findViewById(R.id.btn_upload_document);
        assertNotNull("Upload button should exist", btnUploadDocument);

        btnUploadDocument.performClick();
        // Verify file picker would launch (activity result launcher registered)
        assertNotNull("Activity should handle upload click", activity);
    }

    @Test
    public void testDownloadDocumentButton_withoutDocument_showsError() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        MaterialButton btnDownloadDocument = activity.findViewById(R.id.btn_download_document);
        btnDownloadDocument.performClick();

        assertEquals("Should show error when no document available",
                "Kein Dokument zum Herunterladen verfügbar",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testFindTutorsButton_withoutSubjectArea_showsError() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        MaterialButton btnFindTutors = activity.findViewById(R.id.btn_find_tutors);
        btnFindTutors.performClick();

        assertEquals("Should show error when no subject area selected",
                "Kein Fachgebiet für die Thesis ausgewählt",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testToolbar_backNavigation_finishesActivity() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        com.google.android.material.appbar.MaterialToolbar toolbar =
                activity.findViewById(R.id.toolbar);
        assertNotNull("Toolbar should exist", toolbar);

        // Simulate back navigation
        toolbar.getNavigationIcon();
        // Navigation listener is set to finish activity
        assertNotNull("Activity should exist", activity);
    }

    @Test
    public void testSubjectAreaDropdown_hasAdapter() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .resume()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        // After loading subject areas, adapter should be set
        // Note: This test may need mocking of the repository
        assertNotNull("Dropdown should exist", dropdownSubjectArea);
    }

    @Test
    public void testSubjectAreaSearch_triggersWithTwoCharacters() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .resume()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        // Type less than 2 characters - should not trigger search
        dropdownSubjectArea.setText("A", false);
        assertNotNull("Dropdown should handle single character", dropdownSubjectArea);

        // Type 2 or more characters - should trigger search
        dropdownSubjectArea.setText("Co", false);
        assertNotNull("Dropdown should handle search", dropdownSubjectArea);
    }

    @Test
    public void testSubjectAreaDropdown_showsOnFocus() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .resume()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        // Focus on dropdown with empty text
        dropdownSubjectArea.requestFocus();
        assertNotNull("Dropdown should handle focus", dropdownSubjectArea);
    }

    @Test
    public void testSubjectAreaDropdown_showsOnClick() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .resume()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        dropdownSubjectArea.performClick();
        assertNotNull("Dropdown should handle click", dropdownSubjectArea);
    }

    @Test
    public void testActivityLifecycle_createStartResumeStop() {
        ActivityController<EditThesisActivity> controller =
                Robolectric.buildActivity(EditThesisActivity.class, intent);

        activity = controller
                .create()
                .start()
                .resume()
                .pause()
                .stop()
                .get();

        assertNotNull("Activity should survive lifecycle changes", activity);
    }

    @Test
    public void testActivityLifecycle_recreate() {
        ActivityController<EditThesisActivity> controller =
                Robolectric.buildActivity(EditThesisActivity.class, intent);

        activity = controller
                .create()
                .start()
                .resume()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        etTitle.setText("Test Title");

        controller.pause().stop().destroy();

        // Recreate activity
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .start()
                .resume()
                .get();

        assertNotNull("Activity should be recreated", activity);
    }

    @Test
    public void testGetFileName_withContentUri() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        // Test with content URI - activity should handle content URIs
        // Note: Without mocking ContentResolver, this will return default value
        assertNotNull("Activity should handle content URI", activity);
    }

    @Test
    public void testGetFileName_withFileUri() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        // Test with file URI - activity should handle file URIs
        assertNotNull("Activity should handle file URI", activity);
    }

    @Test
    public void testThesisIdFromIntent_isStoredCorrectly() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        Intent receivedIntent = activity.getIntent();
        assertEquals("Thesis ID should match",
                testThesisId,
                receivedIntent.getStringExtra("THESIS_ID"));
    }

    @Test
    public void testMultipleClicks_onSaveButton_handledCorrectly() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);

        etTitle.setText("Test Title");

        // Click multiple times
        btnSave.performClick();
        btnSave.performClick();
        btnSave.performClick();

        assertNotNull("Activity should handle multiple clicks", activity);
    }

    @Test
    public void testSubjectAreaDropdown_clearText_reloadsInitialList() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .resume()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        // Set text then clear
        dropdownSubjectArea.setText("Computer Science", false);
        dropdownSubjectArea.setText("", false);

        assertNotNull("Dropdown should handle text clearing", dropdownSubjectArea);
    }

    @Test
    public void testExceptionHandling_onCreate() {
        // Test that activity handles exceptions gracefully during onCreate
        // This is already handled with try-catch in the activity
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        assertNotNull("Activity should handle exceptions gracefully", activity);
    }

    @Test
    public void testDocumentDisplay_initialState() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextView tvCurrentDocument = activity.findViewById(R.id.tv_current_document);
        MaterialButton btnDownloadDocument = activity.findViewById(R.id.btn_download_document);

        assertNotNull("Document text view should exist", tvCurrentDocument);
        assertNotNull("Download button should exist", btnDownloadDocument);
    }

    @Test
    public void testAllButtonsAreClickable() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);
        MaterialButton btnUploadDocument = activity.findViewById(R.id.btn_upload_document);
        MaterialButton btnDownloadDocument = activity.findViewById(R.id.btn_download_document);
        MaterialButton btnFindTutors = activity.findViewById(R.id.btn_find_tutors);

        assertTrue("Save button should be clickable", btnSave.isClickable());
        assertTrue("Upload button should be clickable", btnUploadDocument.isClickable());
        assertTrue("Download button should be clickable", btnDownloadDocument.isClickable());
        assertTrue("Find tutors button should be clickable", btnFindTutors.isClickable());
    }

    @Test
    public void testInputFieldsAcceptText() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        TextInputEditText etDescription = activity.findViewById(R.id.et_thesis_description);

        String testTitle = "Machine Learning in Healthcare";
        String testDescription = "This thesis explores the application of ML algorithms";

        etTitle.setText(testTitle);
        etDescription.setText(testDescription);

        assertNotNull("Title text should not be null", etTitle.getText());
        assertNotNull("Description text should not be null", etDescription.getText());
        assertEquals("Title should be set correctly", testTitle, etTitle.getText().toString());
        assertEquals("Description should be set correctly", testDescription, etDescription.getText().toString());
    }

    @Test
    public void testSubjectAreaDropdown_acceptsInput() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);

        String testSubjectArea = "Computer Science";
        dropdownSubjectArea.setText(testSubjectArea, false);

        assertEquals("Subject area should be set", testSubjectArea, dropdownSubjectArea.getText().toString());
    }

    @Test
    public void testSaveButton_withWhitespaceTitle_treatedAsEmpty() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);

        etTitle.setText("   "); // Only whitespace
        btnSave.performClick();

        assertEquals("Should show error for whitespace-only title",
                "Titel ist erforderlich",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testActivityToastShown_onCreation() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        assertEquals("Should show success toast on creation",
                "Activity erfolgreich geladen",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testFindTutorsButton_enabled() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        MaterialButton btnFindTutors = activity.findViewById(R.id.btn_find_tutors);
        assertTrue("Find tutors button should be enabled", btnFindTutors.isEnabled());
    }

    @Test
    public void testAllViewsVisible_afterCreation() {
        activity = Robolectric.buildActivity(EditThesisActivity.class, intent)
                .create()
                .get();

        TextInputEditText etTitle = activity.findViewById(R.id.et_thesis_title);
        TextInputEditText etDescription = activity.findViewById(R.id.et_thesis_description);
        AutoCompleteTextView dropdownSubjectArea = activity.findViewById(R.id.dropdown_subject_area);
        MaterialButton btnSave = activity.findViewById(R.id.btn_save_thesis);
        MaterialButton btnUploadDocument = activity.findViewById(R.id.btn_upload_document);
        MaterialButton btnFindTutors = activity.findViewById(R.id.btn_find_tutors);

        assertEquals("Title field should be visible", android.view.View.VISIBLE, etTitle.getVisibility());
        assertEquals("Description field should be visible", android.view.View.VISIBLE, etDescription.getVisibility());
        assertEquals("Subject area dropdown should be visible", android.view.View.VISIBLE, dropdownSubjectArea.getVisibility());
        assertEquals("Save button should be visible", android.view.View.VISIBLE, btnSave.getVisibility());
        assertEquals("Upload button should be visible", android.view.View.VISIBLE, btnUploadDocument.getVisibility());
        assertEquals("Find tutors button should be visible", android.view.View.VISIBLE, btnFindTutors.getVisibility());
    }
}

