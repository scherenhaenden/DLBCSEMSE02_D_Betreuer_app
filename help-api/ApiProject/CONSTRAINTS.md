# System Constraints & Business Rules

This document outlines the constraints and business rules enforced within the Thesis Management API.

---

## 1. User Management

1. **Email Uniqueness**
   Every user must have a unique email address.

2. **Role Assignment**
   A user must be assigned at least one role (`STUDENT`, `TUTOR`, `ADMIN`) upon creation.

3. **Role Immutability**
   While users can have multiple roles, specific actions are strictly gated by these roles (see below).

---

## 2. Subject Areas (Supervision Areas)

1. **Definition**
   A `SubjectArea` represents a supervision or expertise area (e.g. databases, AI, software engineering).

2. **Tutor Assignment**
   Only users with the `TUTOR` role may be assigned to a `SubjectArea`.

3. **Expertise Constraint**
   A Tutor may only supervise theses or offer topics within the `SubjectAreas` they are explicitly assigned to.

---

## 3. Thesis Offers (Offered Topics)

1. **Ownership**
   A `ThesisOffer` can only be created by a user with the `TUTOR` role.

2. **Subject Area Binding**
   Every `ThesisOffer` must be associated with exactly one `SubjectArea`.

3. **Status Lifecycle**
   A thesis offer follows a strict lifecycle:

   ```
   OPEN → CLOSED → ARCHIVED
   ```

4. **Availability Constraint**
   Only offers with status `OPEN` may accept new applications.

5. **Capacity Constraint**
   If `MaxStudents` is defined, the number of accepted applications must not exceed this value.

6. **Expiration Constraint**
   If `ExpiresAt` is set, the offer must not be selectable after the expiration date.

---

## 4. Thesis Offer Applications

1. **Application Without Thesis**
   A `ThesisOfferApplication` represents a student’s application to an offer **before** any thesis is created.

2. **Role Constraints**

   * **Applicant**: Must have the `STUDENT` role.
   * **Receiver (implicitly)**: The Tutor who owns the offer.

3. **Uniqueness**
   A student may apply **only once** to the same thesis offer.

4. **Application Status Lifecycle**

   ```
   PENDING → ACCEPTED | REJECTED | WITHDRAWN
   ```

5. **Acceptance Effect**

   * Accepting an application must:

     * Create a new `Thesis`
     * Assign the Student as `Owner`
     * Assign the Tutor as `Tutor`
     * Link the Thesis to the originating `ThesisOffer`
     * Transition the offer to `CLOSED` if capacity is reached

---

## 5. Thesis Management

1. **Ownership**
   Only a user with the `STUDENT` role can be the `Owner` of a thesis.

2. **Supervision**
   Only a user with the `TUTOR` role can be assigned as `Tutor` (Main Supervisor) or `SecondSupervisor`.

3. **Subject Area Consistency**
   A Tutor can only supervise a thesis if they are assigned to the thesis’s `SubjectArea`.

4. **Supervisor Distinctness**
   The `Tutor` (Main Supervisor) and `SecondSupervisor` must be two different users.

5. **Creation Constraint**
   A thesis may only be created:

   * via acceptance of a `ThesisOfferApplication`, or
   * via an explicit student-initiated request workflow.

6. **Status Workflow**
   A thesis follows a strict lifecycle:

   ```
   IN_DISCUSSION → REGISTERED → SUBMITTED → DEFENDED
   ```

7. **Deletion**
   A thesis can only be deleted if it is in the `IN_DISCUSSION` state.

---

## 6. Thesis Requests (Workflow)

1. **Request Necessity**
   A Tutor cannot be assigned to a thesis directly without a corresponding request or application acceptance.

2. **Audit Trail – Supervision**
   If a Thesis has a `TutorId` assigned, there **MUST** exist either:

   * an `ACCEPTED` `ThesisOfferApplication`, or
   * a `ThesisRequest` of type `SUPERVISION` with status `ACCEPTED`.

3. **Audit Trail – Co-Supervision**
   If a Thesis has a `SecondSupervisorId` assigned, there **MUST** exist a `ThesisRequest` of type `CO_SUPERVISION` with status `ACCEPTED`.

4. **Request Flow – Supervision**

   * **Requester**: Must be the Thesis Owner (`STUDENT`)
   * **Receiver**: Must be a `TUTOR` covering the thesis subject area
   * **Type**: `SUPERVISION`

5. **Request Flow – Co-Supervision**

   * **Requester**: Must be the current Main Tutor
   * **Receiver**: Must be a different `TUTOR`
   * **Type**: `CO_SUPERVISION`

6. **State Synchronization**
   Accepting a request must update the related thesis fields within the same transaction.

---

## 7. Data Access & Visibility

1. **Student Visibility**
   A Student can only view:

   * their own theses
   * their own thesis offer applications

2. **Tutor Visibility**
   A Tutor can only view:

   * theses where they are `Tutor` or `SecondSupervisor`
   * offers they created
   * applications submitted to their offers

3. **Admin Visibility**
   Admins have global visibility over all users, offers, applications, and theses.

4. **Tutor Discovery**
   Students can search for Tutors and Thesis Offers, filtered by `SubjectArea`.

---

## 8. Database Integrity

1. **Foreign Keys**
   Strict referential integrity is enforced for all relationships (Users, Roles, SubjectAreas, Offers, Applications, Theses).

2. **Delete Behavior**

   * Deleting a User is restricted if dependent records exist.
   * Deleting a ThesisOffer is restricted if applications exist.
   * Deleting a Thesis cascades to its Requests and Documents (or is soft-deleted depending on implementation).
