### **Einheitstabelle ohne Gruppierung**

| **User-Rolle** | **Use Case / Als User kann ich…**                     | **Screen/Entry Point**                          | **Status** | **Beleg aus Code/Views**                                   |
|----------------|------------------------------------------------------|-------------------------------------------------|-----------|------------------------------------------------------------|
| **Alle**       | mich mit E-Mail/Passwort anmelden                    | `LoginActivity`                                 | ✅         | `LoginActivity`, `LoginRepository` **[Gemacht]**          |
| **Alle**       | das Dark/Light-Thema ändern und in SP speichern      | `LoginActivity` (Theme-Switch)                  | ✅         | `ThemeConstants`, `BetreuerApp` **(App-Initialisierung)** **[Gemacht]** |
| **Alle**       | automatisch wieder anmelden                          | Splash/App-Start                                | ✅         | `LoginActivity.checkAutoLogin()` **[Gemacht]**            |
| **Alle**       | mich abmelden                                       | Dashboard-Menü                                  | ✅         | `DashboardActivity.logout()` **[Gemacht]**                |
| **Student**    | Button mit Anzahl der Abschlussarbeiten sehen (Klick öffnet Liste) | `DashboardActivity` (Student-Ansicht)           | ✅         | `student_thesis_card`, `studentThesisCountTextView` in `view_dashboard_student.xml` **[Gemacht]** |
| **Student**    | Liste der Abschlussarbeiten sehen (Klick öffnet Details) | `ThesisListActivity`                            | ✅         | `ThesisListFragment`, `ThesisListAdapter` (onItemClick -> Detail) **[Gemacht]** |
| **Student**    | Details der Abschlussarbeit sehen (Status, Rechnung, PDF laden, Bearbeiten) | `ThesisDetailActivity`                          | ✅         | `ThesisDetailActivity`, `btn_download_document`, `btn_edit_thesis` **[Gemacht]** |
| **Student**    | Abschlussarbeit bearbeiten (Dokument laden, Betreuer finden, Speichern) | `EditThesisActivity`                            | ✅         | `EditThesisActivity`, `btn_upload_document`, `btn_find_tutors`, `btn_save_thesis` **[Gemacht]** |
| **Betreuer**   | meine Thesenangebote verwalten                        | `DashboardActivity` (Betreuer-Ansicht)          | ❌         | `dashboard_lecturer.xml`, `DashboardActivity` **[Gemacht]** |
| **Student**    | Tutoren suchen                                       | `DashboardActivity` (Student)                   | ✅         | `btn_find_tutor` in `dashboard_student.xml` **[Gemacht]** |
| **Betreuer**   | offene Anfragen verwalten                           | `DashboardActivity` (Betreuer)                  | ❌         | `btn_pending_requests`, `lecturer_requests_card` **[Gemacht]** |
| **Student**    | meine Anfragen einsehen                              | `ThesisRequestActivity`                         | ✅         | `recyclerViewRequests` in `activity_thesis_request.xml` **[Gemacht]** |
| **Betreuer**   | neue Thesenausschreibungen erstellen                 | `ThesisOfferDashboardActivity`                  | ❌         | `fab_add_thesis_offer` in `activity_thesis_offer_dashboard.xml` **[Gemacht]** |
| **Betreuer**   | Thesenangebote in einer Liste sehen                  | `ThesisOfferDashboardActivity`                  | ❌         | `rv_thesis_offers`, `ThesisOfferAdapter` **[Gemacht (UI)]** |
| **Betreuer**   | neue Ausschreibung erstellen                         | `CreateThesisOfferActivity`                     | ❌         | `et_thesis_title`, `subject_area_dropdown` **[Gemacht (UI)]** |
| **Betreuer**   | Ausschreibung bearbeiten                             | `CreateThesisOfferActivity` (Edit)              | ❌         | `status_dropdown_layout` (EditMode) **[Gemacht (UI)]**     |
| **Betreuer**   | Rechnungsstatus ändern                               | `ThesisDetailActivity`                          | ❌         | `spinner_billingstatus`, `updateBillingStatus()` **[Gemacht]** |
| **Betreuer**   | Anfragen akzeptieren                                 | `ThesisRequestActivity`                         | ❌         | `buttonAccept` in `item_thesis_request.xml` **[Gemacht]** |
| **Betreuer**   | Anfragen ablehnen                                   | `ThesisRequestActivity`                         | ❌         | `buttonReject` in `item_thesis_request.xml` **[Gemacht]** |
| **Betreuer**   | Ausschreibung löschen                                | `ThesisOfferDashboardActivity`                  | ❌         | **Kein UI-Element, keine Logik gefunden** **[Fehlt]**     |
| **Betreuer**   | Max. Teilnehmer pro Thema festlegen                 | `CreateThesisOfferActivity`                     | ❌         | `maxStudents` im Modell, aber nicht in UI **[Fehlt]**     |
| **Student**    | Tutoren mit Fachrichtung filtern                    | `TutorListActivity`                             | ❌         | `fach_chip_group`, `subject_area_chip_group` **[Gemacht (UI)]** |
| **Student**    | Tutorenprofil ansehen                               | `TutorProfileActivity`                          | ❌         | Profil-Layout existiert, aber Details unvollständig **[Gemacht (UI)]** |
| **Student**    | Thema bei Tutor anfragen                            | `SupervisionRequestFragment`                    | ✅         | `send_request_button`, `et_student_message` **[Gemacht (UI)]** |
| **Student**    | Anfrage zur Revision senden (View öffnen)           | `RevisionRequestActivity`                       | ✅         | `btn_open_revision_request` in `ThesisDetailActivity` **[Gemacht]** |
| **Student**    | Anfrage an Tutor für Revision senden (funktioniert) | `RevisionRequestFragment`                       | ✅         | `send_revision_request_button`, `RevisionRequestService` **[Gemacht]** |
| **Student**    | Abschlussarbeit erstellen                           | `StudentCreateThesisActivity`                   | ✅         | `et_thesis_title`, `btn_create_thesis` **[Gemacht (UI)]** |
| **Student**    | Abschlussarbeit bearbeiten                          | `EditThesisActivity`                            | ❌         | `et_thesis_title`, `dropdown_subject_area` **[Gemacht (UI)]** |
| **Student**    | Dokument zur Abschlussarbeit hochladen               | `EditThesisActivity` / `DocumentUploadFragment` | ✅ | `btn_upload_document`, `documents_recycler_view` **[Gemacht (UI)]** |
| **Student**    | Dokument herunterladen                               | `ThesisDetailActivity`                          | ✅         | `btn_download_document`, `downloadThesisDocument()` **[Gemacht]** |
| **Betreuer**   | mein Zweitgutachter suchen                          | `SecondSupervisorFragment`                      | ❌         | `view_dashboard_student.xml` enthält UI-Element **[Gemacht (UI)]** |
| **Student**    | meine offenen Anfragen löschen                       | `ThesisRequestActivity`                         | ❌         | UI-Element `btn_cancel` vorhanden, Logik nicht **[Fehlt]** |
| **Student**    | Thema über Schwarzes Brett suchen                    | `ThesisSearchActivity`                          | ❌         | `search_text_input`, `thesis_recycler_view` **[Gemacht (UI)]** |
| **Student**    | Thema an einem Tutor anfragen                       | `ThesisRequestActivity`                         | ✅         | `RespondToThesisRequestRequest` in API **[Gemacht (Flow)]** |
| **Betreuer**   | Dokument herunterladen                               | `ThesisDetailActivity`                          | ✅         | `btn_download_document`, `downloadThesisDocument()` **[Gemacht]** |
| **Betreuer**   | Dokument sortiert anzeigen                          | `DocumentUploadFragment`                        | ❌         | `sort_spinner`, `sortAufsteigend()` **[Gemacht]**        |
| **Betreuer**   | Dokument hochladen                                   | `DocumentUploadFragment`                        | ❌         | `upload_fab` in `fragment_document_upload.xml` **[Gemacht (UI)]** |
| **Betreuer**   | Dokument per API anlegen                            | Backend                                         | ❌         | `ThesisDocumentResponse`, `updateThesisDocument()` **[Gemacht]** |
| **Betreuer**   | Abschlussarbeit ansehen                             | `ThesisDetailActivity`                          | ❌         | `textViewTitel`, `textViewDescription` **[Gemacht]**     |
| **Betreuer**   | Rechnungsstatus aktualisieren                      | `ThesisDetailActivity`                          | ❌         | `spinner_billingstatus`, `updateBillingStatus()` **[Gemacht]** |
| **Betreuer**   | Thesis-Offers per API filtern                       | Backend                                         | ❌         | API-Endpoints für `subjectAreaId`, `tutorId` vorhanden **[Gemacht]** |
| **Betreuer**   | Thema über Schwarzes Brett suchen                    | `ThesisSearchActivity`                          | ❌         | `search_text_input`, `thesis_recycler_view` **[Gemacht (UI)]** |
| **Betreuer**   | Thema an einem Tutor anfragen                       | `ThesisRequestActivity`                         | ❌         | `RespondToThesisRequestRequest` in API **[Gemacht (Flow)]** |
| **Alle**       | mich mit E-Mail/Passwort anmelden                    | `LoginActivity`                                 | ❌         | `LoginActivity`, `LoginRepository` **[Gemacht]**          |
| **Alle**       | das Dark/Light-Thema ändern und in SP speichern      | `LoginActivity` (Theme-Switch)                  | ❌         | `ThemeConstants`, `BetreuerApp` **(App-Initialisierung)** **[Gemacht]** |
| **Alle**       | automatisch wieder anmelden                          | Splash/App-Start                                | ❌         | `LoginActivity.checkAutoLogin()` **[Gemacht]**            |
| **Student**    | meine Abschlussarbeiten sehen                         | `DashboardActivity` (Student-Ansicht)           | ✅         | `dashboard_student.xml`, `DashboardActivity` **[Gemacht]** |
| **Betreuer**   | meine Thesenangebote verwalten                       | `DashboardActivity` (Betreuer-Ansicht)          | ❌         | `dashboard_lecturer.xml`, `DashboardActivity` **[Gemacht]** |
| **Student**    | Tutoren suchen                                       | `DashboardActivity` (Student)                   | ✅         | `btn_find_tutor` in `dashboard_student.xml` **[Gemacht]** |
| **Betreuer**   | offene Anfragen verwalten                           | `DashboardActivity` (Betreuer)                  | ❌         | `btn_pending_requests`, `lecturer_requests_card` **[Gemacht]** |
| **Student**    | meine Anfragen einsehen                              | `ThesisRequestActivity`                         | ✅         | `recyclerViewRequests` in `activity_thesis_request.xml` **[Gemacht]** |

---


