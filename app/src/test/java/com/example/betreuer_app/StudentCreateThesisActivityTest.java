package com.example.betreuer_app;

import android.widget.EditText;
import android.widget.Button;
import com.example.betreuer_app.repository.ThesisRepository;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;

import static org.junit.Assert.*;
import static org.mockito.Mockito.*;

@RunWith(RobolectricTestRunner.class)
@Config(sdk = 24, manifest = Config.NONE) // Use SDK 24 to match minSdk
public class StudentCreateThesisActivityTest {

    @Mock
    private ThesisRepository thesisRepository;

    private StudentCreateThesisActivity activity;

    @Before
    public void setUp() {
        MockitoAnnotations.openMocks(this);
        activity = Robolectric.buildActivity(StudentCreateThesisActivity.class).create().get();
    }

    @Test
    public void createButtonClick_withEmptyTitle_setsError() {
        // Set up the UI
        EditText etTitle = activity.findViewById(R.id.et_thesis_title);
        Button btnCreate = activity.findViewById(R.id.btn_create_thesis);

        // Leave title empty
        etTitle.setText("");

        // Click the button
        btnCreate.performClick();

        // Check that error is set on title
        assertEquals("Titel ist erforderlich", etTitle.getError());
    }

    @Test
    public void createButtonClick_withValidInputs_doesNotSetError() {
        // Set up the UI
        EditText etTitle = activity.findViewById(R.id.et_thesis_title);
        EditText etDescription = activity.findViewById(R.id.et_thesis_description);
        Button btnCreate = activity.findViewById(R.id.btn_create_thesis);

        etTitle.setText("Valid Title");
        etDescription.setText("Valid Description");

        // Click the button
        btnCreate.performClick();

        // Check that no error on title
        assertNull(etTitle.getError());
    }
}
