# DevFlow Frontend Integration Reference (`details.md`)

This document is the **definitive, production-accurate technical reference** for building and connecting the **DevFlow Frontend** (React / Vite / Next.js) to the **DevFlow Backend API**.

It contains all API endpoints, exact route structures, HTTP methods, request/response models, query parameters, enums, error contracts, and SignalR WebSocket specs.

---

## 1. Core Architecture & Environment Basics

- **Framework**: .NET 9 ASP.NET Core Web API (Clean Architecture with MediatR CQRS)
- **Base HTTP URL**: `http://localhost:5000` / `https://localhost:7001`
- **SignalR WebSockets URL**: `http://localhost:5000/notificationHub`
- **Allowed CORS Origin**: `http://localhost:5173` (Vite / React default)
- **Content-Type**: `application/json`

---

## 2. Global Response & Error Envelopes

### 2.1 Standard API Envelope (`ApiResponse<T>`)
All successful HTTP responses return data wrapped inside the `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "Operation response message",
  "data": { ... } // Single Object, PagedResult<T>, Array, or null
}
```

### 2.2 Global Exception & Error Envelopes
When an error occurs, the API returns standardized error structures depending on the exception type:

#### 1. Validation Error (`400 Bad Request`)
Occurs when request parameters or body fail FluentValidation rules:
```json
{
  "message": "One or more validation errors occurred.",
  "errors": [
    "Title is required.",
    "Priority must be a valid enum value."
  ]
}
```

#### 2. Domain & Business Errors (`401`, `403`, `404`, `409`)
- `401 Unauthorized`: Missing token, expired token, or bad credentials (`UnauthorizedException`)
- `403 Forbidden`: Insufficient role or membership permissions (`ForbiddenException`)
- `404 Not Found`: Target entity does not exist (`NotFoundException`)
- `409 Conflict`: Resource state collision or duplicate member (`ConflictException`)

```json
{
  "message": "User is not a member of this workspace"
}
```

---

## 3. Authentication & SignalR Setup

### 3.1 Authorization Header
All protected HTTP endpoints require the Bearer token header:
```http
Authorization: Bearer <JWT_ACCESS_TOKEN>
```

### 3.2 SignalR Connection Setup (TypeScript / JavaScript)
To receive real-time notifications, connect using `@microsoft/signalr` passing the access token:

```typescript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/notificationHub", {
    accessTokenFactory: () => accessToken // Returns current JWT token string
  })
  .withAutomaticReconnect()
  .build();

// Listen for incoming notifications
connection.on("ReceiveNotification", (notification: NotificationRealtimeModel) => {
  console.log("Real-time notification received:", notification);
});

await connection.start();
```

---

## 4. Complete Enums Reference

> **CRITICAL FOR FRONTEND**: Enum values should be sent as **integers** (or matching names if configured). Always use the exact integer value when creating or updating resources.

### `UserRole`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Admin` | `0` | Platform Administrator |
| `Manager` | `1` | Manager |
| `Member` | `2` | Standard User |

### `WorkspaceRole`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Owner` | `0` | Workspace Owner (Full Admin Rights) |
| `Admin` | `1` | Workspace Administrator |
| `Member` | `2` | Workspace Member |

### `ProjectRole`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Owner` | `0` | Project Owner |
| `Admin` | `1` | Project Administrator |
| `Member` | `2` | Project Member |

### `TaskPriority`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Low` | `0` | Low Priority Task |
| `Medium` | `1` | Normal / Medium Priority |
| `High` | `2` | High / Urgent Priority |

### `TaskStatus`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Todo` | `0` | Backlog / Todo |
| `InProgress` | `1` | Actively Working |
| `Completed` | `2` | Completed / Done |

### `NotificationType`
| Name | Value | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | Task assigned to user |
| `TaskCompleted` | `1` | Task marked completed |
| `ProjectCreated` | `2` | Project created |
| `MemberAdded` | `3` | Added to workspace/project |
| `Workflow` | `4` | Triggered by automation rule |

