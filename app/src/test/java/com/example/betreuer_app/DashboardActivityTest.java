package com.example.betreuer_app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.betreuer_app.model.ThesesResponse;
import com.example.betreuer_app.repository.ThesisRepository;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.card.MaterialCardView;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.stubbing.Answer;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowActivity;
import org.robolectric.shadows.ShadowLooper;

import retrofit2.Callback;
import retrofit2.Response;

import java.util.Collections;

import static org.junit.Assert.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.robolectric.Shadows.shadowOf;

@RunWith(RobolectricTestRunner.class)
@Config(sdk = 24, manifest = Config.NONE)
public class DashboardActivityTest {

    private DashboardActivity activity;
    private Intent intent;

    @Mock
    private ThesisRepository mockThesisRepository;

    // Subclass to inject the mock repository
    public static class TestDashboardActivity extends DashboardActivity {
        public static ThesisRepository mockRepository;

        @Override
        protected ThesisRepository createThesisRepository() {
            return mockRepository;
        }
    }

    @Before
    public void setUp() {
        MockitoAnnotations.openMocks(this);
        intent = new Intent();
        TestDashboardActivity.mockRepository = mockThesisRepository;

        // Default mock behavior for getTheses to avoid NPEs in callbacks if called
        doAnswer(invocation -> {
            Callback<ThesesResponse> callback = invocation.getArgument(2);
            // Default success response
            ThesesResponse response = new ThesesResponse(Collections.emptyList(), 0, 1, 10);
            callback.onResponse(null, Response.success(response));
            return null;
        }).when(mockThesisRepository).getTheses(anyInt(), anyInt(), any());
    }

    @Test
    public void testActivityCreation_withoutUserData() {
        // Use TestDashboardActivity instead of DashboardActivity
        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .get();

        assertNotNull("Activity should be created", activity);

        MaterialToolbar toolbar = activity.findViewById(R.id.toolbar);
        assertNotNull("Toolbar should be initialized", toolbar);
    }

    @Test
    public void testWelcomeMessage_withUserName() {
        intent.putExtra("USER_NAME", "Max Mustermann");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        TextView welcomeTextView = activity.findViewById(R.id.welcomeTextView);
        assertNotNull("Welcome TextView should exist", welcomeTextView);
        assertEquals("Welcome message should display user name",
                "Hallo Max Mustermann!", welcomeTextView.getText().toString());
    }

    @Test
    public void testWelcomeMessage_withoutUserName() {
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        TextView welcomeTextView = activity.findViewById(R.id.welcomeTextView);
        assertNotNull("Welcome TextView should exist", welcomeTextView);
        // Should have default text from layout or just be "Hallo null!" or similar if logic allows,
        // but code checks if (userName != null).
    }

