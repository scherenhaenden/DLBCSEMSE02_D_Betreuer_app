# SubjectAreaController

## Overview
Manages subject areas including CRUD operations and search functionality.

## Endpoints

### GET /subject-areas
Retrieves paginated list of all subject areas.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<SubjectAreaResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "title": "string",
      "description": "string",
      "subjectArea": "string",
      "isActive": "boolean",
      "tutorIds": ["guid"]
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### GET /subject-areas/search
Searches subject areas by query string.

**Query Parameters:**
- `q` (string, required): Search query
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<SubjectAreaResponse>` (same as above)

### GET /subject-areas/{id}
Retrieves a specific subject area by ID.

**Path Parameters:**
- `id` (Guid): Subject area identifier

**Input Model:** None

**Output Model:** `SubjectAreaResponse`
```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "subjectArea": "string",
  "isActive": "boolean",
  "tutorIds": ["guid"]
}
```

### POST /subject-areas
Creates a new subject area.

**Input Model:** `CreateSubjectAreaRequest`
```json
{
  "title": "string",
  "description": "string",
  "subjectArea": "string",
  "tutorIds": ["guid"]
}
```

**Output Model:** `SubjectAreaResponse` (same as above)

### PUT /subject-areas/{id}
Updates an existing subject area.

**Path Parameters:**
- `id` (Guid): Subject area identifier

**Input Model:** `UpdateSubjectAreaRequest`
```json
{
  "title": "string",
  "description": "string",
  "subjectArea": "string",
  "isActive": "boolean",
  "tutorIds": ["guid"]
}
```

**Output Model:** `SubjectAreaResponse` (same as above)

### DELETE /subject-areas/{id}
Deletes a subject area.

**Path Parameters:**
- `id` (Guid): Subject area identifier

**Input Model:** None

**Output Model:** None

## Models

### CreateSubjectAreaRequest
- `title` (string, required): Subject area title
- `description` (string, required): Subject area description
- `subjectArea` (string, required): Subject area content/details
- `tutorIds` (Guid[], optional): List of tutor identifiers

### UpdateSubjectAreaRequest
- `title` (string, optional): Subject area title
- `description` (string, optional): Subject area description
- `subjectArea` (string, optional): Subject area content/details
- `isActive` (boolean, optional): Whether the subject area is active
- `tutorIds` (Guid[], optional): List of tutor identifiers

### SubjectAreaResponse
- `id` (Guid): Subject area identifier
- `title` (string): Subject area title
- `description` (string): Subject area description
- `subjectArea` (string): Subject area content/details
- `isActive` (boolean): Whether the subject area is active
- `tutorIds` (Guid[]): List of tutor identifiers

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
