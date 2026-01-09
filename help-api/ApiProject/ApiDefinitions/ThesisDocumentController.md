# ThesisDocumentController

## Overview
Manages thesis documents including retrieval, download, update, and deletion operations.

## Endpoints

### GET /theses/{thesisId}/document
Retrieves document metadata for a specific thesis.

**Path Parameters:**
- `thesisId` (Guid): Thesis identifier

**Input Model:** None

**Output Model:** `ThesisDocumentResponse`
```json
{
  "id": "guid",
  "fileName": "string",
  "contentType": "string",
  "thesisId": "guid",
  "userId": "guid"
}
```

### GET /theses/{thesisId}/document/download
Downloads the actual document file for a specific thesis.

**Path Parameters:**
- `thesisId` (Guid): Thesis identifier

**Input Model:** None

**Output Model:** File content (binary)

**HTTP Headers:**
- `Content-Type`: File content type
- `Content-Disposition`: attachment; filename="..."

### PUT /theses/{thesisId}/document
Updates or creates a document for a specific thesis.

**Path Parameters:**
- `thesisId` (Guid): Thesis identifier

**Input Model:** `UpdateThesisDocumentRequest` (Form data)
```
Content-Type: multipart/form-data

document: file (required)
```

**Output Model:** `ThesisDocumentResponse` (same as above)

### DELETE /theses/{thesisId}/document
Deletes the document for a specific thesis.

**Path Parameters:**
- `thesisId` (Guid): Thesis identifier

**Input Model:** None

**Output Model:** None

## Models

### UpdateThesisDocumentRequest
- `document` (IFormFile, required): Document file to upload

### ThesisDocumentResponse
- `id` (Guid): Document identifier
- `fileName` (string): Document filename
- `contentType` (string): Document content type (MIME type)
- `thesisId` (Guid): Associated thesis identifier
- `userId` (Guid): User who requested the document information
