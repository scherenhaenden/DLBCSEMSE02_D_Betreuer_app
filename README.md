# Betreuer-App (Thesis Supervision App)

> **A comprehensive mobile solution for managing academic thesis supervision processes.**

**Betreuer-App** is a robust mobile application designed to streamline the complex workflow of thesis supervision at **IU International University of Applied Sciences**. By connecting **Students**, **Supervisors (Tutors)**, and **Second Examiners**, it facilitates seamless communication, efficient topic management, and real-time tracking of thesis progress.

This project was engineered as part of the **Mobile Software Engineering II (DLBCSEMSE02)** course, adhering to strict architectural patterns (MVVM, Clean Architecture) and modern development practices.

---

## 🚀 Features

The system is composed of two primary components: a native Android client and a .NET backend API.

### 📱 Mobile Application (Android)
Designed for usability and efficiency, the mobile app provides:

*   **Secure Authentication**: Role-based login for students and tutors.
*   **Interactive Dashboard**: A personalized overview of pending tasks and active theses.
*   **Thesis Management**:
    *   **Students**: Search for supervisors, apply for topics, submit exposés, and track status changes.
    *   **Supervisors**: Publish research topics, review applications, and manage supervised students.
*   **Status Tracking**: Real-time updates on the thesis lifecycle (e.g., *In Coordination*, *Registered*, *Submitted*, *Colloquium Completed*).
*   **Billing Overview**: Supervisors can track their issued and paid invoices.
*   **Topic Marketplace**: A centralized hub for available research topics and subject areas.

### 🖥 Backend API
A powerful RESTful service that powers the mobile experience:

*   **Role-Based Access Control (RBAC)**: Enforces strict permissions for different user roles.
*   **Lifecycle Management**: State machine logic to handle valid thesis status transitions.
*   **Automated Seeding**: Initializes the database with sample data (Users, Roles, Topics) for instant testing.
*   **Billing Logic**: Manages billing statuses associated with completed supervision work.
*   **Documentation**: Integrated Swagger UI for API exploration and testing.

---

## 🛠 Tech Stack

### Android Client (`/app`)
*   **Language**: Java 11
*   **Framework**: Native Android SDK (Min SDK 24, Target SDK 36)
*   **Build System**: Gradle (Kotlin DSL)
*   **Architecture**: Model-View-ViewModel (MVVM)
*   **Networking**: Retrofit 2 + Gson
*   **UI**: Material Design, ConstraintLayout, RecyclerView
*   **Testing**: JUnit, Espresso

### Backend API (`/help-api`)
*   **Framework**: .NET 10.0 (Preview)
*   **Architecture**: Clean Architecture (N-Tier: API, Business Logic, Data Access)
*   **Database**: SQLite (`ThesisManagement.db`) with Entity Framework Core
*   **Hosting**: Kestrel (Cross-platform)
*   **Documentation**: Swagger/OpenAPI, PlantUML
*   **CI/CD**: GitHub Actions

---

## 📂 Project Structure

```bash
├── app/                  # 📱 Native Android Application
│   ├── src/main/java     # Java Source Code (Activities, ViewModels, Repositories)
│   ├── src/main/res      # Android Resources (Layouts, Strings, Drawables)
│   └── build.gradle.kts  # App-level build configuration
│
├── help-api/             # 🖥 Backend Solution
│   ├── ApiProject/       # Main Web API Project
│   │   ├── ApiLogic/     # Controllers & Endpoints
│   │   ├── BusinessLogic/# Core Business Rules & Services
│   │   ├── DatabaseAccess/# EF Core Context & Entities
│   │   └── seed.json     # Initial data for the database
│   └── ApiProject.Tests/ # Unit Tests
│
├── .github/workflows/    # ⚙️ CI/CD Pipelines
└── README.md             # 📄 Project Documentation
```

---

## ⚙️ Getting Started

Follow these instructions to set up the development environment.

### Prerequisites
*   **Android Studio** (Koala or newer recommended)
*   **JDK 11** or higher
*   **.NET SDK 10.0** (Preview)
*   **Git**

### 1. Backend Setup (API)
The Android app relies on the API to function. You must start the backend first.

1.  Navigate to the API project directory:
    ```bash
    cd help-api/ApiProject
    ```
2.  Restore dependencies:
    ```bash
    dotnet restore
    ```
3.  Run the application:
    ```bash
    dotnet run
    ```
    *   The API will start at `http://localhost:8080` (default).
    *   **Database**: A SQLite database `ThesisManagement.db` is created and seeded automatically on the first run.
    *   **Swagger UI**: Open `http://localhost:8080/swagger/index.html` to explore the API endpoints.

### 2. Mobile App Setup (Android)
1.  Open **Android Studio**.
2.  Select **Open Project** and choose the root `betreuer-app` directory.
3.  Allow Gradle to sync and download dependencies.
4.  **Configure API Connection**:
    *   **Emulator**: Android emulators use `10.0.2.2` to access `localhost` of the host machine. Ensure the Retrofit base URL is configured correctly if not using service discovery.
    *   **Physical Device**: Ensure your phone and computer are on the same Wi-Fi. Update the API URL to your computer's local IP address (e.g., `http://192.168.1.5:8080`).
    *   *Note: The app allows cleartext traffic (HTTP) by default via `network_security_config` for development ease.*
5.  Press **Run** (Green Play Button) to deploy to your device/emulator.

---

## 🔍 Architecture & Design

### Android Client (MVVM)
The application implements the **Model-View-ViewModel** pattern to ensure a clean separation between the user interface and data logic.
*   **View (Activities)**: Responsible only for rendering the UI and capturing user input.
*   **ViewModel**: Manages UI state, handles business logic, and communicates with the Repository. It survives configuration changes (like screen rotation).
*   **Repository**: Acts as the single source of truth for data, mediating between the remote API (Retrofit) and the rest of the app.

### Backend API (Clean Architecture)
The backend is structured to separate concerns and maximize testability:
*   **ApiLogic**: Contains Controllers that handle HTTP requests and responses.
*   **BusinessLogic**: Encapsulates the core rules (e.g., "A student can only have one active thesis").
*   **DatabaseAccess**: Manages direct interactions with the SQLite database using EF Core.

**Visual Documentation**:
Detailed UML diagrams (Class, Use Case, Sequence, Component) are available in the `help-api/ApiProject/` directory as `.puml` files. These can be viewed using the [PlantUML](https://plantuml.com/) extension in VS Code.

---

## 🔄 CI/CD Pipeline
Automated building and releasing are handled via **GitHub Actions**.

*   **Workflow**: `.github/workflows/dotnet-release.yml`
*   **Trigger**: Pushes to the `api-build` branch.
*   **Process**:
    1.  Sets up the .NET 10.0 environment.
    2.  Builds the API in `Release` configuration.
    3.  Publishes the application and includes `seed.json`.
    4.  Packages the output into a ZIP file.
    5.  Creates a GitHub Release tag.

---

## 📜 Credits
**Developer**: Abraham Angene, Edward Sergio José Flores Mogollón, Stefan Schaerl, Michael Wolff
**Course**: Mobile Software Engineering II (DLBCSEMSE02)
**Institution**: IU International University of Applied Sciences

*Built with passion and code.* ❤️
