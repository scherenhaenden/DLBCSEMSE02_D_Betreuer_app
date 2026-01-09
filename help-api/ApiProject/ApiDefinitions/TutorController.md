# TutorController

## Overview
Provides access to tutor profiles and search functionality for finding tutors by various criteria.

## Endpoints

### GET /api/Tutor
Retrieves paginated list of tutors with optional filtering.

**Query Parameters:**
- `subjectAreaId` (Guid, optional): Filter by subject area ID
- `subjectAreaName` (string, optional): Filter by subject area name (partial match, case-insensitive)
- `name` (string, optional): Search in tutor's first or last name (case-insensitive)
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<TutorProfileResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "firstName": "string",
      "lastName": "string",
      "email": "string",
      "subjectAreas": [
        {
          "id": "guid",
          "title": "string",
          "description": "string",
          "subjectArea": "string",
          "isActive": "boolean",
          "tutorIds": ["guid"]
        }
      ]
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### GET /api/Tutor/{id}
Retrieves a specific tutor's profile by ID.

**Path Parameters:**
- `id` (Guid): Tutor identifier

**Input Model:** None

**Output Model:** `TutorProfileResponse` (same structure as above)

## Models

### TutorProfileResponse
- `id` (Guid, not nullable): Tutor identifier
- `firstName` (string, not nullable): Tutor's first name
- `lastName` (string, not nullable): Tutor's last name
- `email` (string, not nullable): Tutor's email address
- `subjectAreas` (SubjectAreaResponse[], not nullable): List of subject areas the tutor is assigned to

### SubjectAreaResponse
- `id` (Guid, not nullable): Subject area identifier
- `title` (string, not nullable): Subject area title
- `description` (string, not nullable): Subject area description
- `subjectArea` (string, not nullable): Subject area content/details
- `isActive` (boolean, not nullable): Whether the subject area is active
- `tutorIds` (Guid[], not nullable): List of tutor identifiers assigned to this subject area

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
