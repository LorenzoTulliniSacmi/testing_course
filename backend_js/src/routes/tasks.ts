import { Router, Request, Response } from 'express';
import { CreateTaskDto, UpdateTaskDto, TaskQueryParams, PatchTaskDto } from '../models/task.js';
import { TaskRepository } from '../repositories/task.repository.js';

export function createTaskRouter(repository: TaskRepository): Router {
  const router = Router();

  /**
   * @swagger
   * /api/tasks:
   *   get:
   *     summary: Get all tasks
   *     description: Retrieve tasks with optional filtering, searching, and ordering
   *     tags: [Tasks]
   *     parameters:
   *       - in: query
   *         name: status
   *         schema:
   *           type: string
   *           enum: [todo, in-progress, done]
   *         description: Filter by status
   *       - in: query
   *         name: priority
   *         schema:
   *           type: string
   *           enum: [low, medium, high]
   *         description: Filter by priority
   *       - in: query
   *         name: archived
   *         schema:
   *           type: string
   *           enum: ['true', 'false', 'all']
   *           default: 'false'
   *         description: Filter by archived status (default shows non-archived)
   *       - in: query
   *         name: search
   *         schema:
   *           type: string
   *         description: Search by title (case-insensitive)
   *       - in: query
   *         name: orderBy
   *         schema:
   *           type: string
   *           enum: [createdAt, updatedAt, priority, title]
   *         description: Field to sort by
   *       - in: query
   *         name: order
   *         schema:
   *           type: string
   *           enum: [asc, desc]
   *           default: asc
   *         description: Sort order
   *     responses:
   *       200:
   *         description: List of tasks
   *         content:
   *           application/json:
   *             schema:
   *               type: array
   *               items:
   *                 $ref: '#/components/schemas/Task'
   */
  router.get('/', async (req: Request<{}, {}, {}, TaskQueryParams>, res: Response) => {
    try {
      const tasks = await repository.findAll(req.query);
      res.json(tasks);
    } catch (error) {
      res.status(500).json({ error: 'Failed to fetch tasks' });
    }
  });

  /**
   * @swagger
   * /api/tasks/{id}:
   *   get:
   *     summary: Get task by ID
   *     tags: [Tasks]
   *     parameters:
   *       - in: path
   *         name: id
   *         required: true
   *         schema:
   *           type: string
   *         description: Task ID
   *     responses:
   *       200:
   *         description: Task found
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Task'
   *       404:
   *         description: Task not found
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Error'
   */
  router.get('/:id', async (req: Request<{ id: string }>, res: Response) => {
    try {
      const task = await repository.findById(req.params.id);
      if (!task) {
        res.status(404).json({ error: 'Task not found' });
        return;
      }
      res.json(task);
    } catch (error) {
      res.status(500).json({ error: 'Failed to fetch task' });
    }
  });

  /**
   * @swagger
   * /api/tasks:
   *   post:
   *     summary: Create a new task
   *     tags: [Tasks]
   *     requestBody:
   *       required: true
   *       content:
   *         application/json:
   *           schema:
   *             $ref: '#/components/schemas/CreateTask'
   *     responses:
   *       201:
   *         description: Task created
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Task'
   *       400:
   *         description: Validation error
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Error'
   */
  router.post('/', async (req: Request<{}, {}, CreateTaskDto>, res: Response) => {
    try {
      const { title, description } = req.body;
      if (!title || !description) {
        res.status(400).json({ error: 'Title and description are required' });
        return;
      }
      const task = await repository.create(req.body);
      res.status(201).json(task);
    } catch (error) {
      res.status(500).json({ error: 'Failed to create task' });
    }
  });

  /**
   * @swagger
   * /api/tasks/{id}:
   *   put:
   *     summary: Full update of a task
   *     description: All fields (title, description, status, priority) are required
   *     tags: [Tasks]
   *     parameters:
   *       - in: path
   *         name: id
   *         required: true
   *         schema:
   *           type: string
   *     requestBody:
   *       required: true
   *       content:
   *         application/json:
   *           schema:
   *             type: object
   *             required: [title, description, status, priority]
   *             properties:
   *               title:
   *                 type: string
   *               description:
   *                 type: string
   *               status:
   *                 type: string
   *                 enum: [todo, in-progress, done]
   *               priority:
   *                 type: string
   *                 enum: [low, medium, high]
   *               archived:
   *                 type: boolean
   *     responses:
   *       200:
   *         description: Task updated
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Task'
   *       400:
   *         description: Validation error
   *       404:
   *         description: Task not found
   */
  router.put('/:id', async (req: Request<{ id: string }, {}, UpdateTaskDto>, res: Response) => {
    try {
      const { title, description, status, priority } = req.body;
      if (!title || !description || !status || !priority) {
        res.status(400).json({ error: 'All fields (title, description, status, priority) are required for PUT' });
        return;
      }
      const task = await repository.update(req.params.id, req.body);
      if (!task) {
        res.status(404).json({ error: 'Task not found' });
        return;
      }
      res.json(task);
    } catch (error) {
      res.status(500).json({ error: 'Failed to update task' });
    }
  });

  /**
   * @swagger
   * /api/tasks/{id}:
   *   patch:
   *     summary: Partial update of a task
   *     description: Update any subset of fields. Use this to archive/unarchive tasks.
   *     tags: [Tasks]
   *     parameters:
   *       - in: path
   *         name: id
   *         required: true
   *         schema:
   *           type: string
   *     requestBody:
   *       required: true
   *       content:
   *         application/json:
   *           schema:
   *             $ref: '#/components/schemas/UpdateTask'
   *           examples:
   *             changeStatus:
   *               summary: Change status
   *               value: { "status": "in-progress" }
   *             archive:
   *               summary: Archive task
   *               value: { "archived": true }
   *             unarchive:
   *               summary: Unarchive task
   *               value: { "archived": false }
   *     responses:
   *       200:
   *         description: Task updated
   *         content:
   *           application/json:
   *             schema:
   *               $ref: '#/components/schemas/Task'
   *       404:
   *         description: Task not found
   */
  router.patch('/:id', async (req: Request<{ id: string }, {}, PatchTaskDto>, res: Response) => {
    try {
      const task = await repository.update(req.params.id, req.body);
      if (!task) {
        res.status(404).json({ error: 'Task not found' });
        return;
      }
      res.json(task);
    } catch (error) {
      res.status(500).json({ error: 'Failed to update task' });
    }
  });

  /**
   * @swagger
   * /api/tasks/{id}:
   *   delete:
   *     summary: Delete a task
   *     tags: [Tasks]
   *     parameters:
   *       - in: path
   *         name: id
   *         required: true
   *         schema:
   *           type: string
   *     responses:
   *       204:
   *         description: Task deleted
   *       404:
   *         description: Task not found
   */
  router.delete('/:id', async (req: Request<{ id: string }>, res: Response) => {
    try {
      const deleted = await repository.delete(req.params.id);
      if (!deleted) {
        res.status(404).json({ error: 'Task not found' });
        return;
      }
      res.status(204).send();
    } catch (error) {
      res.status(500).json({ error: 'Failed to delete task' });
    }
  });

  return router;
}
