import { Routes } from '@angular/router';
import { KanbanBoardComponent } from './components/kanban-board/kanban-board';
import { ArchiveComponent } from './components/archive/archive';

export const routes: Routes = [
  { path: '', component: KanbanBoardComponent },
  { path: 'archive', component: ArchiveComponent },
  { path: '**', redirectTo: '' }
];
