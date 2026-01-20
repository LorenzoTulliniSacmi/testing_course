# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

```bash
npm start              # Start dev server at http://localhost:4200
npm run build          # Production build
npm test               # Run unit/integration tests (Vitest)
npm test -- --watch    # Run tests in watch mode
npx playwright test    # Run E2E tests (headless)
npx playwright test --ui    # Run E2E tests with interactive UI
npx playwright test --headed  # Run E2E tests in visible browser
```

## Project Purpose

This is a course demo application for teaching Angular frontend testing (unit, integration, and E2E tests). The codebase contains exercise stubs (methods throwing "Not implemented" errors) that students complete. See `EXERCISES.md` for instructions and `SOLUTIONS.md` for reference implementations.

## Architecture Overview

Angular 21 Kanban board using signals for state management, standalone components, and the `inject()` function.

**Data Flow:**
- `TaskService` holds all state via signals (`tasks`, `columns`, `taskCounts`)
- Components consume computed signals from the service
- API calls go through `HttpClient` to `http://localhost:3000/api/tasks`

**Component Hierarchy:**
```
App
└── KanbanBoardComponent (main container)
    ├── TaskColumnComponent (one per status: todo, in-progress, done)
    │   └── TaskCardComponent (one per task)
    └── TaskFormComponent (modal for add/edit)
```

**Key Files:**
- `src/app/services/task.service.ts` - Central state management with signals
- `src/app/models/task.model.ts` - Task, Column, TaskStatus, TaskPriority types
- `playwright.config.ts` - E2E tests configured in `./e2e` directory

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Use signals for state management
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

### Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- Prefer Reactive forms instead of Template-driven ones
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.
- Do not write arrow functions in templates (they are not supported).

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection
