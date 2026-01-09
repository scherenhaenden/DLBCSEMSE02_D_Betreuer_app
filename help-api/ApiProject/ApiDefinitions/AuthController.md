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
- `email` (string, required): User's email address
- `password` (string, required): User's password

### LoginResponse
- `token` (string): JWT authentication token
- `user` (UserResponse): User information

### UserResponse
- `id` (Guid): User identifier
- `firstName` (string): User's first name
- `lastName` (string): User's last name
- `email` (string): User's email address
- `roles` (string[]): Array of user roles
