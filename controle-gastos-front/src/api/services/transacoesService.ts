import { http } from '../http';
import type { Transacao, CreateTransacao } from '../../types';

export const transacoesService = {
  getAll: async (): Promise<Transacao[]> => {
    const response = await http.get<Transacao[]>('/transacoes');
    return response.data;
  },
  
  create: async (data: CreateTransacao): Promise<Transacao> => {
    const response = await http.post<Transacao>('/transacoes', data);
    return response.data;
  }
};