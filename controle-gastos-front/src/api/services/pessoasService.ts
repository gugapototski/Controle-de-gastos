import { http } from '../http';
import type { Pessoa, CreatePessoa } from '../../types';

export const pessoasService = {
  getAll: async (): Promise<Pessoa[]> => 
  {
    const response = await http.get<Pessoa[]>('/pessoas');
    return response.data;
  },
  
  create: async (data: CreatePessoa): Promise<Pessoa> => 
  {
    const response = await http.post<Pessoa>('/pessoas', data);
    return response.data;
  },

  delete: async (id: string): Promise<void> =>
  {
    await http.delete(`/pessoas/${id}`);
  },

  update: async (id: string, data: CreatePessoa): Promise<Pessoa> => 
  {
  const response = await http.put<Pessoa>(`/pessoas/${id}`, data);
  return response.data;
  }
};