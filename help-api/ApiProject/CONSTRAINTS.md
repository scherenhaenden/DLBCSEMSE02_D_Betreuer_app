# System Constraints & Business Rules

This document outlines the constraints and business rules enforced within the Thesis Management API.

## 1. User Management
1.  **Email Uniqueness**: Every user must have a unique email address.
2.  **Role Assignment**: A user must be assigned at least one role (`STUDENT`, `TUTOR`, `ADMIN`) upon creation.
3.  **Role Immutability**: While users can have multiple roles, specific actions are strictly gated by these roles (see below).

## 2. Thesis Management
1.  **Ownership**: Only a user with the `STUDENT` role can be the `Owner` of a thesis.
2.  **Supervision**: Only a user with the `TUTOR` role can be assigned as `Tutor` (Main Supervisor) or `SecondSupervisor`.
3.  **Topic Expertise**: A Tutor can only be assigned to a thesis if they are explicitly assigned to cover the thesis's `Topic`.
4.  **Supervisor Distinctness**: The `Tutor` (Main Supervisor) and `SecondSupervisor` must be two different users.
5.  **Status Workflow**: A thesis follows a strict lifecycle: `IN_DISCUSSION` -> `REGISTERED` -> `SUBMITTED` -> `DEFENDED`.
6.  **Deletion**: A thesis can only be deleted if it is in the `IN_DISCUSSION` or `PENDING_APPROVAL` state (soft constraint, usually enforced by business logic).

## 3. Thesis Requests (Workflow)
1.  **Request Necessity**: A Tutor cannot be assigned to a thesis directly without a corresponding `ThesisRequest` record.
2.  **Audit Trail**: If a Thesis has a `TutorId` assigned, there **MUST** exist a `ThesisRequest` of type `SUPERVISION` with status `ACCEPTED` linking that Student and Tutor for that Thesis.
3.  **Co-Supervision Audit**: If a Thesis has a `SecondSupervisorId` assigned, there **MUST** exist a `ThesisRequest` of type `CO_SUPERVISION` with status `ACCEPTED` linking the Main Tutor and the Second Supervisor.
4.  **Request Flow - Supervision**:
    *   **Requester**: Must be the Thesis Owner (`STUDENT`).
    *   **Receiver**: Must be a `TUTOR` covering the thesis topic.
    *   **Type**: `SUPERVISION`.
5.  **Request Flow - Co-Supervision**:
    *   **Requester**: Must be the current Main Tutor of the thesis.
    *   **Receiver**: Must be a different `TUTOR`.
    *   **Type**: `CO_SUPERVISION`.
6.  **State Synchronization**: Accepting a request (`ACCEPTED`) must automatically update the corresponding field (`TutorId` or `SecondSupervisorId`) in the `Theses` table within the same transaction.

## 4. Data Access & Visibility
1.  **Student Visibility**: A Student can only view theses where they are the `Owner`.
2.  **Tutor Visibility**: A Tutor can only view theses where they are assigned as `Tutor` or `SecondSupervisor`.
3.  **Admin Visibility**: Admins have global visibility over all theses and users.
4.  **Tutor Discovery**: Students can search for Tutors, but the results should be filtered/prioritized by the Topic they are interested in.

## 5. Database Integrity
1.  **Foreign Keys**: Strict referential integrity is maintained for all relationships (Users, Roles, Topics, Theses).
2.  **Delete Behavior**:
    *   Deleting a User is restricted if they have dependent records (Theses, Requests).
    *   Deleting a Thesis cascades to its Requests and Documents (or soft-deletes depending on implementation).
