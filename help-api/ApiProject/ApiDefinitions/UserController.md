# UserController

## Overview
Manages user accounts including CRUD operations and search functionality.

## Endpoints

### GET /users
Retrieves paginated list of users with optional filtering.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10)
- `email` (string, optional): Filter by email (partial match)
- `firstName` (string, optional): Filter by first name (partial match)
- `lastName` (string, optional): Filter by last name (partial match)
- `role` (string, optional): Filter by role

**Input Model:** None

**Output Model:** `PaginatedResponse<UserResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "firstName": "string",
      "lastName": "string",
      "email": "string",
      "roles": ["string"]
    }
  ],
  "totalCount": "int",
  "page": "int",
  "pageSize": "int"
}
```

### POST /users
Creates a new user account.

**Input Model:** `CreateUserRequest`
```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "roles": ["string"]
}
```

**Output Model:** `UserResponse` (same as above)

### GET /users/{id}
Retrieves a specific user by ID.

**Path Parameters:**
- `id` (Guid): User identifier

**Input Model:** None

**Output Model:** `UserResponse` (same as above)

## Models

### CreateUserRequest
- `firstName` (string, required): User's first name
- `lastName` (string, required): User's last name
- `email` (string, required): User's email address (must be valid email format)
- `password` (string, required): User's password
- `roles` (string[], optional): List of user roles

### UserResponse
- `id` (Guid): User identifier
- `firstName` (string): User's first name
- `lastName` (string): User's last name
- `email` (string): User's email address
- `roles` (string[]): Array of user roles

### PaginatedResponse<T>
- `items` (T[]): Array of items
- `totalCount` (int): Total number of items
- `page` (int): Current page number
- `pageSize` (int): Items per page
