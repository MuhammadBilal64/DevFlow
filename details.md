# DevFlow Backend Integration Reference (`details.md`)

This document serves as the complete technical reference for connecting the **DevFlow Frontend** to the **DevFlow Backend API**. It contains all API endpoints, requests, responses, data models, enums, real-time SignalR events, error handling contracts, and authentication specifications.

---

## 1. Core Architecture & SignalR / HTTP Basics

- **Framework**: .NET 9 ASP.NET Core Web API (Clean Architecture with MediatR CQRS)
- **Base URL**: `http://localhost:5000` / `https://localhost:7001`
- **CORS Allowed Origin**: `http://localhost:5173` (Vite / React default)
- **Content-Type**: `application/json`

---

## 2. Global API Response & Error Envelopes

### 2.1 Standard API Envelope (`ApiResponse<T>`)
All successful HTTP endpoints return data wrapped in the `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "Operation response message",
  "data": { ... } // Or list of objects / null
}
```

### 2.2 Global Exception & Error Envelope
When an error occurs, the server responds with one of the following JSON structures depending on the exception:

#### Validation Error (`400 Bad Request` - `ValidationException`)
Occurs when request body or parameters fail FluentValidation rules:
```json
{
  "message": "One or more validation errors occurred.",
  "errors": [
    "Email address is invalid.",
    "Password is required."
  ]
}
```

#### Standard Business Errors (`401`, `403`, `404`, `409`)
- `401 Unauthorized`: Token missing, expired, or invalid credentials (`UnauthorizedException`)
- `403 Forbidden`: Insufficient permissions or workspace member role (`ForbiddenException`)
- `404 Not Found`: Requested entity (Task, Project, Workspace, User, Workflow) does not exist (`NotFoundException`)
- `409 Conflict`: Resource already exists or state collision (`ConflictException`)

```json
{
  "message": "Exact error message explaining the failure"
}
```

#### Unhandled System Error (`500 Internal Server Error`)
```json
{
  "message": "An unexpected error occurred"
}
```

---

## 3. Authentication & Authorization Setup

### 3.1 HTTP Bearer Header
All protected endpoints require the HTTP Authorization header:
```http
Authorization: Bearer <JWT_ACCESS_TOKEN>
```

### 3.2 SignalR Connection Authentication
Pass the access token as a query string parameter when initiating the SignalR WebSocket connection:
```javascript
const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:5000/notificationHub", {
    accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .build();
```

---

## 4. Enums Reference (Numeric Values & String Names)

> **Important**: When sending enum values to the API, use the exact integer value (or corresponding string representation if configured).

### `UserRole`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Admin` | `0` | Platform Administrator |
| `Manager` | `1` | Workspace / Project Manager |
| `Member` | `2` | Standard User |

### `WorkspaceRole`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Owner` | `0` | Workspace Owner (Full privileges) |
| `Admin` | `1` | Workspace Administrator |
| `Member` | `2` | Regular Workspace Member |

### `TaskPriority`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Low` | `0` | Low Priority Task |
| `Medium` | `1` | Normal / Medium Priority |
| `High` | `2` | Urgent / High Priority |

### `TaskStatus`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Todo` | `0` | Backlog / Todo state |
| `InProgress` | `1` | Currently actively worked on |
| `Completed` | `2` | Task finished |

### `NotificationType`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | A task was assigned to user |
| `TaskCompleted` | `1` | A task was marked complete |
| `ProjectCreated` | `2` | A new project was created |
| `MemberAdded` | `3` | Added to a workspace |
| `Workflow` | `4` | Triggered by automated workflow |

### `NotificationRecipient`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Assignee` | `0` | User assigned to the task |
| `Reporter` | `1` | User who logged/reported issue |
| `ProjectManager` | `2` | Manager of the project |
| `Creator` | `3` | Creator of the task/project |
| `WorkspaceMember` | `4` | All members of workspace |

