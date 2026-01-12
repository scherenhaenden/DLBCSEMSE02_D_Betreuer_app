package com.example.betreuer_app;

import android.content.Intent;
import android.widget.ImageView;
import android.widget.TextView;

import com.google.android.material.button.MaterialButton;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.android.controller.ActivityController;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowActivity;
import org.robolectric.shadows.ShadowToast;

import java.util.UUID;

import static org.junit.Assert.*;
import static org.robolectric.Shadows.shadowOf;

@RunWith(RobolectricTestRunner.class)
@Config(sdk = 24, manifest = Config.NONE)
public class TutorProfileActivityTest {

    private TutorProfileActivity activity;
    private Intent intent;
    private String testTutorId;
    private String testTutorName;

    @Before
    public void setUp() {
        testTutorId = UUID.randomUUID().toString();
        testTutorName = "Dr. Max Mustermann";
        intent = new Intent();
        intent.putExtra("TUTOR_ID", testTutorId);
        intent.putExtra("TUTOR_NAME", testTutorName);
    }

    @Test
    public void testActivityCreation_withTutorIdAndName() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        assertNotNull("Activity should be created", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testActivityCreation_withoutTutorId_finishesActivity() {
        Intent invalidIntent = new Intent();
        invalidIntent.putExtra("TUTOR_NAME", testTutorName);

        activity = Robolectric.buildActivity(TutorProfileActivity.class, invalidIntent)
                .create()
                .get();

        assertTrue("Activity should be finishing without tutor ID", activity.isFinishing());
        assertEquals("Should show error toast",
                "Tutor ID missing",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testActivityCreation_withNullIntent() {
        Intent nullIntent = new Intent();

        activity = Robolectric.buildActivity(TutorProfileActivity.class, nullIntent)
                .create()
                .get();

        assertTrue("Activity should be finishing without tutor ID", activity.isFinishing());
        assertEquals("Should show error toast",
                "Tutor ID missing",
                ShadowToast.getTextOfLatestToast());
    }

    @Test
    public void testInitializeViews_allViewsAreInitialized() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        ImageView ivTutorAvatar = activity.findViewById(R.id.iv_tutor_avatar);
        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        assertNotNull("Tutor name TextView should be initialized", tvTutorName);
        assertNotNull("Tutor avatar ImageView should be initialized", ivTutorAvatar);
        assertNotNull("View offers button should be initialized", btnViewOffers);
        assertNotNull("Create request button should be initialized", btnCreateRequest);
    }

    @Test
    public void testTutorName_isDisplayedCorrectly() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Tutor name should be displayed",
                testTutorName,
                tvTutorName.getText().toString());
    }

