package com.example.betreuer_app.util;

import static org.junit.Assert.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.*;

import android.content.Context;

import com.example.betreuer_app.R;
import com.example.betreuer_app.model.ThesisApiModel;
import com.example.betreuer_app.model.ThesisStatusResponse;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.MockedStatic;
import org.mockito.junit.MockitoJUnitRunner;

import java.util.List;
import java.util.UUID;

/**
 * Comprehensive unit tests for ThesisStatusDisplayLogic
 *
 * Tests cover:
 * - Student scenarios (with/without supervision request, with/without tutor)
 * - Tutor scenarios (different thesis statuses)
 * - Status transitions
 * - Edge cases and boundary conditions
 */
@RunWith(MockitoJUnitRunner.class)
public class ThesisStatusDisplayLogicTest {

    @Mock
    private Context mockContext;

    private ThesisStatusDisplayLogic logic;

    @Before
    public void setUp() {
        logic = new ThesisStatusDisplayLogic();
        setupMockStrings();
    }

    private void setupMockStrings() {
        when(mockContext.getString(R.string.status_created)).thenReturn("Erstellt");
        when(mockContext.getString(R.string.status_in_coordination)).thenReturn("In Abstimmung");
        when(mockContext.getString(R.string.status_in_discussion)).thenReturn("In Diskussion");
        when(mockContext.getString(R.string.status_registered)).thenReturn("Angemeldet");
        when(mockContext.getString(R.string.status_submitted)).thenReturn("Abgegeben");
        when(mockContext.getString(R.string.status_defended)).thenReturn("Verteidigt");
    }

    // ==================== Student Tests - No Supervision Request ====================

    @Test
    public void testStudent_NoSupervisionRequest_ShowsCreatedStatus() {
        // Given: Student with no supervision request
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", null, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);
            mockedHelper.when(() -> ThesisStatusHelper.translateStatus(any(), anyString()))
                       .thenReturn("In Diskussion");

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("Erstellt", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
            assertTrue(result.getAvailableStatuses().isEmpty());
        }
    }

    // ==================== Student Tests - Request Not Accepted ====================

