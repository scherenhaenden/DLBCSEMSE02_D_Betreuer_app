# ThesisOfferApplicationController

## Overview
Manages thesis offer applications including retrieval of user's applications and creation of new applications.

## Endpoints

### GET /thesis-offer-applications
Retrieves paginated list of current user's thesis offer applications.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisOfferApplicationResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "thesisOfferId": "guid",
      "studentId": "guid",
      "status": "string",
      "message": "string"
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### POST /thesis-offer-applications
Creates a new thesis offer application (Student role required).

**Input Model:** `CreateThesisOfferApplicationRequest`
```json
{
  "thesisOfferId": "guid",
  "message": "string",
  "studentId": "guid"
}
```

**Output Model:** `ThesisOfferApplicationResponse` (same as above)

## Models

### CreateThesisOfferApplicationRequest
- `thesisOfferId` (Guid, required): Thesis offer identifier
- `message` (string, optional): Application message
- `studentId` (Guid): Student identifier (set automatically from authenticated user)

### ThesisOfferApplicationResponse
- `id` (Guid): Application identifier
- `thesisOfferId` (Guid): Thesis offer identifier
- `studentId` (Guid): Student identifier
- `status` (string): Application status
- `message` (string): Application message

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
