# User Management API

## Project Overview

An ASP.NET Core Web API for TechHive Solutions to manage employee records for HR and IT departments.

## Technology Stack

- C# and .NET 10
- ASP.NET Core Web API
- RESTful endpoints with JSON request and response bodies
- Postman for API testing
- Microsoft Copilot for code generation and improvements

## API Endpoints

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/users` | Retrieve all users |
| GET | `/api/users/{id}` | Retrieve a user by ID |
| POST | `/api/users` | Create a user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |

The API uses an in-memory list with sample records. Changes are not persisted after the application stops.
`GET /api/users` accepts optional `page` and `pageSize` query parameters together; `pageSize` is limited to 100. Without them, the endpoint returns the full list for backward compatibility.
All `/api/users` endpoints require a valid JWT bearer token.

### Validation and Responses

- `200 OK` when retrieving users successfully.
- `201 Created` when a user is created; the response includes a location for the new user.
- `204 No Content` when a user is updated or deleted successfully.
- `400 Bad Request` when model validation fails or the PUT route ID does not match the user ID in the request body.
- `404 Not Found` when the requested user does not exist.
- `401 Unauthorized` when the bearer token is missing, invalid, or expired.
- `409 Conflict` when an email address is already used by another user.

The user model requires a name, email, and department, and validates the email format. For PUT requests, the request body's `Id` must match the `{id}` in the URL.
Names and departments are limited to 100 characters, and email addresses to 254 characters. Email addresses are trimmed and compared case-insensitively to reject duplicates.

## JWT Authentication

The nonsecret issuer and audience are configured in `appsettings.json` under `Authentication:Schemes:Bearer`. Do not put a signing key in a committed settings file. The .NET configuration system also accepts environment-variable overrides, using double underscores in place of colons (for example, `Authentication__Schemes__Bearer__ValidIssuer`).

For local development, create a project-specific JWT with the .NET user-JWT tool:

```powershell
dotnet user-jwts create --issuer UserManagementAPI --audience UserManagementAPI.Client --valid-for 1h --output token
```

The tool stores its signing key in the developer's user-secrets store and writes the matching development issuer and audience settings to `appsettings.Development.json`. Use the printed token in Postman with Authorization type **Bearer Token**. For deployment, provide the signing key through a secret store or the `Authentication__Schemes__Bearer__SigningKeys__0__Value` environment variable, and override issuer and audience with environment variables as needed.

## How Copilot Assisted

- Helped create the user model and validation rules.
- Generated CRUD endpoints in the users controller.
- Assisted with HTTP status codes and request validation.
- Helped identify and resolve API testing issues.

## Testing

Tested all five endpoints using Postman. Verified successful creation, retrieval, updating, and deletion of users.

## Improvements and Learnings

Reviewed generated code and corrected the PUT request URL to include the user ID. Learned how REST routing, JSON request bodies, model validation, and HTTP status codes work in ASP.NET Core.