    @Test
    public void testTutorName_withoutName_showsDefaultText() {
        Intent intentWithoutName = new Intent();
        intentWithoutName.putExtra("TUTOR_ID", testTutorId);

        activity = Robolectric.buildActivity(TutorProfileActivity.class, intentWithoutName)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Should show default text when name is missing",
                "Unbekannter Tutor",
                tvTutorName.getText().toString());
    }

    @Test
    public void testTutorName_withNullName_showsDefaultText() {
        Intent intentWithNullName = new Intent();
        intentWithNullName.putExtra("TUTOR_ID", testTutorId);
        intentWithNullName.putExtra("TUTOR_NAME", (String) null);

        activity = Robolectric.buildActivity(TutorProfileActivity.class, intentWithNullName)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Should show default text when name is null",
                "Unbekannter Tutor",
                tvTutorName.getText().toString());
    }

    @Test
    public void testViewOffersButton_startsThesisOfferDashboardActivity() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        btnViewOffers.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisOfferDashboardActivity",
                ThesisOfferDashboardActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testViewOffersButton_passesCorrectExtras() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        btnViewOffers.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should pass tutor ID",
                testTutorId,
                startedIntent.getStringExtra("TUTOR_ID"));
        assertEquals("Should pass tutor name",
                testTutorName,
                startedIntent.getStringExtra("TUTOR_NAME"));
        assertEquals("Should pass mode",
                "VIEW_TUTOR_OFFERS",
                startedIntent.getStringExtra("MODE"));
    }

    @Test
    public void testCreateRequestButton_startsSupervisionRequestActivity() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);
        btnCreateRequest.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start SupervisionRequestActivity",
                SupervisionRequestActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testCreateRequestButton_passesCorrectExtras() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);
        btnCreateRequest.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should pass tutor ID",
                testTutorId,
                startedIntent.getStringExtra("TUTOR_ID"));
        assertEquals("Should pass tutor name",
                testTutorName,
                startedIntent.getStringExtra("TUTOR_NAME"));
    }

    @Test
    public void testBothButtons_areClickable() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        assertTrue("View offers button should be clickable", btnViewOffers.isClickable());
        assertTrue("Create request button should be clickable", btnCreateRequest.isClickable());
    }

    @Test
    public void testBothButtons_areEnabled() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        assertTrue("View offers button should be enabled", btnViewOffers.isEnabled());
        assertTrue("Create request button should be enabled", btnCreateRequest.isEnabled());
    }

    @Test
    public void testBothButtons_areVisible() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        assertEquals("View offers button should be visible",
                android.view.View.VISIBLE,
                btnViewOffers.getVisibility());
        assertEquals("Create request button should be visible",
                android.view.View.VISIBLE,
                btnCreateRequest.getVisibility());
    }

    @Test
    public void testTutorIdFromIntent_isStoredCorrectly() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        Intent receivedIntent = activity.getIntent();
        assertEquals("Tutor ID should match",
                testTutorId,
                receivedIntent.getStringExtra("TUTOR_ID"));
    }

    @Test
    public void testTutorNameFromIntent_isStoredCorrectly() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        Intent receivedIntent = activity.getIntent();
        assertEquals("Tutor name should match",
                testTutorName,
                receivedIntent.getStringExtra("TUTOR_NAME"));
    }

    @Test
    public void testActivityLifecycle_createStartResumeStop() {
        ActivityController<TutorProfileActivity> controller =
                Robolectric.buildActivity(TutorProfileActivity.class, intent);

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
        ActivityController<TutorProfileActivity> controller =
                Robolectric.buildActivity(TutorProfileActivity.class, intent);

        activity = controller
                .create()
                .start()
                .resume()
                .get();

        controller.pause().stop().destroy();

        // Recreate activity
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .start()
                .resume()
                .get();

        assertNotNull("Activity should be recreated", activity);
        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Tutor name should still be displayed after recreate",
                testTutorName,
                tvTutorName.getText().toString());
    }

    @Test
    public void testMultipleClicks_onViewOffersButton() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);

        // Click multiple times
        btnViewOffers.performClick();
        btnViewOffers.performClick();
        btnViewOffers.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);

        // Should have multiple intents started
        assertNotNull("First intent should be started", shadowActivity.getNextStartedActivity());
        assertNotNull("Second intent should be started", shadowActivity.getNextStartedActivity());
        assertNotNull("Third intent should be started", shadowActivity.getNextStartedActivity());
    }

    @Test
    public void testMultipleClicks_onCreateRequestButton() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        // Click multiple times
        btnCreateRequest.performClick();
        btnCreateRequest.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);

        // Should have multiple intents started
        assertNotNull("First intent should be started", shadowActivity.getNextStartedActivity());
        assertNotNull("Second intent should be started", shadowActivity.getNextStartedActivity());
    }

    @Test
    public void testAlternatingButtonClicks() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        btnViewOffers.performClick();
        btnCreateRequest.performClick();
        btnViewOffers.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);

        Intent firstIntent = shadowActivity.getNextStartedActivity();
        Intent secondIntent = shadowActivity.getNextStartedActivity();
        Intent thirdIntent = shadowActivity.getNextStartedActivity();

        assertEquals("First click should start ThesisOfferDashboardActivity",
                ThesisOfferDashboardActivity.class.getName(),
                firstIntent.getComponent().getClassName());
        assertEquals("Second click should start SupervisionRequestActivity",
                SupervisionRequestActivity.class.getName(),
                secondIntent.getComponent().getClassName());
        assertEquals("Third click should start ThesisOfferDashboardActivity",
                ThesisOfferDashboardActivity.class.getName(),
                thirdIntent.getComponent().getClassName());
    }

    @Test
    public void testTutorAvatar_isVisible() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        ImageView ivTutorAvatar = activity.findViewById(R.id.iv_tutor_avatar);
        assertEquals("Tutor avatar should be visible",
                android.view.View.VISIBLE,
                ivTutorAvatar.getVisibility());
    }

    @Test
    public void testTutorName_isVisible() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Tutor name should be visible",
                android.view.View.VISIBLE,
                tvTutorName.getVisibility());
    }

    @Test
    public void testAllViewsVisible_afterCreation() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        ImageView ivTutorAvatar = activity.findViewById(R.id.iv_tutor_avatar);
        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);
        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        assertEquals("Tutor name should be visible",
                android.view.View.VISIBLE,
                tvTutorName.getVisibility());
        assertEquals("Tutor avatar should be visible",
                android.view.View.VISIBLE,
                ivTutorAvatar.getVisibility());
        assertEquals("View offers button should be visible",
                android.view.View.VISIBLE,
                btnViewOffers.getVisibility());
        assertEquals("Create request button should be visible",
                android.view.View.VISIBLE,
                btnCreateRequest.getVisibility());
    }

    @Test
    public void testEmptyTutorName_showsDefaultText() {
        Intent intentWithEmptyName = new Intent();
        intentWithEmptyName.putExtra("TUTOR_ID", testTutorId);
        intentWithEmptyName.putExtra("TUTOR_NAME", "");

        activity = Robolectric.buildActivity(TutorProfileActivity.class, intentWithEmptyName)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        // Empty string is not null, so it will show empty string, not default
        assertEquals("Should show empty string when name is empty",
                "",
                tvTutorName.getText().toString());
    }

    @Test
    public void testLongTutorName_isDisplayed() {
        String longName = "Prof. Dr. Dr. h.c. mult. Maximilian Alexander Friedrich von Mustermann III";
        Intent intentWithLongName = new Intent();
        intentWithLongName.putExtra("TUTOR_ID", testTutorId);
        intentWithLongName.putExtra("TUTOR_NAME", longName);

        activity = Robolectric.buildActivity(TutorProfileActivity.class, intentWithLongName)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Should display long tutor name",
                longName,
                tvTutorName.getText().toString());
    }

    @Test
    public void testSpecialCharactersInTutorName_areDisplayed() {
        String specialName = "Dr. Müller-Østergård & Söhne";
        Intent intentWithSpecialName = new Intent();
        intentWithSpecialName.putExtra("TUTOR_ID", testTutorId);
        intentWithSpecialName.putExtra("TUTOR_NAME", specialName);

        activity = Robolectric.buildActivity(TutorProfileActivity.class, intentWithSpecialName)
                .create()
                .get();

        TextView tvTutorName = activity.findViewById(R.id.tv_tutor_name);
        assertEquals("Should display tutor name with special characters",
                specialName,
                tvTutorName.getText().toString());
    }

    @Test
    public void testActivityDoesNotCrash_withValidInputs() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .start()
                .resume()
                .get();

        assertNotNull("Activity should not crash with valid inputs", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testIntentExtras_arePersisted() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        // Get intent extras
        String tutorId = activity.getIntent().getStringExtra("TUTOR_ID");
        String tutorName = activity.getIntent().getStringExtra("TUTOR_NAME");

        assertEquals("Tutor ID should be persisted", testTutorId, tutorId);
        assertEquals("Tutor name should be persisted", testTutorName, tutorName);
    }

    @Test
    public void testViewOffersButton_doesNotCrash() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnViewOffers = activity.findViewById(R.id.btn_view_offers);

        // Should not throw exception
        btnViewOffers.performClick();

        assertNotNull("Activity should not crash after button click", activity);
    }

    @Test
    public void testCreateRequestButton_doesNotCrash() {
        activity = Robolectric.buildActivity(TutorProfileActivity.class, intent)
                .create()
                .get();

        MaterialButton btnCreateRequest = activity.findViewById(R.id.btn_create_request);

        // Should not throw exception
        btnCreateRequest.performClick();

        assertNotNull("Activity should not crash after button click", activity);
    }
}

