export interface Task {
    id: number;
    taskName: string;
    description?: string;
    isActive: boolean;
    createdByName: string;
    createdAt: Date;
    currentAssignedTo?: string;
    currentStatus: string;
    canTrigger: boolean;
  }
  
  export interface CreateTaskRequest {
    taskName: string;
    description?: string;
  }
  
  export interface TriggerTaskRequest {
    taskId: number;
    triggeredBy: number;
  }
  