### `WorkflowTrigger`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | Event when task assignee is updated |
| `TaskCompleted` | `1` | Event when task status changes to Completed |
| `ProjectCreated` | `2` | Event when a new project is created |

### `WorkflowActionType`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `NotifyUser` | `0` | Send notification action |

### `WorkflowOperator`
| Name | Integer Value | Description |
| :--- | :--- | :--- |
| `Equals` | `0` | `==` exact match |
| `NotEquals` | `1` | `!=` not equal |
| `GreaterThan` | `2` | `>` numeric / date greater |
| `LessThan` | `3` | `<` numeric / date less |
| `GreaterThanOrEqual` | `4` | `>=` greater or equal |
| `LessThanOrEqual` | `5` | `<=` less or equal |
| `Contains` | `6` | String substring search |

---

## 5. Common Utility Models & DTOs

### `PaginationRequest` (Query Parameters)
Used across all paginated GET requests:
- `pageNumber`: `number` (integer, default: `1`)
- `pageSize`: `number` (integer, default: `10`)
- `searchTerm`: `string | null` (optional search keyword)
- `sortBy`: `string | null` (property name to sort by)
- `descending`: `boolean` (default: `false`)

### `PagedResult<T>` (Pagination Response Envelope)
Returned inside `data` for all paginated GET endpoints:
- `items`: `T[]` (array of items for current page)
- `pageNumber`: `number` (current page index)
- `pageSize`: `number` (items requested per page)
- `totalPages`: `number` (calculated total page count)
- `totalCount`: `number` (total items available across all pages)
- `hasPreviousPage`: `boolean` (read-only helper)
- `hasNextPage`: `boolean` (read-only helper)

### `WorkflowConditionDto`
Used in Workflow creation & updates:
- `field`: `string` (e.g. `"Priority"`, `"Status"`, `"Title"`)
- `operator`: `WorkflowOperator` (enum integer 0-6)
- `value`: `string` (target value to evaluate)

### `WorkflowActionDto`
Used in Workflow creation & updates:
- `actionType`: `WorkflowActionType` (enum integer, e.g. `0` for `NotifyUser`)
- `parameters`: `string` (JSON string parameter payload, e.g., `"{\"Recipient\":0,\"Message\":\"Task completed\"}"`)
- `order`: `number` (execution sequence order, must be > 0)

### `NotificationRealtimeModel` (SignalR Payload)
- `userId`: `number`
- `message`: `string`
- `type`: `NotificationType` (enum integer)
- `referenceId`: `number | null`
- `createdAt`: `string` (ISO DateTime)

---

## 6. Domain Entity Definitions & Attributes

### 6.1 `User`
- `id`: `number` (Primary Key)
- `name`: `string`
- `email`: `string`
- `passwordHash`: `string`
- `createdAt`: `string` (ISO DateTime)
- `role`: `UserRole` (0 = Admin, 1 = Manager, 2 = Member)

### 6.2 `Workspace`
- `id`: `number` (Primary Key)
- `name`: `string`
- `createdBy`: `number` (User Id of creator)
- `createdAt`: `string` (ISO DateTime)

### 6.3 `WorkspaceMember`
- `id`: `number` (Primary Key)
- `workspaceId`: `number` (Foreign Key -> Workspace)
- `userId`: `number` (Foreign Key -> User)
- `role`: `WorkspaceRole` (0 = Owner, 1 = Admin, 2 = Member)
- `joinedAt`: `string` (ISO DateTime)

### 6.4 `Project`
- `id`: `number` (Primary Key)
- `name`: `string`
- `description`: `string`
- `workspaceId`: `number` (Foreign Key -> Workspace)
- `createdBy`: `number` (User Id of creator)
- `createdAt`: `string` (ISO DateTime)

