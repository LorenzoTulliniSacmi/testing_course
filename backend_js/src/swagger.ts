import swaggerJsdoc from 'swagger-jsdoc';

const options: swaggerJsdoc.Options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'Kanban Board API',
      version: '1.0.0',
      description: 'REST API for the Kanban Board application',
    },
    servers: [
      {
        url: 'http://localhost:3000',
        description: 'Development server',
      },
    ],
    components: {
      schemas: {
        Task: {
          type: 'object',
          properties: {
            id: { type: 'string', format: 'uuid', example: '550e8400-e29b-41d4-a716-446655440000' },
            title: { type: 'string', example: 'Implement login feature' },
            description: { type: 'string', example: 'Add user authentication with JWT' },
            status: { type: 'string', enum: ['todo', 'in-progress', 'done'], example: 'todo' },
            priority: { type: 'string', enum: ['low', 'medium', 'high'], example: 'high' },
            archived: { type: 'boolean', example: false },
            createdAt: { type: 'string', format: 'date-time' },
            updatedAt: { type: 'string', format: 'date-time' },
          },
        },
        CreateTask: {
          type: 'object',
          required: ['title', 'description'],
          properties: {
            title: { type: 'string', example: 'New feature' },
            description: { type: 'string', example: 'Feature description' },
            priority: { type: 'string', enum: ['low', 'medium', 'high'], default: 'medium' },
          },
        },
        UpdateTask: {
          type: 'object',
          properties: {
            title: { type: 'string' },
            description: { type: 'string' },
            status: { type: 'string', enum: ['todo', 'in-progress', 'done'] },
            priority: { type: 'string', enum: ['low', 'medium', 'high'] },
            archived: { type: 'boolean' },
          },
        },
        Error: {
          type: 'object',
          properties: {
            error: { type: 'string' },
          },
        },
      },
    },
  },
  apis: ['./src/routes/*.ts'],
};

export const swaggerSpec = swaggerJsdoc(options);
