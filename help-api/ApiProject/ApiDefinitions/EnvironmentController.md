# EnvironmentController

## Overview
Provides information about the current environment.

## Endpoints

### GET /api/Environment
Returns the current environment name.

**Input Model:** None

**Output Model:** Anonymous object
```json
{
  "environment": "string"
}
```

**HTTP Status Codes:**
- 200 OK: Environment information returned

## Models

No specific models used - returns anonymous object with environment name.