### `WorkflowTrigger`
| Name | Value | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | Event when task assignee updates |
| `TaskCompleted` | `1` | Event when task status changes to Completed |
| `ProjectCreated` | `2` | Event when a project is created |

### `WorkflowActionType`
| Name | Value | Description |
| :--- | :--- | :--- |
| `NotifyUser` | `0` | Send persistent + SignalR notification |

### `WorkflowOperator`
| Name | Value | Description |
| :--- | :--- | :--- |
| `Equals` | `0` | `==` exact match |
| `NotEquals` | `1` | `!=` not equal |
| `GreaterThan` | `2` | `>` numeric / date greater |
| `LessThan` | `3` | `<` numeric / date less |
| `GreaterThanOrEqual` | `4` | `>=` greater or equal |
| `LessThanOrEqual` | `5` | `<=` less or equal |
| `Contains` | `6` | Substring match |

---

## 5. Common Utility Models & DTOs

### `PaginationRequest` (Query Parameters)
Used across paginated `GET` endpoints:
- `pageNumber`: `number` (integer, default: `1`)
- `pageSize`: `number` (integer, default: `10`)
- `searchTerm`: `string | null` (optional search keyword)
- `sortBy`: `string | null` (property name to sort by)
- `descending`: `boolean` (default: `false`)

### `PagedResult<T>` (Pagination Response Wrapper)
```json
{
  "items": [],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1,
  "totalCount": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

### `WorkflowConditionDto`
```json
{
  "field": "Priority", // Target field name (e.g. "Priority", "Status", "Title")
  "operator": 0,      // WorkflowOperator enum (0 = Equals, 6 = Contains, etc.)
  "value": "2"        // Comparison value string
}
```

### `WorkflowActionDto`
```json
{
  "actionType": 0, // WorkflowActionType enum (0 = NotifyUser)
  "parameters": "{\"Recipient\":0,\"Message\":\"Task assigned to you\"}", // JSON string payload
  "order": 1
}
```

### `NotificationRealtimeModel` (SignalR WebSocket Payload)
```typescript
interface NotificationRealtimeModel {
  userId: number;
  message: string;
  type: NotificationType; // enum 0-4
  referenceId: number | null;
  createdAt: string; // ISO DateTime string
}
```

---

## 6. Comprehensive API Endpoint Specification

---

### MODULE 1: Auth API (`/api/Auth`)

#### 1.1 Register User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/Register`
- **Authentication**: Public
- **Request Body**:
  ```json
  {
    "name": "Jane Doe",
    "email": "jane@example.com",
    "password": "Password123!",
    "role": 2
  }
  ```
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Register Successfully",
    "data": {
      "id": 1,
      "name": "Jane Doe",
      "email": "jane@example.com",
      "role": 2,
      "createdAt": "2026-07-31T18:00:00Z"
    }
  }
  ```

#### 1.2 Login User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/Login`
- **Authentication**: Public
- **Request Body**:
  ```json
  {
    "email": "jane@example.com",
    "password": "Password123!"
  }
  ```
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Login Successfull",
    "data": {
      "token": "eyJhbGciOiJIUzI1Ni...",
      "refreshToken": "a4b2c3...",
      "refreshTokenExpiresAt": "2026-08-07T18:00:00Z"
    }
  }
  ```

#### 1.3 Refresh Access Token
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/refresh`
- **Authentication**: Public
- **Request Body**:
  ```json
  {
    "refreshToken": "a4b2c3..."
  }
  ```
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Token Refreshed Successfully",
    "data": {
      "token": "eyJhbGciOiJIUzI1Ni...",
      "refreshToken": "f8e7d6...",
      "refreshTokenExpiresAt": "2026-08-07T18:00:00Z"
    }
  }
  ```

#### 1.4 Logout User
- **HTTP Method**: `POST`
- **Endpoint**: `/api/Auth/logout`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "refreshToken": "f8e7d6..."
  }
  ```
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Log out Successfully",
    "data": null
  }
  ```

---

### MODULE 2: Workspace API (`/api/workspaces`)

#### 2.1 Create Workspace
- **HTTP Method**: `POST`
- **Endpoint**: `/api/workspaces`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "name": "Frontend Engineering"
  }
  ```
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Workspace Created Successfully",
    "data": {
      "id": 1,
      "name": "Frontend Engineering",
      "createdBy": 1,
      "createdAt": "2026-07-31T18:00:00Z"
    }
  }
  ```

#### 2.2 Get My Workspaces (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/my`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**: `pageNumber`, `pageSize`, `searchTerm`, `sortBy`, `descending`
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetMyWorkspacesResult>>`

#### 2.3 Get Workspace By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `ApiResponse<GetWorkspaceByIdResult>`

#### 2.4 Get Workspace Members (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `ApiResponse<PagedResult<GetWorkspaceMembersResult>>`

#### 2.5 Add Workspace Member
- **HTTP Method**: `POST`
- **Endpoint**: `/api/workspaces/{workspaceId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "email": "developer@example.com",
    "role": 2 // WorkspaceRole (0 = Owner, 1 = Admin, 2 = Member)
  }
  ```

#### 2.6 Remove Workspace Member
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/workspaces/{workspaceId}/members/{userId}`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 3: Project API (`/api/projects`)