### 6.5 `TaskItem`
- `id`: `number` (Primary Key)
- `title`: `string`
- `description`: `string | null`
- `projectId`: `number` (Foreign Key -> Project)
- `createdBy`: `number` (User Id of creator)
- `assignedToUserId`: `number | null` (Foreign Key -> User, nullable)
- `createdAt`: `string` (ISO DateTime)
- `assignedAt`: `string | null` (ISO DateTime)
- `dueDate`: `string | null` (ISO DateTime)
- `completedAt`: `string | null` (ISO DateTime)
- `priority`: `TaskPriority` (0 = Low, 1 = Medium, 2 = High)
- `status`: `TaskStatus` (0 = Todo, 1 = InProgress, 2 = Completed)

### 6.6 `Notification`
- `id`: `number` (Primary Key)
- `userId`: `number` (Recipient User Id)
- `message`: `string`
- `type`: `NotificationType` (0-4)
- `isRead`: `boolean`
- `createdAt`: `string` (ISO DateTime)
- `referenceId`: `number | null` (Associated Task / Project Id)

### 6.7 `RefreshToken`
- `id`: `number` (Primary Key)
- `token`: `string`
- `userId`: `number` (Foreign Key -> User)
- `expiresAt`: `string` (ISO DateTime)
- `createdAt`: `string` (ISO DateTime)
- `isRevoked`: `boolean`
- `revokedAt`: `string | null`
- `replacedByToken`: `string | null`

### 6.8 `Workflow`
- `id`: `number` (Primary Key)
- `name`: `string`
- `description`: `string | null`
- `trigger`: `WorkflowTrigger` (0 = TaskAssigned, 1 = TaskCompleted, 2 = ProjectCreated)
- `isEnabled`: `boolean`
- `createdAt`: `string` (ISO DateTime)
- `updatedAt`: `string | null` (ISO DateTime)

### 6.9 `WorkflowAction`
- `id`: `number` (Primary Key)
- `workflowId`: `number` (Foreign Key -> Workflow)
- `actionType`: `WorkflowActionType` (0 = NotifyUser)
- `parameters`: `string` (JSON payload)
- `order`: `number`

### 6.10 `WorkflowCondition`
- `id`: `number` (Primary Key)
- `workflowId`: `number` (Foreign Key -> Workflow)
- `field`: `string`
- `operator`: `WorkflowOperator` (0-6)
- `value`: `string`

---

## 7. Exhaustive API Modules & Endpoint Specification

---

### MODULE 1: Auth API (`/api/Auth`)

#### 1.1 Login User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/Login`
- **Authentication**: Public (Anonymous)
- **Request Body**: `LoginUserCommand`
  ```json
  {
    "email": "user@example.com",
    "password": "Password123!"
  }
  ```
- **Validation Rules**: Email is required & valid format; Password is required.
- **Success Response (`200 OK`)**: `ApiResponse<LoginUserResult>`
  ```json
  {
    "success": true,
    "message": "Login Successfull",
    "data": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
      "refreshToken": "4a8e2b10...",
      "refreshTokenExpiresAt": "2026-07-30T23:40:00Z"
    }
  }
  ```

#### 1.2 Register User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/Register`
- **Authentication**: Public (Anonymous)
- **Request Body**: `RegisterUserCommand`
  ```json
  {
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "role": 2
  }
  ```
- **Validation Rules**: Name required; Email required & valid; Password required; Role is valid UserRole enum.
- **Success Response (`200 OK`)**: `ApiResponse<RegisterUserResult>`
  ```json
  {
    "success": true,
    "message": "Register Successfully",
    "data": {
      "id": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "role": 2,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

#### 1.3 Refresh Access Token
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/refresh`
- **Authentication**: Public (Anonymous)
- **Request Body**: `RefreshTokenCommand`
  ```json
  {
    "refreshToken": "4a8e2b10..."
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<RefreshTokenResult>`
  ```json
  {
    "success": true,
    "message": "Token Refreshed Successfully",
    "data": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
      "refreshToken": "b91f0c22...",
      "refreshTokenExpiresAt": "2026-07-30T23:40:00Z"
    }
  }
  ```

