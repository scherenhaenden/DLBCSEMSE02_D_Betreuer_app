package com.example.betreuer_app;

import android.content.Context;
import android.content.SharedPreferences;
import android.widget.EditText;
import android.widget.Button;
import android.widget.ProgressBar;
import android.view.View;

import com.example.betreuer_app.constants.AuthConstants;
import com.example.betreuer_app.constants.ThemeConstants;
import com.google.android.material.switchmaterial.SwitchMaterial;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowLooper;
import org.robolectric.shadows.ShadowToast;

import static org.junit.Assert.*;

@RunWith(RobolectricTestRunner.class)
@Config(sdk = 24, manifest = Config.NONE)
public class LoginActivityTest {

    private LoginActivity activity;

    @Before
    public void setUp() {
        activity = Robolectric.buildActivity(LoginActivity.class).create().get();
    }

    @Test
    public void testInitializeViews_allViewsAreInitialized() {
        // Test that all views are properly initialized
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);
        ProgressBar progressBar = activity.findViewById(R.id.progressBar);
        SwitchMaterial themeSwitch = activity.findViewById(R.id.themeSwitch);

        assertNotNull("Email EditText should be initialized", emailEditText);
        assertNotNull("Password EditText should be initialized", passwordEditText);
        assertNotNull("Login Button should be initialized", loginButton);
        assertNotNull("ProgressBar should be initialized", progressBar);
        assertNotNull("Theme Switch should be initialized", themeSwitch);
    }

    @Test
    public void testLoginButtonClick_withEmptyEmail_showsToast() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("");
        passwordEditText.setText("password123");

        loginButton.performClick();

        String toastText = ShadowToast.getTextOfLatestToast();
        assertEquals("Please enter email and password", toastText);
    }

    @Test
    public void testLoginButtonClick_withEmptyPassword_showsToast() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("test@example.com");
        passwordEditText.setText("");

        loginButton.performClick();

        String toastText = ShadowToast.getTextOfLatestToast();
        assertEquals("Please enter email and password", toastText);
    }

    @Test
    public void testLoginButtonClick_withBothFieldsEmpty_showsToast() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("");
        passwordEditText.setText("");

        loginButton.performClick();

        String toastText = ShadowToast.getTextOfLatestToast();
        assertEquals("Please enter email and password", toastText);
    }

    @Test
    public void testLoginButtonClick_withValidInputs_showsProgressBar() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);
        ProgressBar progressBar = activity.findViewById(R.id.progressBar);

        emailEditText.setText("test@example.com");
        passwordEditText.setText("password123");

        loginButton.performClick();

        // Progress bar should be visible when login is initiated
        assertEquals(View.VISIBLE, progressBar.getVisibility());
    }

    @Test
    public void testLoginButtonClick_withValidInputs_disablesButton() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("test@example.com");
        passwordEditText.setText("password123");

        assertTrue("Login button should be enabled initially", loginButton.isEnabled());

        loginButton.performClick();

        // Button should be disabled during login
        assertFalse("Login button should be disabled during login", loginButton.isEnabled());
    }

    @Test
    public void testValidateInputs_withValidInputs_returnsTrue() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("test@example.com");
        passwordEditText.setText("password123");

        loginButton.performClick();

        // No toast should be shown for valid inputs
        String toastText = ShadowToast.getTextOfLatestToast();
        assertNotEquals("Please enter email and password", toastText);
    }

    @Test
    public void testThemeSwitch_initialState_matchesSharedPreferences() {
        SwitchMaterial themeSwitch = activity.findViewById(R.id.themeSwitch);
        SharedPreferences prefs = activity.getSharedPreferences(ThemeConstants.PREFS_NAME, Context.MODE_PRIVATE);
        boolean isDarkMode = prefs.getBoolean(ThemeConstants.KEY_IS_DARK_MODE, false);

        assertEquals(isDarkMode, themeSwitch.isChecked());
    }

    @Test
    public void testThemeSwitch_whenToggled_savesToSharedPreferences() {
        SwitchMaterial themeSwitch = activity.findViewById(R.id.themeSwitch);

        boolean initialState = themeSwitch.isChecked();
        themeSwitch.setChecked(!initialState);

        SharedPreferences prefs = activity.getSharedPreferences(ThemeConstants.PREFS_NAME, Context.MODE_PRIVATE);
        boolean savedState = prefs.getBoolean(ThemeConstants.KEY_IS_DARK_MODE, false);

        assertEquals(!initialState, savedState);
    }

    @Test
    public void testShowLogin_clearsSharedPreferences() {
        // Save some data first
        SharedPreferences authPreferences = activity.getSharedPreferences(AuthConstants.PREFS_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = authPreferences.edit();
        editor.putString(AuthConstants.KEY_JWT_TOKEN, "test_token");
        editor.putString(AuthConstants.KEY_USER_NAME, "Test User");
        editor.putString(AuthConstants.KEY_USER_ROLE, "STUDENT");
        editor.apply();

        // Trigger showLogin by having invalid credentials in auto-login
        // Recreate activity to run onCreate and checkAutoLogin
        activity.finish();
        activity = Robolectric.buildActivity(LoginActivity.class).create().get();

        // Run main thread tasks to allow Retrofit callback (onFailure) to execute
        ShadowLooper.idleMainLooper();

        // Check that authentication data is cleared
        String token = authPreferences.getString(AuthConstants.KEY_JWT_TOKEN, null);
        assertNull("Token should be cleared after auto-login failure", token);
    }

    @Test
    public void testProgressBar_initiallyGone() {
        ProgressBar progressBar = activity.findViewById(R.id.progressBar);
        // After onCreate completes and no auto-login data exists
        // Progress bar should eventually be gone
        assertEquals(View.GONE, progressBar.getVisibility());
    }

    @Test
    public void testLoginButton_initiallyVisible() {
        Button loginButton = activity.findViewById(R.id.loginButton);
        assertEquals(View.VISIBLE, loginButton.getVisibility());
    }

    @Test
    public void testEmailEditText_initiallyVisible() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        assertEquals(View.VISIBLE, emailEditText.getVisibility());
    }

    @Test
    public void testPasswordEditText_initiallyVisible() {
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        assertEquals(View.VISIBLE, passwordEditText.getVisibility());
    }

    @Test
    public void testThemeSwitch_initiallyVisible() {
        SwitchMaterial themeSwitch = activity.findViewById(R.id.themeSwitch);
        assertEquals(View.VISIBLE, themeSwitch.getVisibility());
    }

    @Test
    public void testValidateInputs_withWhitespaceOnlyEmail_showsToast() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("   ");
        passwordEditText.setText("password123");

        loginButton.performClick();

        String toastText = ShadowToast.getTextOfLatestToast();
        assertEquals("Please enter email and password", toastText);
    }

    @Test
    public void testValidateInputs_withWhitespaceOnlyPassword_showsToast() {
        EditText emailEditText = activity.findViewById(R.id.emailEditText);
        EditText passwordEditText = activity.findViewById(R.id.passwordEditText);
        Button loginButton = activity.findViewById(R.id.loginButton);

        emailEditText.setText("test@example.com");
        passwordEditText.setText("   ");

        loginButton.performClick();

        String toastText = ShadowToast.getTextOfLatestToast();
        assertEquals("Please enter email and password", toastText);
    }
}
