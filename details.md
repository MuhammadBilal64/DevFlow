# ⚡ DevFlow Integration & API Reference (`details.md`)

<p align="center">
  <img src="https://raw.githubusercontent.com/lucide-icons/lucide/main/icons/server.svg" width="90" alt="DevFlow API Logo" />
</p>

<h3 align="center">Definitive Production-Accurate Frontend & API Integration Reference</h3>

<p align="center">
  This document serves as the complete technical blueprint for integrating frontend applications (React, Vite, Next.js) with the <strong>DevFlow ASP.NET Core (.NET 9) Backend API</strong>.
</p>

---

## 📋 Table of Contents

- [1. Core Architecture & Environment Basics](#1-core-architecture--environment-basics)
- [2. Global Response & Error Envelopes](#2-global-response--error-envelopes)
- [3. Frontend Axios & SignalR WebSockets Client Setup](#3-frontend-axios--signalr-websockets-client-setup)
- [4. Complete Enums Reference](#4-complete-enums-reference)
- [5. Common Utility Models & DTOs](#5-common-utility-models--dtos)
- [6. Comprehensive API Endpoint Specifications](#6-comprehensive-api-endpoint-specifications)
  - [Module 1: Auth API (`/api/Auth`)](#module-1-auth-api-apiauth)
  - [Module 2: Workspace API (`/api/workspaces`)](#module-2-workspace-api-apiworkspaces)
  - [Module 3: Project API (`/api/projects`)](#module-3-project-api-apiprojects)
  - [Module 4: Project Members API (`/api/projects/{projectId}/members`)](#module-4-project-members-api-apiprojectsprojectidmembers)
  - [Module 5: Task API (`/api/projects/{projectId}/tasks`)](#module-5-task-api-apiprojectsprojectidtasks)
  - [Module 6: Workflow Automation API (`/api/projects/{projectId}/workflows`)](#module-6-workflow-automation-api-apiprojectsprojectidworkflows)
  - [Module 7: Notification API (`/api/notifications`)](#module-7-notification-api-apinotifications)
- [7. Key Best Practices for Frontend Integration](#7-key-best-practices-for-frontend-integration)

---

## 1. Core Architecture & Environment Basics

- **Backend Framework**: .NET 9 ASP.NET Core Web API (Clean Architecture with MediatR CQRS)
- **Base Backend HTTP URL**: `http://localhost:5000` / `https://localhost:7001`
- **SignalR WebSocket URL**: `http://localhost:5000/notificationHub`
- **Allowed CORS Origins**: `http://localhost:5173`, `http://localhost:5174`, `http://localhost:3000`
- **Default Content-Type**: `application/json`

---

## 2. Global Response & Error Envelopes

### 2.1 Standard API Envelope (`ApiResponse<T>`)
All successful HTTP responses return data wrapped inside the standardized `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "Operation response message",
  "data": { ... } // Object, PagedResult<T>, Array, or null
}
```

### 2.2 Global Exception & Error Envelopes
When an exception occurs, the server responds with standardized JSON error objects:

#### 1. Validation Error (`400 Bad Request`)
Triggered when request bodies fail FluentValidation rules:
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
- `401 Unauthorized`: Token missing, expired, or invalid credentials (`UnauthorizedException`)
- `403 Forbidden`: Insufficient role or workspace/project permissions (`ForbiddenException`)
- `404 Not Found`: Target entity (Task, Project, Workspace, User, Workflow) does not exist (`NotFoundException`)
- `409 Conflict`: Resource state collision or duplicate member (`ConflictException`)

```json
{
  "message": "User is not a member of this workspace"
}
```

---

## 3. Frontend Axios & SignalR WebSockets Client Setup

### 3.1 Axios Client with JWT Token & Auto-Refresh Interceptor

```javascript
// src/api/axiosClient.js
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach Bearer JWT Access Token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => Promise.reject(error));

// Response Interceptor: Handle 401 & Refresh Token Automatically
api.interceptors.response.use(
  (response) => response.data,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const res = await axios.post(`${API_BASE_URL}/api/Auth/refresh`, { refreshToken });
          if (res.data?.success) {
            const { token, refreshToken: newRefreshToken } = res.data.data;
            localStorage.setItem('token', token);
            localStorage.setItem('refreshToken', newRefreshToken);
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return api(originalRequest);
          }
        }
      } catch (refreshError) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error.response?.data || error);
  }
);

export default api;
```

### 3.2 SignalR WebSockets Client Hook (React)

Install `@microsoft/signalr`: `npm install @microsoft/signalr`

```javascript
// src/hooks/useSignalRNotification.js
import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

export const useSignalRNotification = (onNotificationReceived) => {
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/notificationHub', {
        accessTokenFactory: () => localStorage.getItem('token'),
      })
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveNotification', (notification) => {
      console.log('Real-time notification received:', notification);
      if (onNotificationReceived) {
        onNotificationReceived(notification);
      }
    });

    connection.start().catch((err) => console.error('SignalR connection error:', err));

    return () => {
      connection.stop();
    };
  }, [onNotificationReceived]);
};
```

---

## 4. Complete Enums Reference

> ⚠️ **CRITICAL FOR FRONTEND**: Always send enum values as **integers** in API requests.

### `UserRole`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `Admin` | `0` | Platform Administrator |
| `Manager` | `1` | Workspace/Project Manager |
| `Member` | `2` | Standard User |

### `WorkspaceRole`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `Owner` | `0` | Workspace Owner (Full Admin Rights) |
| `Admin` | `1` | Workspace Administrator |
| `Member` | `2` | Workspace Member |

### `ProjectRole`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `Owner` | `0` | Project Owner |
| `Admin` | `1` | Project Administrator |
| `Member` | `2` | Project Member |

### `TaskPriority`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `Low` | `0` | Low Priority Task |
| `Medium` | `1` | Normal / Medium Priority |
| `High` | `2` | High / Urgent Priority |

### `TaskStatus`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `Todo` | `0` | Backlog / Todo Column |
| `InProgress` | `1` | In Progress Column |
| `Completed` | `2` | Completed / Done Column |

### `NotificationType`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | Task assigned to user |
| `TaskCompleted` | `1` | Task marked completed |
| `ProjectCreated` | `2` | Project created |
| `MemberAdded` | `3` | Member added to workspace/project |
| `Workflow` | `4` | Triggered by automation engine |

### `WorkflowTrigger`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `TaskAssigned` | `0` | Event when task assignee updates |
| `TaskCompleted` | `1` | Event when task status changes to Completed |
| `ProjectCreated` | `2` | Event when a new project is created |

### `WorkflowActionType`
| Name | Integer | Description |
| :--- | :--- | :--- |
| `NotifyUser` | `0` | Send persistent DB + real-time SignalR notification |

### `WorkflowOperator`
| Name | Integer | Description |
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
- `pageNumber`: `number` (default: `1`)
- `pageSize`: `number` (default: `10`)
- `searchTerm`: `string | null` (optional search filter)
- `sortBy`: `string | null` (property to sort by)
- `descending`: `boolean` (default: `false`)

### `PagedResult<T>` (Response Model)
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
  "field": "Priority", // Target property (e.g., "Priority", "Status", "Title")
  "operator": 0,      // WorkflowOperator enum (0 = Equals, 6 = Contains, etc.)
  "value": "2"        // Comparison target value
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
  type: NotificationType; // 0-4
  referenceId: number | null;
  createdAt: string; // ISO DateTime
}
```

---

## 6. Comprehensive API Endpoint Specifications

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

#### 2.2 Get My Workspaces (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/my`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**: `pageNumber`, `pageSize`, `searchTerm`, `sortBy`, `descending`

#### 2.3 Get Workspace By ID
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}`
- **Authentication**: Required (`[Authorize]`)

#### 2.4 Get Workspace Members (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/workspaces/{workspaceId}/members`
- **Authentication**: Required (`[Authorize]`)

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
    "description": "React & Tailwind CSS frontend application",
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
    "description": "Updated project description"
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

#### 4.3 Remove Project Member
- **HTTP Method**: `DELETE`
- **Endpoint**: `/api/projects/{projectId}/members/{userId}`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 5: Task API (`/api/projects/{projectId}/tasks`)

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

### MODULE 6: Workflow Automation API (`/api/projects/{projectId}/workflows`)

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

#### 6.5 Enable Workflow Rule
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}/enable`
- **Authentication**: Required (`[Authorize]`)

#### 6.6 Disable Workflow Rule
- **HTTP Method**: `PATCH`
- **Endpoint**: `/api/projects/{projectId}/workflows/{workflowId}/disable`
- **Authentication**: Required (`[Authorize]`)

---

### MODULE 7: Notification API (`/api/notifications`)

#### 7.1 Get User Notifications (Paginated)
- **HTTP Method**: `GET`
- **Endpoint**: `/api/notifications`
- **Authentication**: Required (`[Authorize]`)
- **Query Parameters**: `pageNumber`, `pageSize`, `isRead`

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

## 7. Key Best Practices for Frontend Integration

1. **Token Refresh Interceptor**: Always implement a response interceptor for `401 Unauthorized` errors that automatically calls `POST /api/Auth/refresh` using the `refreshToken`.
2. **WebSocket Handshake Token**: Ensure the JWT access token is passed in the query string `?access_token=` during the SignalR connection initialization (`/notificationHub`).
3. **Enum Values**: Convert dropdown values to integer representations before dispatching HTTP payloads to avoid model state validation failures.
4. **Kanban Status Updates**: Use `PATCH /api/projects/{projectId}/tasks/{taskId}/status` when moving tasks between board columns.
