import apiClient from './apiClient'
import type {
  TodoItem,
  CreateTodoRequest,
  UpdateTodoRequest,
  AddProgressionRequest,
  ApiResponse
} from '@/types/todo'

export const todoService = {
  async getAll(): Promise<TodoItem[]> {
    const response = await apiClient.get<ApiResponse<TodoItem[]>>('/todolists')
    return response.data.data
  },

  async getById(id: number): Promise<TodoItem> {
    const response = await apiClient.get<ApiResponse<TodoItem>>(`/todolists/${id}`)
    return response.data.data
  },

  async create(data: CreateTodoRequest): Promise<TodoItem> {
    const response = await apiClient.post<ApiResponse<TodoItem>>('/todolists', data)
    return response.data.data
  },

  async update(id: number, data: UpdateTodoRequest): Promise<void> {
    await apiClient.put<ApiResponse<null>>(`/todolists/${id}`, data)
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete<ApiResponse<null>>(`/todolists/${id}`)
  },

  async addProgression(data: AddProgressionRequest): Promise<void> {
    await apiClient.post<ApiResponse<null>>(`/todolists/${data.id}/progressions`, {
      percent: data.percent,
      dateTime: data.dateTime
    })
  },

  async getCategories(): Promise<string[]> {
    const response = await apiClient.get<ApiResponse<string[]>>('/categories')
    return response.data.data
  }
}
