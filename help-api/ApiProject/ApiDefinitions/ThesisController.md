# ThesisController

## Overview
Manages thesis entities including CRUD operations, document handling, and billing status retrieval.

## Endpoints

### GET /theses
Retrieves paginated list of theses with authorization-based filtering.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "title": "string",
      "description": "string",
      "status": "string",
      "billingStatus": "string",
      "ownerId": "guid",
      "tutorId": "guid",
      "secondSupervisorId": "guid",
      "subjectAreaId": "guid",
      "documentFileName": "string",
      "documentId": "guid"
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### GET /theses/{id}
Retrieves a specific thesis by ID.

**Path Parameters:**
- `id` (Guid): Thesis identifier

**Input Model:** None

**Output Model:** `ThesisResponse` (same structure as above)

### POST /theses
Creates a new thesis with optional document upload.

**Input Model:** `CreateThesisApiRequest` (Form data)
```
Content-Type: multipart/form-data

title: string
description: string (optional)
subjectAreaId: guid
document: file (optional)
```

**Output Model:** `ThesisResponse`

### PUT /theses/{id}
Updates an existing thesis with optional document update.

**Path Parameters:**
- `id` (Guid): Thesis identifier

**Input Model:** `UpdateThesisRequest` (Form data)
```
Content-Type: multipart/form-data

title: string (optional)
description: string (optional)
subjectAreaId: guid (optional)
document: file (optional)
```

**Output Model:** `ThesisResponse`

### DELETE /theses/{id}
Deletes a thesis (authorization required).

**Path Parameters:**
- `id` (Guid): Thesis identifier

**Input Model:** None

**Output Model:** None

### GET /theses/billing-statuses
Retrieves all available billing statuses.

**Input Model:** None

**Output Model:** `BillingStatusResponse[]`
```json
[
  {
    "id": "guid",
    "name": "string"
  }
]
```

## Models

### CreateThesisApiRequest
- `title` (string, not nullable): Thesis title
- `description` (string, nullable): Thesis description
- `subjectAreaId` (Guid, not nullable): Subject area identifier
- `document` (IFormFile, nullable): Document file upload

### UpdateThesisRequest
- `title` (string, nullable): Thesis title
- `description` (string, nullable): Thesis description
- `subjectAreaId` (Guid, nullable): Subject area identifier
- `document` (IFormFile, nullable): Document file upload

### ThesisResponse
- `id` (Guid, not nullable): Thesis identifier
- `title` (string, not nullable): Thesis title
- `description` (string, nullable): Thesis description
- `status` (string, not nullable): Thesis status
- `billingStatus` (string, not nullable): Billing status
- `ownerId` (Guid, not nullable): Owner user identifier
- `tutorId` (Guid, nullable): Tutor user identifier
- `secondSupervisorId` (Guid, nullable): Second supervisor identifier
- `subjectAreaId` (Guid, nullable): Subject area identifier
- `documentFileName` (string, nullable): Document filename
- `documentId` (Guid, nullable): Document identifier

### BillingStatusResponse
- `id` (Guid, not nullable): Billing status identifier
- `name` (string, not nullable): Billing status name

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