    @Test
    public void testStudentView_isInflated() {
        intent.putExtra("USER_NAME", "Student User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        // Check that student-specific views are present
        Button btnCreateNewThesis = activity.findViewById(R.id.btn_create_new_thesis);
        Button btnFindTutor = activity.findViewById(R.id.btn_find_tutor);
        Button btnPendingRequests = activity.findViewById(R.id.btn_pending_requests);
        MaterialCardView studentThesisCard = activity.findViewById(R.id.student_thesis_card);
        TextView studentThesisCountTextView = activity.findViewById(R.id.studentThesisCountTextView);

        assertNotNull("Create new thesis button should exist", btnCreateNewThesis);
        assertNotNull("Find tutor button should exist", btnFindTutor);
        assertNotNull("Pending requests button should exist", btnPendingRequests);
        assertNotNull("Student thesis card should exist", studentThesisCard);
        assertNotNull("Student thesis count text should exist", studentThesisCountTextView);
    }

    @Test
    public void testLecturerView_isInflated() {
        intent.putExtra("USER_NAME", "Lecturer User");
        intent.putExtra("USER_ROLE", "tutor");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        // Check that lecturer-specific views are present
        Button btnManageThesisOffers = activity.findViewById(R.id.btn_manage_thesis_offers);
        MaterialCardView lecturerThesisCard = activity.findViewById(R.id.lecturer_thesis_card);
        MaterialCardView lecturerRequestsCard = activity.findViewById(R.id.lecturer_requests_card);
        TextView lecturerThesisCountTextView = activity.findViewById(R.id.lecturerThesisCountTextView);
        TextView lecturerRequestsCountTextView = activity.findViewById(R.id.lecturerRequestsCountTextView);

        assertNotNull("Manage thesis offers button should exist", btnManageThesisOffers);
        assertNotNull("Lecturer thesis card should exist", lecturerThesisCard);
        assertNotNull("Lecturer requests card should exist", lecturerRequestsCard);
        assertNotNull("Lecturer thesis count text should exist", lecturerThesisCountTextView);
        assertNotNull("Lecturer requests count text should exist", lecturerRequestsCountTextView);
    }

    @Test
    public void testStudentCreateThesisButton_startsCorrectActivity() {
        intent.putExtra("USER_NAME", "Student User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnCreateNewThesis = activity.findViewById(R.id.btn_create_new_thesis);
        btnCreateNewThesis.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start StudentCreateThesisActivity",
                StudentCreateThesisActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testStudentFindTutorButton_startsCorrectActivity() {
        intent.putExtra("USER_NAME", "Student User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnFindTutor = activity.findViewById(R.id.btn_find_tutor);
        btnFindTutor.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start TutorListActivity",
                TutorListActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testStudentPendingRequestsButton_startsCorrectActivity() {
        intent.putExtra("USER_NAME", "Student User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnPendingRequests = activity.findViewById(R.id.btn_pending_requests);
        btnPendingRequests.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisRequestActivity",
                ThesisRequestActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testStudentThesisCard_startsThesisListActivity() {
        intent.putExtra("USER_NAME", "Student User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        MaterialCardView studentThesisCard = activity.findViewById(R.id.student_thesis_card);
        studentThesisCard.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisListActivity",
                ThesisListActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testLecturerManageThesisOffersButton_startsCorrectActivity() {
        intent.putExtra("USER_NAME", "Lecturer User");
        intent.putExtra("USER_ROLE", "tutor");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnManageThesisOffers = activity.findViewById(R.id.btn_manage_thesis_offers);
        btnManageThesisOffers.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisOfferDashboardActivity",
                ThesisOfferDashboardActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testLecturerThesisCard_startsThesisListActivity() {
        intent.putExtra("USER_NAME", "Lecturer User");
        intent.putExtra("USER_ROLE", "tutor");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        MaterialCardView lecturerThesisCard = activity.findViewById(R.id.lecturer_thesis_card);
        lecturerThesisCard.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisListActivity",
                ThesisListActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testLecturerRequestsCard_startsThesisRequestActivity() {
        intent.putExtra("USER_NAME", "Lecturer User");
        intent.putExtra("USER_ROLE", "tutor");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        MaterialCardView lecturerRequestsCard = activity.findViewById(R.id.lecturer_requests_card);
        lecturerRequestsCard.performClick();

        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start ThesisRequestActivity",
                ThesisRequestActivity.class.getName(),
                startedIntent.getComponent().getClassName());
    }

    @Test
    public void testOptionsMenu_isCreated() {
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Menu menu = shadowOf(activity).getOptionsMenu();
        activity.onCreateOptionsMenu(menu);

        assertNotNull("Options menu should be created", menu);
    }

    @Test
    public void testLogoutMenuItem_triggersLogout() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        MenuItem mockMenuItem = mock(MenuItem.class);
        when(mockMenuItem.getItemId()).thenReturn(R.id.action_logout);

        boolean handled = activity.onOptionsItemSelected(mockMenuItem);

        assertTrue("Logout menu item should be handled", handled);

        // Check that LoginActivity is started
        ShadowActivity shadowActivity = shadowOf(activity);
        Intent startedIntent = shadowActivity.getNextStartedActivity();

        assertNotNull("Intent should be started", startedIntent);
        assertEquals("Should start LoginActivity",
                LoginActivity.class.getName(),
                startedIntent.getComponent().getClassName());

        // Check that flags are set correctly
        int flags = startedIntent.getFlags();
        assertTrue("Should have NEW_TASK flag",
                (flags & Intent.FLAG_ACTIVITY_NEW_TASK) != 0);
        assertTrue("Should have CLEAR_TASK flag",
                (flags & Intent.FLAG_ACTIVITY_CLEAR_TASK) != 0);

        // Check that activity is finishing
        assertTrue("Activity should be finishing", activity.isFinishing());
    }

    @Test
    public void testLogout_clearsSharedPreferences() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        // Set some auth preferences
        SharedPreferences authPrefs = activity.getSharedPreferences("auth_prefs", AppCompatActivity.MODE_PRIVATE);
        authPrefs.edit().putString("token", "test_token").apply();

        // Verify preference is set
        assertNotNull("Token should be set", authPrefs.getString("token", null));

        MenuItem mockMenuItem = mock(MenuItem.class);
        when(mockMenuItem.getItemId()).thenReturn(R.id.action_logout);

        activity.onOptionsItemSelected(mockMenuItem);

        // Verify preferences are cleared
        assertNull("Token should be cleared", authPrefs.getString("token", null));
    }

    @Test
    public void testUnknownMenuItem_returnsSuper() {
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        MenuItem mockMenuItem = mock(MenuItem.class);
        when(mockMenuItem.getItemId()).thenReturn(android.R.id.home);

        boolean handled = activity.onOptionsItemSelected(mockMenuItem);

        // Should return false as it's not handled by the activity
        assertFalse("Unknown menu item should not be handled", handled);
    }

    @Test
    public void testRoleEquality_caseInsensitive_student() {
        intent.putExtra("USER_ROLE", "STUDENT");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnCreateNewThesis = activity.findViewById(R.id.btn_create_new_thesis);
        assertNotNull("Student view should be inflated even with uppercase role", btnCreateNewThesis);
    }

    @Test
    public void testRoleEquality_caseInsensitive_tutor() {
        intent.putExtra("USER_ROLE", "TUTOR");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        Button btnManageThesisOffers = activity.findViewById(R.id.btn_manage_thesis_offers);
        assertNotNull("Lecturer view should be inflated even with uppercase role", btnManageThesisOffers);
    }

    @Test
    public void testOnResume_callsLoadDashboardData() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .start()
                .resume()
                .get();

        // After resume, loadDashboardData should have been called
        // We can verify this by checking if the repository method was called
        verify(mockThesisRepository).getTheses(anyInt(), anyInt(), any());

        assertNotNull("Activity should be in resumed state", activity);
    }

    @Test
    public void testNullUserRole_doesNotCrash() {
        intent.putExtra("USER_NAME", "Test User");
        // No USER_ROLE extra

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        assertNotNull("Activity should handle null role gracefully", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testEmptyUserRole_doesNotCrash() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        assertNotNull("Activity should handle empty role gracefully", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testInvalidUserRole_doesNotCrash() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "invalid_role");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .resume()
                .get();

        assertNotNull("Activity should handle invalid role gracefully", activity);
        assertFalse("Activity should not be finishing", activity.isFinishing());
    }

    @Test
    public void testToolbar_isSetAsActionBar() {
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .get();

        assertNotNull("Action bar should be set", activity.getSupportActionBar());
    }

    @Test
    public void testActivityLifecycle_createStartResumeStop() {
        intent.putExtra("USER_NAME", "Test User");
        intent.putExtra("USER_ROLE", "student");

        activity = Robolectric.buildActivity(TestDashboardActivity.class, intent)
                .create()
                .start()
                .resume()
                .pause()
                .stop()
                .get();

        assertNotNull("Activity should survive lifecycle changes", activity);
    }
}
