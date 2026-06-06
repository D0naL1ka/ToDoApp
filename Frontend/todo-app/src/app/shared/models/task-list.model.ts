export interface TaskList {
  id: number;
  name: string;
  taskCount: number;
  createdAt: string;
}

export interface CreateTaskListDto {
  name: string;
}

export interface UpdateTaskListDto {
  name: string;
}