#### 1.4 Logout User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/logout`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**: `LogoutCommand`
  ```json
  {
    "refreshToken": "b91f0c22..."
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<object>`
  ```json
  {
    "success": true,
    "message": "Log out Successfully",
    "data": null
  }
  ```

#### 1.5 Test Protected Endpoint
- **HTTP Method**: `GET`
- **Endpoint**: `/api/Auth/test`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `"You are authorized"`

---

### MODULE 2: Workspace API (`/api/workspaces`)

#### 2.1 Create Workspace
- **HTTP Method**: `POST`
- **Endpoint**: `/api/workspaces`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**: `CreateWorkspaceCommand`
  ```json
  {
    "name": "Engineering Team"
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<CreateWorkspaceResult>`
  ```json
  {
    "success": true,
    "message": "Workspace Created Successfully",
    "data": {
      "id": 1,
      "name": "Engineering Team",
      "createdBy": 1,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

#### 2.2 Get My Workspaces (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/my`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**: Inherits `PaginationRequest`
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `searchTerm` (string, optional)
  - `sortBy` (string, optional)
  - `descending` (bool, default: false)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetMyWorkspacesResult>>`
  ```json
  {
    "success": true,
    "message": "Retrieved all workspaces",
    "data": {
      "items": [
        {
          "id": 1,
          "name": "Engineering Team",
          "createdBy": 1,
          "createdAt": "2026-07-23T23:40:00Z",
          "userRole": 0,
          "memberCount": 5
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 2.3 Get Workspace By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workspaceId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<GetWorkspaceByIdResult>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "id": 1,
      "name": "Engineering Team",
      "createdBy": 1,
      "createdAt": "2026-07-23T23:40:00Z",
      "userRole": 0
    }
  }
  ```

#### 2.4 Get Workspace Members (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workspaceId` (int)
- **Query Parameters**: `PaginationRequest` (`pageNumber`, `pageSize`, `searchTerm`, etc.)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetWorkspaceMembersResult>>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "items": [
        {
          "userId": 1,
          "name": "John Doe",
          "email": "john@example.com",
          "role": 0,
          "joinedAt": "2026-07-23T23:40:00Z"
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 2.5 Add Workspace Member
- **HTTP Method**: `POST`
- **Endpoint**: `/api/workspaces/{workspaceId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workspaceId` (int)
- **Request Body**: `AddWorkspaceMemberCommand`
  ```json
  {
    "email": "member@example.com",
    "role": 2
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<AddWorkspaceMemberResult>`
  ```json
  {
    "success": true,
    "message": "Member Added Successfully",
    "data": {
      "id": 10,
      "workspaceId": 1,
      "userId": 2,
      "userEmail": "member@example.com",
      "userName": "Jane Smith",
      "role": 2,
      "joinedAt": "2026-07-23T23:45:00Z"
    }
  }
  ```

#### 2.6 Remove Workspace Member
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/workspaces/{workspaceId}/members/{userId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameters**: `workspaceId` (int), `userId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<RemoveWorkspaceMemberResult>`
  ```json
  {
    "success": true,
    "message": "Removed Successfully",
    "data": {
      "success": true
    }
  }
  ```

---

### MODULE 3: Project API (`/api/projects`)

#### 3.1 Create Project
- **HTTP Method**: `POST`
- **Endpoint**: `/api/projects`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**: `CreateProjectCommand`
  ```json
  {
    "name": "Mobile Application",
    "description": "Building the iOS and Android app",
    "workspaceId": 1
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<CreateProjectResult>`
  ```json
  {
    "success": true,
    "message": "Project Created Successfully",
    "data": {
      "id": 1,
      "name": "Mobile Application",
      "description": "Building the iOS and Android app",
      "workspaceId": 1,
      "createdBy": 1,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

#### 3.2 Get Project By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{Id}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `Id` (int)
- **Success Response (`200 OK`)**: `ApiResponse<GetProjectByIdResult>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "id": 1,
      "name": "Mobile Application",
      "description": "Building the iOS and Android app",
      "workspaceId": 1,
      "createdBy": 1,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

#### 3.3 Get Projects By Workspace (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/workspace/{workspaceId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workspaceId` (int)
- **Query Parameters**: `PaginationRequest` (`pageNumber`, `pageSize`, `searchTerm`, `sortBy`, `descending`)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetProjectsByWorkspaceResult>>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "items": [
        {
          "id": 1,
          "name": "Mobile Application",
          "description": "Building the iOS and Android app",
          "workspaceId": 1,
          "createdBy": 1,
          "createdAt": "2026-07-23T23:40:00Z",
          "taskCount": 12
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 3.4 Update Project
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/projects/{ProjectId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `ProjectId` (int)
- **Request Body**: `UpdateProjectCommand`
  ```json
  {
    "name": "Updated Project Name",
    "description": "Updated project description"
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<UpdateProjectResult>`
  ```json
  {
    "success": true,
    "message": "Updated Successfully",
    "data": {
      "id": 1,
      "name": "Updated Project Name",
      "description": "Updated project description",
      "workspaceId": 1,
      "createdBy": 1,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

---

### MODULE 4: Task API (`/api/tasks`)

#### 4.1 Create Task
- **HTTP Method**: `POST`
- **Endpoint**: `/api/tasks`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**: `CreateTaskCommand`
  ```json
  {
    "title": "Implement Login UI",
    "description": "Design login form screen",
    "projectId": 1,
    "dueDate": "2026-08-01T12:00:00Z",
    "priority": 1
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<CreateTaskResult>`
  ```json
  {
    "success": true,
    "message": "Task Created Successfully",
    "data": {
      "id": 101,
      "title": "Implement Login UI",
      "description": "Design login form screen",
      "projectId": 1,
      "createdBy": 1,
      "assignedToUserId": null,
      "createdAt": "2026-07-23T23:40:00Z",
      "dueDate": "2026-08-01T12:00:00Z",
      "priority": 1,
      "status": 0
    }
  }
  ```

#### 4.2 Get Task By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `taskId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<GetTaskByIdResult>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "id": 101,
      "title": "Implement Login UI",
      "description": "Design login form screen",
      "projectId": 1,
      "createdBy": 1,
      "creatorName": "John Doe",
      "assignedToUserId": 2,
      "assigneeName": "Jane Smith",
      "createdAt": "2026-07-23T23:40:00Z",
      "assignedAt": "2026-07-23T23:45:00Z",
      "dueDate": "2026-08-01T12:00:00Z",
      "completedAt": null,
      "priority": 1,
      "status": 0
    }
  }
  ```

#### 4.3 Get Tasks By Project (Paginated & Filtered)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/tasks/project/{projectId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `projectId` (int)
- **Query Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `searchTerm` (string, optional)
  - `sortBy` (string, optional)
  - `descending` (bool, default: false)
  - `status`: `TaskStatus` enum (0 = Todo, 1 = InProgress, 2 = Completed, optional)
  - `priority`: `TaskPriority` enum (0 = Low, 1 = Medium, 2 = High, optional)
  - `assignedToUserId`: `number` (int, optional)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetTasksByProjectResult>>`
  ```json
  {
    "success": true,
    "message": "Retrieved Successfully",
    "data": {
      "items": [
        {
          "id": 101,
          "title": "Implement Login UI",
          "description": "Design login form screen",
          "projectId": 1,
          "createdBy": 1,
          "creatorName": "John Doe",
          "assignedToUserId": 2,
          "assigneeName": "Jane Smith",
          "createdAt": "2026-07-23T23:40:00Z",
          "assignedAt": "2026-07-23T23:45:00Z",
          "dueDate": "2026-08-01T12:00:00Z",
          "completedAt": null,
          "priority": 1,
          "status": 0
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 4.4 Update Task Details
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `taskId` (int)
- **Request Body**: `UpdateTaskCommand`
  ```json
  {
    "title": "Updated Task Title",
    "description": "Updated detailed description",
    "dueDate": "2026-08-10T12:00:00Z",
    "priority": 2
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<UpdateTaskResult>`
  ```json
  {
    "success": true,
    "message": "Task Updated Successfully",
    "data": {
      "id": 101,
      "title": "Updated Task Title",
      "description": "Updated detailed description",
      "projectId": 1,
      "createdBy": 1,
      "assignedToUserId": 2,
      "createdAt": "2026-07-23T23:40:00Z",
      "dueDate": "2026-08-10T12:00:00Z",
      "priority": 2,
      "status": 0
    }
  }
  ```

#### 4.5 Update Task Status
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/tasks/{taskId}/status`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `taskId` (int)
- **Request Body**: `UpdateTaskStatusCommand`
  ```json
  {
    "status": 2
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<UpdateTaskStatusResult>`
  ```json
  {
    "success": true,
    "message": "Status Updated Successfully",
    "data": {
      "id": 101,
      "status": 2,
      "completedAt": "2026-07-23T23:50:00Z"
    }
  }
  ```

#### 4.6 Update Task Assignee
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/tasks/{taskId}/assignee`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `taskId` (int)
- **Request Body**: `UpdateTaskAssigneeCommand`
  ```json
  {
    "assignedToUserId": 2
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<UpdateTaskAssigneeResult>`
  ```json
  {
    "success": true,
    "message": "Assignee Updated Successfully",
    "data": {
      "id": 101,
      "assignedToUserId": 2,
      "assigneeName": "Jane Smith",
      "assignedAt": "2026-07-23T23:50:00Z"
    }
  }
  ```

#### 4.7 Delete Task
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `taskId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<DeleteTaskResult>`
  ```json
  {
    "success": true,
    "message": "Task Deleted Successfully",
    "data": {
      "taskId": 101,
      "success": true
    }
  }
  ```

---

### MODULE 5: Workflow Automation API (`/api/workflows`)

#### 5.1 Create Workflow Rule
- **HTTP Method**: `POST`
- **Endpoint**: `/api/workflows`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**: `CreateWorkflowCommand`
  ```json
  {
    "name": "Auto Notify on High Priority",
    "description": "Send notification whenever high priority task is assigned",
    "trigger": 0,
    "conditions": [
      {
        "field": "Priority",
        "operator": 0,
        "value": "High"
      }
    ],
    "actions": [
      {
        "actionType": 0,
        "parameters": "{\"Recipient\":0,\"Message\":\"High priority task assigned\"}",
        "order": 1
      }
    ]
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<CreateWorkflowResult>`
  ```json
  {
    "success": true,
    "message": "Workflow created successfully",
    "data": {
      "id": 5,
      "name": "Auto Notify on High Priority",
      "description": "Send notification whenever high priority task is assigned",
      "trigger": 0,
      "isEnabled": false,
      "createdAt": "2026-07-23T23:40:00Z"
    }
  }
  ```

#### 5.2 Get Workflow By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workflows/{workflowId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workflowId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<GetWorkflowByIdResult>`
  ```json
  {
    "success": true,
    "message": "Retrieved successfully",
    "data": {
      "id": 5,
      "name": "Auto Notify on High Priority",
      "description": "Send notification whenever high priority task is assigned",
      "trigger": 0,
      "isEnabled": true,
      "createdAt": "2026-07-23T23:40:00Z",
      "updatedAt": "2026-07-23T23:45:00Z",
      "conditions": [
        {
          "field": "Priority",
          "operator": 0,
          "value": "High"
        }
      ],
      "actions": [
        {
          "actionType": 0,
          "parameters": "{\"Recipient\":0,\"Message\":\"High priority task assigned\"}",
          "order": 1
        }
      ]
    }
  }
  ```

#### 5.3 Get All Workflows (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workflows`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `searchTerm` (string, optional)
  - `sortBy` (string, optional)
  - `descending` (bool, default: false)
  - `trigger`: `WorkflowTrigger` enum (optional)
  - `isEnabled`: `boolean` (optional)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetAllWorkflowsResult>>`
  ```json
  {
    "success": true,
    "message": "Retrieved successfully",
    "data": {
      "items": [
        {
          "id": 5,
          "name": "Auto Notify on High Priority",
          "description": "Send notification whenever high priority task is assigned",
          "trigger": 0,
          "isEnabled": true,
          "createdAt": "2026-07-23T23:40:00Z",
          "updatedAt": "2026-07-23T23:45:00Z",
          "conditionsCount": 1,
          "actionsCount": 1
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 5.4 Update Workflow
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/workflows/{workflowId}`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workflowId` (int)
- **Note**: Modifying conditions/actions requires the workflow to be disabled first.
- **Request Body**: `UpdateWorkflowCommand`
  ```json
  {
    "name": "Updated Workflow Name",
    "description": "Updated workflow description",
    "conditions": [ ... ],
    "actions": [ ... ]
  }
  ```
- **Success Response (`200 OK`)**: `ApiResponse<object>`
  ```json
  {
    "success": true,
    "message": "Workflow updated successfully",
    "data": null
  }
  ```

#### 5.5 Enable Workflow
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/workflows/{workflowId}/enable`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workflowId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<object>`
  ```json
  {
    "success": true,
    "message": "Workflow enabled successfully",
    "data": null
  }
  ```

#### 5.6 Disable Workflow
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/workflows/{workflowId}/disable`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `workflowId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<object>`
  ```json
  {
    "success": true,
    "message": "Workflow disabled successfully",
    "data": null
  }
  ```

---

### MODULE 6: Notification API (`/api/notifications`)

#### 6.1 Get Notifications (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/notifications`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `searchTerm` (string, optional)
  - `sortBy` (string, optional)
  - `descending` (bool, default: false)
  - `isRead`: `boolean` (filter read/unread notifications, optional)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetNotificationsResult>>`
  ```json
  {
    "success": true,
    "message": "Notifications Retrieved Successfully",
    "data": {
      "items": [
        {
          "id": 1,
          "userId": 1,
          "message": "Task 'Implement Login UI' assigned to you.",
          "type": 0,
          "isRead": false,
          "createdAt": "2026-07-23T23:45:00Z",
          "referenceId": 101
        }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalPages": 1,
      "totalCount": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  }
  ```

#### 6.2 Get Unread Notification Count
- **HTTP Method**: `GET`
- **Endpoint**: `/api/notifications/unread-count`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `ApiResponse<GetUnreadNotificationCountResult>`
  ```json
  {
    "success": true,
    "message": "Count Retrieved Successfully",
    "data": {
      "count": 3
    }
  }
  ```

#### 6.3 Mark Single Notification As Read
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/notifications/{notificationId}/read`
- **Authentication**: Required (`[Authorize]`)
- **Route Parameter**: `notificationId` (int)
- **Success Response (`200 OK`)**: `ApiResponse<MarkNotificationAsReadResult>`
  ```json
  {
    "success": true,
    "message": "Notification marked as read successfully",
    "data": {
      "id": 1,
      "isRead": true
    }
  }
  ```

#### 6.4 Mark All Notifications As Read
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/notifications/read-all`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `ApiResponse<MarkAllNotificationsAsReadResult>`
  ```json
  {
    "success": true,
    "message": "All Notification marked as read Successfully ",
    "data": {
      "markedCount": 3
    }
  }
  ```

---

## 8. Real-Time SignalR Websocket Event Reference

### Hub Endpoint
- **URL Path**: `/notificationHub` (Full URL: `http://localhost:5000/notificationHub`)

### Client Event Listener
When connected, listen to the `"ReceiveNotification"` event:

```typescript
connection.on("ReceiveNotification", (notification: NotificationRealtimeModel) => {
  console.log("Real-time notification received:", notification);
  // Payload fields:
  // - userId: number
  // - message: string
  // - type: NotificationType enum (0 = TaskAssigned, 1 = TaskCompleted, 2 = ProjectCreated, 3 = MemberAdded, 4 = Workflow)
  // - referenceId: number | null (Task Id or Project Id)
  // - createdAt: string (ISO Timestamp)
});
```

---

## 9. Quick Frontend Integration Summary Table

| Feature / Action | Method | Endpoint Path | Requires Auth Header |
| :--- | :--- | :--- | :--- |
| **User Login** | `POST` | `/api/Auth/Login` | No |
| **User Register** | `POST` | `/api/Auth/Register` | No |
| **Refresh Token** | `POST` | `/api/Auth/refresh` | No |
| **Logout** | `POST` | `/api/Auth/logout` | Yes |
| **Create Workspace** | `POST` | `/api/workspaces` | Yes |
| **Get My Workspaces** | `GET` | `/api/workspaces/my` | Yes |
| **Get Workspace Details** | `GET` | `/api/workspaces/{workspaceId}` | Yes |
| **List Workspace Members** | `GET` | `/api/workspaces/{workspaceId}/members` | Yes |
| **Add Workspace Member** | `POST` | `/api/workspaces/{workspaceId}/members` | Yes |
| **Remove Member** | `DELETE` | `/api/workspaces/{workspaceId}/members/{userId}` | Yes |
| **Create Project** | `POST` | `/api/projects` | Yes |
| **Get Project By ID** | `GET` | `/api/projects/{Id}` | Yes |
| **List Workspace Projects** | `GET` | `/api/projects/workspace/{workspaceId}` | Yes |
| **Update Project** | `PUT` | `/api/projects/{ProjectId}` | Yes |
| **Create Task** | `POST` | `/api/tasks` | Yes |
| **Get Task Details** | `GET` | `/api/tasks/{taskId}` | Yes |
| **List Project Tasks** | `GET` | `/api/tasks/project/{projectId}` | Yes |
| **Update Task Details** | `PUT` | `/api/tasks/{taskId}` | Yes |
| **Update Task Status** | `PATCH` | `/api/tasks/{taskId}/status` | Yes |
| **Update Task Assignee** | `PATCH` | `/api/tasks/{taskId}/assignee` | Yes |
| **Delete Task** | `DELETE` | `/api/tasks/{taskId}` | Yes |
| **Create Workflow** | `POST` | `/api/workflows` | Yes |
| **Get Workflow By ID** | `GET` | `/api/workflows/{workflowId}` | Yes |
| **List All Workflows** | `GET` | `/api/workflows` | Yes |
| **Update Workflow** | `PUT` | `/api/workflows/{workflowId}` | Yes |
| **Enable Workflow** | `PATCH` | `/api/workflows/{workflowId}/enable` | Yes |
| **Disable Workflow** | `PATCH` | `/api/workflows/{workflowId}/disable` | Yes |
| **Get Notifications** | `GET` | `/api/notifications` | Yes |
| **Unread Notification Count** | `GET` | `/api/notifications/unread-count` | Yes |
| **Mark Notification Read** | `PUT` | `/api/notifications/{notificationId}/read` | Yes |
| **Mark All Notifications Read** | `PUT` | `/api/notifications/read-all` | Yes |
| **SignalR Realtime Hub** | `WS/HTTP` | `/notificationHub` | Yes (`access_token`) |
