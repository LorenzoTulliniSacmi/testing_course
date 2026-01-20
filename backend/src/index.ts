import express from 'express';
import cors from 'cors';
import swaggerUi from 'swagger-ui-express';
import { createTaskRouter } from './routes/tasks.js';
import { JsonTaskRepository } from './repositories/json.repository.js';
import { MongoTaskRepository } from './repositories/mongo.repository.js';
import { TaskRepository } from './repositories/task.repository.js';
import { swaggerSpec } from './swagger.js';

const app = express();
const PORT = process.env.PORT || 3000;
const STORAGE_TYPE = process.env.STORAGE_TYPE || 'json'; // 'json' or 'mongo'
const MONGO_URI = process.env.MONGO_URI || 'mongodb://localhost:27017/kanban';

// Middleware
app.use(cors());
app.use(express.json());

// Swagger UI
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec));
app.get('/api-docs.json', (_, res) => res.json(swaggerSpec));

async function startServer() {
  let repository: TaskRepository;

  if (STORAGE_TYPE === 'mongo') {
    const mongoRepo = new MongoTaskRepository();
    await mongoRepo.connect(MONGO_URI);
    repository = mongoRepo;
    console.log(`Using MongoDB storage: ${MONGO_URI}`);
  } else {
    repository = new JsonTaskRepository();
    console.log('Using JSON file storage');
  }

  // Routes
  app.use('/api/tasks', createTaskRouter(repository));

  // Health check
  app.get('/health', (_, res) => {
    res.json({ status: 'ok', storage: STORAGE_TYPE, timestamp: new Date().toISOString() });
  });

  // Start server
  app.listen(PORT, () => {
    console.log(`Server running on http://localhost:${PORT}`);
    console.log(`API available at http://localhost:${PORT}/api/tasks`);
    console.log(`Swagger UI at http://localhost:${PORT}/api-docs`);
  });
}

startServer().catch((error) => {
  console.error('Failed to start server:', error);
  process.exit(1);
});
