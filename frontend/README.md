# Angular Testing Course - Kanban Board

A demo application for learning Angular testing (unit, integration, and E2E tests).

Built with **Angular 20**, featuring:
- ✅ Signals for state management
- ✅ `inject()` function (no constructor injection)
- ✅ Standalone components
- ✅ Minimal, pleasant SCSS styling

## Quick Start

```bash
# Install dependencies
npm install

# Start development server
npm start
# Open http://localhost:4200

# Run unit/integration tests
npm test

# Run E2E tests (Playwright)
npx playwright test
```

## Project Structure

```
src/app/
├── models/           # Task, Column interfaces
├── services/         # TaskService with signals
└── components/
    ├── kanban-board/ # Main board component
    ├── task-column/  # Column with drag-drop
    ├── task-card/    # Individual task card
    └── task-form/    # Add/edit task modal
```

## Course Exercises

This project contains **18 exercises** covering:

| Type | Exercises | Topics |
|------|-----------|--------|
| Unit | 1-4 | Service methods, signal testing |
| Integration | 5-14 | Component testing, mocking, forms |
| E2E | 15-18 | Playwright, user flows |

📖 See **[EXERCISES.md](./EXERCISES.md)** for detailed instructions.

🔑 Instructor solutions in **[SOLUTIONS.md](./SOLUTIONS.md)**.

## Running Tests

```bash
# Unit/Integration tests (Jasmine)
npm test                    # Run once
npm test -- --watch         # Watch mode

# E2E tests (Playwright)
npx playwright test         # Headless
npx playwright test --ui    # Interactive UI
npx playwright test --headed # See browser
```

## Features

- **Kanban Board**: Three columns (To Do, In Progress, Done)
- **Task Management**: Create, edit, delete tasks
- **Drag & Drop**: Move tasks between columns
- **Priority Levels**: High, medium, low with visual indicators
- **Persistence**: Tasks saved to localStorage
- **Responsive**: Works on mobile and desktop
