# AuthController

## Overview
Handles user authentication including login functionality.

## Endpoints

### POST /auth/login
Authenticates a user and returns a JWT token.

**Input Model:** `LoginRequest`
```json
{
  "email": "string",
  "password": "string"
}
```

**Output Model:** `LoginResponse`
```json
{
  "token": "string",
  "user": {
    "id": "guid",
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "roles": ["string"]
  }
}
```

**HTTP Status Codes:**
- 200 OK: Successful login
- 401 Unauthorized: Invalid credentials

## Models

### LoginRequest
- `email` (string, not nullable): User's email address
- `password` (string, not nullable): User's password

### LoginResponse
- `token` (string, not nullable): JWT authentication token
- `user` (UserResponse, not nullable): User information

### UserResponse
- `id` (Guid, not nullable): User identifier
- `firstName` (string, not nullable): User's first name
- `lastName` (string, not nullable): User's last name
- `email` (string, not nullable): User's email address
- `roles` (string[], not nullable): Array of user roles
