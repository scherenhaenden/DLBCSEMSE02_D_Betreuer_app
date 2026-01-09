# ThesisRequestController

## Overview
Manages thesis requests including creation, retrieval, and response handling for supervision and co-supervision requests.

## Endpoints

### POST /thesis-requests
Creates a new thesis request.

**Input Model:** `CreateThesisRequestRequest`
```json
{
  "thesisId": "guid",
  "receiverId": "guid",
  "requestType": "string",
  "message": "string"
}
```

**Output Model:** `ThesisRequestResponse`
```json
{
  "id": "guid",
  "thesisId": "guid",
  "thesisTitle": "string",
  "requester": {
    "id": "guid",
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "roles": ["string"]
  },
  "receiver": {
    "id": "guid",
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "roles": ["string"]
  },
  "requestType": "string",
  "status": "string",
  "message": "string",
  "createdAt": "datetime"
}
```

### GET /thesis-requests
Retrieves paginated list of current user's thesis requests.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisRequestResponse>` (same structure as above)

### GET /thesis-requests/{id}
Retrieves a specific thesis request by ID.

**Path Parameters:**
- `id` (Guid): Thesis request identifier

**Input Model:** None

**Output Model:** `ThesisRequestResponse` (same as above)

### POST /thesis-requests/{id}/respond
Responds to a thesis request.

**Path Parameters:**
- `id` (Guid): Thesis request identifier

**Input Model:** `RespondToThesisRequestRequest`
```json
{
  "accepted": "boolean",
  "message": "string"
}
```

**Output Model:** None

### DELETE /thesis-requests/{id}
Deletes a thesis request (only the requester can delete their own pending requests).

**Path Parameters:**
- `id` (Guid): Thesis request identifier

**Input Model:** None

**Output Model:** None

### GET /thesis-requests/tutor/receiver
Retrieves paginated thesis requests where current user is the receiver (Tutor role).

**Query Parameters:**
- `status` (string, optional): Filter by status
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisRequestResponse>` (same as above)

### GET /thesis-requests/tutor/requester
Retrieves paginated thesis requests where current user is the requester (Tutor role).

**Query Parameters:**
- `status` (string, optional): Filter by status
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)

**Input Model:** None

**Output Model:** `PaginatedResponse<ThesisRequestResponse>` (same as above)

## Models

### CreateThesisRequestRequest
- `thesisId` (Guid, not nullable): Thesis identifier
- `receiverId` (Guid, not nullable): Receiver user identifier
- `requestType` (string, not nullable): Request type ("SUPERVISION" or "CO_SUPERVISION")
- `message` (string, nullable): Request message

### RespondToThesisRequestRequest
- `accepted` (boolean, not nullable): Whether the request is accepted
- `message` (string, nullable): Response message

### ThesisRequestResponse
- `id` (Guid, not nullable): Request identifier
- `thesisId` (Guid, not nullable): Thesis identifier
- `thesisTitle` (string, not nullable): Thesis title
- `requester` (UserResponse, not nullable): Requester user information
- `receiver` (UserResponse, not nullable): Receiver user information
- `requestType` (string, not nullable): Request type
- `status` (string, not nullable): Request status
- `message` (string, nullable): Request message
- `createdAt` (DateTime, not nullable): Creation timestamp

### UserResponse
- `id` (Guid, not nullable): User identifier
- `firstName` (string, not nullable): User's first name
- `lastName` (string, not nullable): User's last name
- `email` (string, not nullable): User's email address
- `roles` (string[], not nullable): Array of user roles

### PaginatedResponse<T>
- `items` (T[], not nullable): Array of items
- `totalCount` (int, not nullable): Total number of items
- `page` (int, not nullable): Current page number
- `pageSize` (int, not nullable): Items per page
