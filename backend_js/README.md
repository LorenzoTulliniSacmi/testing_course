# Kanban Backend API

Simple REST API backend for the Kanban Board frontend testing course.

## Getting Started

```bash
npm install
npm run dev     # Development with hot reload
npm run build   # Build for production
npm start       # Run production build
```

Server runs on `http://localhost:3000`

## Storage Options

### JSON File (default)
```bash
npm run dev
# or explicitly
STORAGE_TYPE=json npm run dev
```

### MongoDB
```bash
STORAGE_TYPE=mongo MONGO_URI=mongodb://localhost:27017/kanban npm run dev
```

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `PORT` | `3000` | Server port |
| `STORAGE_TYPE` | `json` | Storage backend: `json` or `mongo` |
| `MONGO_URI` | `mongodb://localhost:27017/kanban` | MongoDB connection string |

## API Endpoints

### Tasks

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | Get all tasks (with filtering/ordering) |
| GET | `/api/tasks/:id` | Get task by ID |
| POST | `/api/tasks` | Create a new task |
| PUT | `/api/tasks/:id` | Full update (all fields required) |
| PATCH | `/api/tasks/:id` | Partial update (any fields) |
| DELETE | `/api/tasks/:id` | Delete a task |

### Query Parameters (GET /api/tasks)

| Parameter | Values | Description |
|-----------|--------|-------------|
| `status` | `todo`, `in-progress`, `done` | Filter by status |
| `priority` | `low`, `medium`, `high` | Filter by priority |
| `archived` | `true`, `false`, `all` | Filter by archived state (default: `false`) |
| `search` | string | Search by title (case-insensitive) |
| `orderBy` | `createdAt`, `updatedAt`, `priority`, `title` | Sort field |
| `order` | `asc`, `desc` | Sort direction |

### Task Object

```json
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "status": "todo" | "in-progress" | "done",
  "priority": "low" | "medium" | "high",
  "archived": boolean,
  "createdAt": "ISO date string",
  "updatedAt": "ISO date string"
}
```

### Create Task (POST /api/tasks)

```json
{
  "title": "string (required)",
  "description": "string (required)",
  "priority": "low" | "medium" | "high" (optional, default: "medium")
}
```

### Update Task (PUT /api/tasks/:id)

All fields optional:
```json
{
  "title": "string",
  "description": "string",
  "status": "todo" | "in-progress" | "done",
  "priority": "low" | "medium" | "high",
  "archived": boolean
}
```

## Examples

```bash
# Get all non-archived tasks
curl http://localhost:3000/api/tasks

# Get archived tasks only
curl http://localhost:3000/api/tasks?archived=true

# Get all tasks (including archived)
curl http://localhost:3000/api/tasks?archived=all

# Filter by status and order by priority
curl "http://localhost:3000/api/tasks?status=todo&orderBy=priority&order=desc"

# Search tasks
curl "http://localhost:3000/api/tasks?search=feature"

# Create a task
curl -X POST http://localhost:3000/api/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"New Task","description":"Task description","priority":"high"}'

# Partial update - change status only (PATCH)
curl -X PATCH http://localhost:3000/api/tasks/[id] \
  -H "Content-Type: application/json" \
  -d '{"status":"in-progress"}'

# Archive a task (PATCH with archived field)
curl -X PATCH http://localhost:3000/api/tasks/[id] \
  -H "Content-Type: application/json" \
  -d '{"archived":true}'

# Unarchive a task
curl -X PATCH http://localhost:3000/api/tasks/[id] \
  -H "Content-Type: application/json" \
  -d '{"archived":false}'

# Full update (PUT - all fields required)
curl -X PUT http://localhost:3000/api/tasks/[id] \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated","description":"New desc","status":"done","priority":"high"}'

# Delete a task
curl -X DELETE http://localhost:3000/api/tasks/[id]
```
