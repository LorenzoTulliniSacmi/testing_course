import { Routes } from '@angular/router';
import { KanbanBoardComponent } from './components/kanban-board/kanban-board';

export const routes: Routes = [
  { path: '', component: KanbanBoardComponent },
  { path: '**', redirectTo: '' }
];
