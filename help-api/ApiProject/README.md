# Thesis Management API

This project implements a fully object-oriented API for managing users, roles, topics, and theses. All IDs are GUIDs to prevent collisions.

## Architecture

- **DatabaseAccess/**: Data models (entities) using C# classes.
- **Logic/**: Business logic services for validation and operations.
- **Api/**: REST controllers for HTTP endpoints.

## UML Diagrams

This project includes a set of PlantUML diagrams to visualize the application's architecture and behavior.

- **`ThesisManagementAPI-ClassDiagram.puml`**: Shows the classes, their attributes, methods, and the relationships between them.
- **`ThesisManagementAPI-UseCaseDiagram.puml`**: Describes the interactions between actors (Student, Tutor, Admin) and the system.
- **`ThesisManagementAPI-SequenceDiagram.puml`**: Details the flow of a user login.
- **`ThesisManagementAPI-ComponentDiagram.puml`**: Represents the main software components and their dependencies.
- **`ThesisManagementAPI-ActivityDiagram.puml`**: Models the process of submitting a thesis.

### Usage

To view and edit these diagrams, you can use the following tools:
- **Visual Studio Code**: With the [PlantUML extension](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml).
- **Online Editor**: Tools like the [official PlantUML web server](http://www.plantuml.com/plantuml).
- **Other IDEs**: Many IDEs, like IntelliJ IDEA, offer plugins for viewing `.puml` files.

Simply copy the content of a `.puml` file into an online editor or open it in an IDE with the appropriate extension to generate the visual diagram.

## Configuration

### API Port
The port on which the API runs can be configured in the `appsettings.json` file. Change the value of `Urls` to set the desired port:

```json
{
  "Urls": "http://localhost:8080"
}
```

## Database Schema

### Users
- Id: Guid (PK)
- FirstName: string
- LastName: string
- Email: string (unique)
- PasswordHash: string

### Roles
- Id: Guid (PK)
- Name: string (e.g., "STUDENT", "TUTOR")

### UserRoles (Junction Table)
- UserId: Guid (FK to Users.Id)
- RoleId: Guid (FK to Roles.Id)

### Topics
- Id: Guid (PK)
- Title: string
- Description: string
- SubjectArea: string
- IsActive: bool

### UserTopics (Junction Table)
- UserId: Guid (FK to Users.Id)
- TopicId: Guid (FK to Topics.Id)

### Theses
- Id: Guid (PK)
- Title: string
- SubjectArea: string
- ExposePath: string?
- OwnerId: Guid (FK to Users.Id)
- TutorId: Guid (FK to Users.Id)
- SecondSupervisorId: Guid? (FK to Users.Id)
- TopicId: Guid? (FK to Topics.Id)
- StatusId: Guid (FK to ThesisStatuses.Id)
- BillingStatusId: Guid (FK to BillingStatuses.Id)

### ThesisStatuses
- Id: Guid (PK)
- Name: string (e.g., "PendingApproval", "Registered")

### BillingStatuses
- Id: Guid (PK)
- Name: string (e.g., "None", "Invoiced")

## API Endpoints

- GET /users: Retrieve all users
- POST /users: Create a new user (with role validation)
- GET /theses: Retrieve all theses
- POST /theses: Create a new thesis (Validation: Owner must be STUDENT, Tutors must be TUTOR)

## Setup

1. Install the .NET SDK.
2. Run `dotnet restore`.
3. Configure the database connection in `appsettings.json`.
4. Start the server: `dotnet run`.

## Validations

- On Thesis creation: Check if `OwnerId` has the STUDENT role and `TutorId`/`SecondSupervisorId` have the TUTOR role.
- All GUIDs are generated automatically.