#### 3.1 Create Project
- **HTTP Method**: `POST`
- **Endpoint**: `/api/projects`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "name": "DevFlow Web Client",
    "description": "React & TypeScript frontend application",
    "workspaceId": 1
  }
  ```

#### 3.2 Get Project By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{Id}`
- **Authentication**: Required (`[Authorize]`)

#### 3.3 Get Projects By Workspace (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/workspace/{workspaceId}`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**: `pageNumber`, `pageSize`, `searchTerm`, `sortBy`, `descending`

#### 3.4 Update Project
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/projects/{ProjectId}`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "name": "Updated Project Name",
    "description": "Updated description"
  }
  ```

---

### MODULE 4: Project Members API (`/api/projects/{projectId}/members`)

#### 4.1 Add Project Member
- **HTTP Method**: `POST`
- **Endpoint**: `/api/projects/{projectId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "userId": 2,
    "role": 2 // ProjectRole (0 = Owner, 1 = Admin, 2 = Member)
  }
  ```

#### 4.2 Get Project Members
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{projectId}/members`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**: `ApiResponse<List<GetProjectMembersResult>>`

#### 4.3 Remove Project Member
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/projects/{projectId}/members/{userId}`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 5: Task API (`/api/projects/{projectId}/tasks`)

> **IMPORTANT**: All task endpoints are scoped under `/api/projects/{projectId}/tasks`.

#### 5.1 Create Task
- **HTTP Method**: `POST`
- **Endpoint**: `/api/projects/{projectId}/tasks`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "title": "Design Workspace Dashboard",
    "description": "Create modern UI layout with stats",
    "dueDate": "2026-08-15T12:00:00Z",
    "priority": 2 // TaskPriority (0 = Low, 1 = Medium, 2 = High)
  }
  ```

#### 5.2 Get Tasks By Project (Paginated & Filtered)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{projectId}/tasks`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `searchTerm` (string, optional)
  - `sortBy` (string, optional)
  - `descending` (bool, default: false)
  - `status` (TaskStatus enum: 0 = Todo, 1 = InProgress, 2 = Completed, optional)
  - `priority` (TaskPriority enum: 0 = Low, 1 = Medium, 2 = High, optional)
  - `assignedToUserId` (int, optional)

#### 5.3 Get Task By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{projectId}/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)

