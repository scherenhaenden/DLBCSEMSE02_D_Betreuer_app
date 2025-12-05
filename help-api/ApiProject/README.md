# Thesis Management API

Dieses Projekt implementiert eine vollständig objektorientierte API für die Verwaltung von Benutzern, Rollen, Themen und Abschlussarbeiten (Theses). Alle IDs sind GUIDs zur Vermeidung von Kollisionen.

## Architektur

- **db/**: Datenmodelle (Entitäten) mit C#-Klassen.
- **logic/**: Geschäftslogik-Services für Validierungen und Operationen.
- **api/**: REST-Controller für HTTP-Endpunkte.

## UML-Diagramme

Dieses Projekt enthält eine Reihe von PlantUML-Diagrammen, um die Architektur und das Verhalten der Anwendung zu visualisieren.

- **`ThesisManagementAPI-ClassDiagram.puml`**: Zeigt die Klassen, ihre Attribute, Methoden und die Beziehungen zwischen ihnen.
- **`ThesisManagementAPI-UseCaseDiagram.puml`**: Beschreibt die Interaktionen zwischen den Akteuren (Student, Tutor, Admin) und dem System.
- **`ThesisManagementAPI-SequenceDiagram.puml`**: Detailliert den Ablauf einer Benutzeranmeldung.
- **`ThesisManagementAPI-ComponentDiagram.puml`**: Stellt die Hauptkomponenten der Software und ihre Abhängigkeiten dar.
- **`ThesisManagementAPI-ActivityDiagram.puml`**: Modelliert den Prozess der Einreichung einer Abschlussarbeit.

### Verwendung

Um diese Diagramme anzuzeigen und zu bearbeiten, können Sie die folgenden Werkzeuge verwenden:
- **Visual Studio Code**: Mit der [PlantUML-Erweiterung](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml).
- **Online-Editor**: Tools wie der [offizielle PlantUML-Webserver](http://www.plantuml.com/plantuml).
- **Andere IDEs**: Viele IDEs wie IntelliJ IDEA bieten Plugins zur Anzeige von `.puml`-Dateien.

Kopieren Sie einfach den Inhalt einer `.puml`-Datei in einen Online-Editor oder öffnen Sie sie in einer IDE mit entsprechender Erweiterung, um das visuelle Diagramm zu generieren.

## Konfiguration

### API-Port
Der Port, auf dem die API ausgeführt wird, kann in der Datei `appsettings.json` konfiguriert werden. Ändern Sie den Wert von `Urls`, um den gewünschten Port festzulegen:

```json
{
  "Urls": "http://localhost:8080"
}
```

## Datenbank-Schema

### Users
- id: Guid (PK)
- first_name: string
- last_name: string
- email: string (unique)
- password_hash: string

### Roles
- id: Guid (PK)
- name: "STUDENT" | "TUTOR"

### UserRoles
- user_id: Guid (FK zu users.id)
- role_id: Guid (FK zu roles.id)

### Topics
- id: Guid (PK)
- title: string
- description: string
- subject_area: string
- is_active: bool
- tutor_id: Guid (FK zu users.id)

### Theses
- id: Guid (PK)
- title: string
- status: string (z.B. "DRAFT", "SUBMITTED")
- progress_percent: int
- expose_path: string
- billing_status: "NONE" | "INVOICED" | "PAID"
- owner_id: Guid (FK zu users.id, muss STUDENT sein)
- tutor_id: Guid (FK zu users.id, muss TUTOR sein)
- second_supervisor_id: Guid? (FK zu users.id, muss TUTOR sein)
- topic_id: Guid? (FK zu topics.id)

## API-Endpunkte

- GET /users: Alle Benutzer abrufen
- POST /users: Neuen Benutzer erstellen (mit Rollenvalidierung)
- GET /theses: Alle Theses abrufen
- POST /theses: Neue Thesis erstellen (Validierung: Owner=STUDENT, Tutors=TUTOR)

## Setup

1. Installieren Sie .NET SDK.
2. Führen Sie `dotnet restore` aus.
3. Konfigurieren Sie die Datenbank (z.B. SQL Server mit Guid-Unterstützung).
4. Starten Sie den Server: `dotnet run`.

## Validierungen

- Bei Thesis-Erstellung: Prüfen, ob owner_id eine STUDENT-Rolle hat und tutor_id/second_supervisor_id TUTOR-Rollen haben.
- Alle GUIDs werden automatisch generiert.
