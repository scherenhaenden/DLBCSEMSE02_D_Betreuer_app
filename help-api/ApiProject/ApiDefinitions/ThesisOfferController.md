# ThesisOfferController

## Overview
Manages thesis offers including CRUD operations, user-specific retrieval, and status management.

## Endpoints

### GET /thesis-offers
Retrieves paginated list of thesis offers with authorization-based filtering.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisOfferResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "title": "string",
      "description": "string",
      "subjectAreaId": "guid",
      "tutorId": "guid",
      "status": "string",
      "maxStudents": "int",
      "expiresAt": "datetime"
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### GET /thesis-offers/user/{userId}
Retrieves paginated thesis offers for a specific user.

**Path Parameters:**
- `userId` (Guid): User identifier

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisOfferResponse>` (same as above)

### POST /thesis-offers
Creates a new thesis offer (Tutor role required).

**Input Model:** `CreateThesisOfferRequest`
```json
{
  "title": "string",
  "description": "string",
  "subjectAreaId": "guid",
  "maxStudents": "int",
  "expiresAt": "datetime",
  "tutorId": "guid"
}
```

**Output Model:** `ThesisOfferResponse` (same as above)

### PUT /thesis-offers/{id}
Updates an existing thesis offer (Tutor role required).

**Path Parameters:**
- `id` (Guid): Thesis offer identifier

**Input Model:** `UpdateThesisOfferRequest`
```json
{
  "title": "string",
  "description": "string",
  "subjectAreaId": "guid",
  "maxStudents": "int",
  "expiresAt": "datetime",
  "thesisOfferStatusId": "guid"
}
```

**Output Model:** `ThesisOfferResponse` (same as above)

### GET /thesis-offers/statuses
Retrieves all available thesis offer statuses.

**Input Model:** None

**Output Model:** `ThesisOfferStatusResponse[]`
```json
[
  {
    "id": "guid",
    "name": "string"
  }
]
```

## Models

### CreateThesisOfferRequest
- `title` (string, required): Thesis offer title
- `description` (string, optional): Thesis offer description
- `subjectAreaId` (Guid, required): Subject area identifier
- `maxStudents` (int, optional): Maximum number of students
- `expiresAt` (DateTime, optional): Expiration date
- `tutorId` (Guid): Tutor identifier (set automatically from authenticated user)

### UpdateThesisOfferRequest
- `title` (string, optional): Thesis offer title
- `description` (string, optional): Thesis offer description
- `subjectAreaId` (Guid, optional): Subject area identifier
- `maxStudents` (int, optional): Maximum number of students
- `expiresAt` (DateTime, optional): Expiration date
- `thesisOfferStatusId` (Guid, optional): Thesis offer status identifier

### ThesisOfferResponse
- `id` (Guid): Thesis offer identifier
- `title` (string): Thesis offer title
- `description` (string): Thesis offer description
- `subjectAreaId` (Guid): Subject area identifier
- `tutorId` (Guid): Tutor identifier
- `status` (string): Thesis offer status
- `maxStudents` (int): Maximum number of students
- `expiresAt` (DateTime): Expiration date

### ThesisOfferStatusResponse
- `id` (Guid): Status identifier
- `name` (string): Status name

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