#### 5.4 Update Task Details
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/projects/{projectId}/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "title": "Updated Task Title",
    "description": "Updated details",
    "dueDate": "2026-08-20T12:00:00Z",
    "priority": 1
  }
  ```

#### 5.5 Update Task Status (Kanban Drag & Drop)
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/tasks/{taskId}/status`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "status": 1 // TaskStatus (0 = Todo, 1 = InProgress, 2 = Completed)
  }
  ```

#### 5.6 Update Task Assignee
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/tasks/{taskId}/assignee`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "assignedToUserId": 2 // User ID to assign (or null to unassign)
  }
  ```

#### 5.7 Delete Task
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/projects/{projectId}/tasks/{taskId}`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 6: Workflow API (`/api/projects/{projectId}/workflows`)

> **IMPORTANT**: Workflow management endpoints are scoped under `/api/projects/{projectId}/workflows`.

#### 6.1 Create Workflow Rule
- **HTTP Method**: `POST`
- **Endpoint**: `/api/projects/{projectId}/workflows`
- **Authentication**: Required (`[Authorize]`)
- **Request Body**:
  ```json
  {
    "name": "Auto Notify Assignee on High Priority Task",
    "description": "Sends notification when high priority task is assigned",
    "trigger": 0, // WorkflowTrigger (0 = TaskAssigned, 1 = TaskCompleted, 2 = ProjectCreated)
    "isEnabled": true,
    "conditions": [
      {
        "field": "Priority",
        "operator": 0, // Equals
        "value": "2"   // High Priority
      }
    ],
    "actions": [
      {
        "actionType": 0, // NotifyUser
        "parameters": "{\"Recipient\":0,\"Message\":\"High priority task assigned to you\"}",
        "order": 1
      }
    ]
  }
  ```

#### 6.2 Get Workflows By Project (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{projectId}/workflows`
- **Authentication**: Required (`[Authorize]`)

#### 6.3 Get Workflow By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}`
- **Authentication**: Required (`[Authorize]`)

#### 6.4 Update Workflow Rule
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}`
- **Authentication**: Required (`[Authorize]`)

#### 6.5 Enable Workflow
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}/enable`
- **Authentication**: Required (`[Authorize]`)

#### 6.6 Disable Workflow
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}/disable`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 7: Notification API (`/api/notifications`)

#### 7.1 Get User Notifications (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/notifications`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**:
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `isRead` (boolean, optional filter)

#### 7.2 Get Unread Notification Count
- **HTTP Method**: `GET`
- **Endpoint**: `/api/notifications/unread-count`
- **Authentication**: Required (`[Authorize]`)
- **Success Response (`200 OK`)**:
  ```json
  {
    "success": true,
    "message": "Count Retrieved Successfully",
    "data": {
      "unreadCount": 5
    }
  }
  ```

#### 7.3 Mark Single Notification as Read
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/notifications/{notificationId}/read`
- **Authentication**: Required (`[Authorize]`)

#### 7.4 Mark All Notifications as Read
- **HTTP Method**: `PUT`
- **Endpoint**: `/api/notifications/read-all`
- **Authentication**: Required (`[Authorize]`)

---

## 7. Key Best Practices for Frontend Agents & Developers

1. **JWT Storage & Auto-Refresh**:
   - Store `token` and `refreshToken` securely in browser local storage or memory.
   - Attach an HTTP Axios / Fetch interceptor to append `Authorization: Bearer <token>` on all API requests.
   - When an API request returns `401 Unauthorized`, call `POST /api/Auth/refresh` using the stored `refreshToken` to obtain a fresh access token seamlessly.

2. **SignalR WebSocket Lifecycle**:
   - Initialize SignalR connection upon successful user login using `@microsoft/signalr`.
   - Pass the token via `accessTokenFactory`.
   - On `"ReceiveNotification"` event, update in-app notification badge counts and show toast notifications dynamically without forcing a page refresh.

3. **Kanban Drag & Drop**:
   - Use `PATCH /api/projects/{projectId}/tasks/{taskId}/status` with body `{ "status": newStatusInt }` when dragging tasks between `Todo` (0), `InProgress` (1), and `Completed` (2) columns.
