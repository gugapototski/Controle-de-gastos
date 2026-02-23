import { http } from '../http';
import type { Categoria, CreateCategoria } from '../../types';

export const categoriasService = {
  getAll: async (): Promise<Categoria[]> => {
    const response = await http.get<Categoria[]>('/categorias');
    return response.data;
  },
  
  create: async (data: CreateCategoria): Promise<Categoria> => {
    const response = await http.post<Categoria>('/categorias', data);
    return response.data;
  }
};