    @Test
    public void testStudent_RequestNotAccepted_ShowsInCoordinationStatus() {
        // Given: Student with supervision request but not accepted
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", null, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("In Abstimmung", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
        }
    }

    @Test
    public void testStudent_RequestAcceptedButNoTutor_ShowsInCoordinationStatus() {
        // Given: Student with accepted request but tutor not assigned yet
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", null, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("In Abstimmung", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
        }
    }

    // ==================== Student Tests - IN_DISCUSSION Status ====================

    @Test
    public void testStudent_InDiscussion_WithTutor_ShowsSpinnerWithTwoOptions() {
        // Given: Student with tutor, thesis in IN_DISCUSSION
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertTrue(result.isSpinnerEnabled());
            assertNull(result.getCurrentStatusText());

            List<ThesisStatusResponse> statuses = result.getAvailableStatuses();
            assertEquals(2, statuses.size());
            assertEquals("IN_DISCUSSION", statuses.get(0).getName());
            assertEquals("In Abstimmung", statuses.get(0).getDisplayName());
            assertEquals("REGISTERED", statuses.get(1).getName());
            assertEquals("Angemeldet", statuses.get(1).getDisplayName());
        }
    }

    // ==================== Student Tests - REGISTERED Status ====================

    @Test
    public void testStudent_Registered_WithTutor_ShowsSpinnerWithTwoOptions() {
        // Given: Student with tutor, thesis REGISTERED
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("REGISTERED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertTrue(result.isSpinnerEnabled());

            List<ThesisStatusResponse> statuses = result.getAvailableStatuses();
            assertEquals(2, statuses.size());
            assertEquals("REGISTERED", statuses.get(0).getName());
            assertEquals("Angemeldet", statuses.get(0).getDisplayName());
            assertEquals("SUBMITTED", statuses.get(1).getName());
            assertEquals("Abgegeben", statuses.get(1).getDisplayName());
        }
    }

    @Test
    public void testStudent_RegisteredButNotConfirmed_TreatedAsInDiscussion() {
        // Given: Student with REGISTERED status but not confirmed
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("REGISTERED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());

            List<ThesisStatusResponse> statuses = result.getAvailableStatuses();
            assertEquals(2, statuses.size());
            // Should show IN_DISCUSSION options
            assertEquals("IN_DISCUSSION", statuses.get(0).getName());
            assertEquals("REGISTERED", statuses.get(1).getName());
        }
    }

    // ==================== Student Tests - SUBMITTED Status ====================

    @Test
    public void testStudent_Submitted_ShowsTextViewOnly() {
        // Given: Student with submitted thesis
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("SUBMITTED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);
            mockedHelper.when(() -> ThesisStatusHelper.translateStatus(mockContext, "SUBMITTED"))
                       .thenReturn("Abgegeben");

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("Abgegeben", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
            assertTrue(result.getAvailableStatuses().isEmpty());
        }
    }

    // ==================== Student Tests - DEFENDED Status ====================

    @Test
    public void testStudent_Defended_ShowsTextViewOnly() {
        // Given: Student with defended thesis
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("DEFENDED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);
            mockedHelper.when(() -> ThesisStatusHelper.translateStatus(mockContext, "DEFENDED"))
                       .thenReturn("Verteidigt");

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, true, false, true, true);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("Verteidigt", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
        }
    }

    // ==================== Tutor Tests ====================

    @Test
    public void testTutor_InDiscussion_ShowsSpinnerButDisabled() {
        // Given: Tutor viewing thesis in IN_DISCUSSION
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, false, true, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertFalse(result.isSpinnerEnabled()); // Disabled because not SUBMITTED

            List<ThesisStatusResponse> statuses = result.getAvailableStatuses();
            assertEquals(4, statuses.size());
            assertEquals("IN_DISCUSSION", statuses.get(0).getName());
            assertEquals("REGISTERED", statuses.get(1).getName());
            assertEquals("SUBMITTED", statuses.get(2).getName());
            assertEquals("DEFENDED", statuses.get(3).getName());
        }
    }

    @Test
    public void testTutor_Registered_ShowsSpinnerButDisabled() {
        // Given: Tutor viewing registered thesis
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("REGISTERED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, false, true, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertFalse(result.isSpinnerEnabled());
            assertEquals(4, result.getAvailableStatuses().size());
        }
    }

    @Test
    public void testTutor_Submitted_ShowsSpinnerAndEnabled() {
        // Given: Tutor viewing submitted thesis
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("SUBMITTED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, false, true, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertTrue(result.isSpinnerEnabled()); // Enabled because SUBMITTED
            assertEquals(4, result.getAvailableStatuses().size());
        }
    }

    @Test
    public void testTutor_Defended_ShowsSpinnerButDisabled() {
        // Given: Tutor viewing defended thesis
        UUID tutorId = UUID.randomUUID();
        ThesisApiModel thesis = createThesis("DEFENDED", tutorId, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, false, true, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result.getDisplayMode());
            assertFalse(result.isSpinnerEnabled());
            assertEquals(4, result.getAvailableStatuses().size());
        }
    }

    // ==================== Edge Cases ====================

    @Test
    public void testNeitherStudentNorTutor_ShowsDefaultTextView() {
        // Given: User is neither student nor tutor (edge case)
        ThesisApiModel thesis = createThesis("IN_DISCUSSION", null, null);

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);
            mockedHelper.when(() -> ThesisStatusHelper.translateStatus(mockContext, "IN_DISCUSSION"))
                       .thenReturn("In Diskussion");

            // When
            ThesisStatusDisplayLogic.DisplayResult result = logic.computeStatusDisplay(
                mockContext, thesis, false, false, false, false);

            // Then
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result.getDisplayMode());
            assertEquals("In Diskussion", result.getCurrentStatusText());
            assertFalse(result.isSpinnerEnabled());
        }
    }

    // ==================== Helper Method Tests ====================

    @Test
    public void testFindStatusIndex_FindsCorrectIndex() {
        // Given
        List<ThesisStatusResponse> statuses = List.of(
            new ThesisStatusResponse("IN_DISCUSSION", "In Diskussion"),
            new ThesisStatusResponse("REGISTERED", "Angemeldet"),
            new ThesisStatusResponse("SUBMITTED", "Abgegeben")
        );

        // When & Then
        assertEquals(0, logic.findStatusIndex(statuses, "IN_DISCUSSION"));
        assertEquals(1, logic.findStatusIndex(statuses, "REGISTERED"));
        assertEquals(2, logic.findStatusIndex(statuses, "SUBMITTED"));
    }

    @Test
    public void testFindStatusIndex_ReturnsMinusOneWhenNotFound() {
        // Given
        List<ThesisStatusResponse> statuses = List.of(
            new ThesisStatusResponse("IN_DISCUSSION", "In Diskussion")
        );

        // When & Then
        assertEquals(-1, logic.findStatusIndex(statuses, "DEFENDED"));
    }

    @Test
    public void testFindStatusIndex_HandlesNullList() {
        // When & Then
        assertEquals(-1, logic.findStatusIndex(null, "IN_DISCUSSION"));
    }

    @Test
    public void testFindStatusIndex_HandlesNullStatusName() {
        // Given
        List<ThesisStatusResponse> statuses = List.of(
            new ThesisStatusResponse("IN_DISCUSSION", "In Diskussion")
        );

        // When & Then
        assertEquals(-1, logic.findStatusIndex(statuses, null));
    }

    @Test
    public void testFindStatusIndex_HandlesEmptyList() {
        // Given
        List<ThesisStatusResponse> statuses = List.of();

        // When & Then
        assertEquals(-1, logic.findStatusIndex(statuses, "IN_DISCUSSION"));
    }

    // ==================== Integration Scenarios ====================

    @Test
    public void testCompleteStudentJourney_FromCreatedToSubmitted() {
        UUID tutorId = UUID.randomUUID();

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(false);
            mockedHelper.when(() -> ThesisStatusHelper.translateStatus(any(), anyString()))
                       .thenAnswer(invocation -> invocation.getArgument(1));

            // Step 1: Created, no request
            ThesisApiModel thesis1 = createThesis("IN_DISCUSSION", null, null);
            ThesisStatusDisplayLogic.DisplayResult result1 = logic.computeStatusDisplay(
                mockContext, thesis1, true, false, false, false);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result1.getDisplayMode());
            assertEquals("Erstellt", result1.getCurrentStatusText());

            // Step 2: Request sent, not accepted
            ThesisStatusDisplayLogic.DisplayResult result2 = logic.computeStatusDisplay(
                mockContext, thesis1, true, false, true, false);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result2.getDisplayMode());
            assertEquals("In Abstimmung", result2.getCurrentStatusText());

            // Step 3: Request accepted, tutor assigned, IN_DISCUSSION
            ThesisApiModel thesis2 = createThesis("IN_DISCUSSION", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result3 = logic.computeStatusDisplay(
                mockContext, thesis2, true, false, true, true);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result3.getDisplayMode());
            assertEquals(2, result3.getAvailableStatuses().size());

            // Step 4: Student registers thesis
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);
            ThesisApiModel thesis3 = createThesis("REGISTERED", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result4 = logic.computeStatusDisplay(
                mockContext, thesis3, true, false, true, true);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result4.getDisplayMode());
            assertEquals("REGISTERED", result4.getAvailableStatuses().get(0).getName());
            assertEquals("SUBMITTED", result4.getAvailableStatuses().get(1).getName());

            // Step 5: Student submits thesis
            ThesisApiModel thesis4 = createThesis("SUBMITTED", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result5 = logic.computeStatusDisplay(
                mockContext, thesis4, true, false, true, true);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.TEXT_VIEW, result5.getDisplayMode());
        }
    }

    @Test
    public void testCompleteTutorJourney_FromSubmittedToDefended() {
        UUID tutorId = UUID.randomUUID();

        try (MockedStatic<ThesisStatusHelper> mockedHelper = mockStatic(ThesisStatusHelper.class)) {
            mockedHelper.when(() -> ThesisStatusHelper.isStudentRegistrationConfirmed(any(), any()))
                       .thenReturn(true);

            // Step 1: Tutor sees IN_DISCUSSION - cannot edit
            ThesisApiModel thesis1 = createThesis("IN_DISCUSSION", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result1 = logic.computeStatusDisplay(
                mockContext, thesis1, false, true, false, false);
            assertEquals(ThesisStatusDisplayLogic.DisplayMode.SPINNER, result1.getDisplayMode());
            assertFalse(result1.isSpinnerEnabled());

            // Step 2: Tutor sees REGISTERED - cannot edit
            ThesisApiModel thesis2 = createThesis("REGISTERED", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result2 = logic.computeStatusDisplay(
                mockContext, thesis2, false, true, false, false);
            assertFalse(result2.isSpinnerEnabled());

            // Step 3: Tutor sees SUBMITTED - can edit
            ThesisApiModel thesis3 = createThesis("SUBMITTED", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result3 = logic.computeStatusDisplay(
                mockContext, thesis3, false, true, false, false);
            assertTrue(result3.isSpinnerEnabled());

            // Step 4: Tutor marks as DEFENDED - cannot edit anymore
            ThesisApiModel thesis4 = createThesis("DEFENDED", tutorId, null);
            ThesisStatusDisplayLogic.DisplayResult result4 = logic.computeStatusDisplay(
                mockContext, thesis4, false, true, false, false);
            assertFalse(result4.isSpinnerEnabled());
        }
    }

    // ==================== Test Helper Methods ====================

    private ThesisApiModel createThesis(String status, UUID tutorId, UUID secondSupervisorId) {
        ThesisApiModel thesis = new ThesisApiModel();
        thesis.setId(UUID.randomUUID());
        thesis.setStatus(status);
        thesis.setTutorId(tutorId);
        thesis.setSecondSupervisorId(secondSupervisorId);
        thesis.setOwnerId(UUID.randomUUID());
        thesis.setTitle("Test Thesis");
        thesis.setDescription("Test Description");
        return thesis;
    }
}
