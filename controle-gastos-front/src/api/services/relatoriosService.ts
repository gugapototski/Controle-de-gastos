import { http } from '../http';
import type { RelatorioTotais } from '../../types';

export const relatoriosService = {
  getTotaisPorPessoa: async (): Promise<RelatorioTotais> => {
    const response = await http.get<RelatorioTotais>('/relatorios/totais-por-pessoa');
    return response.data;
  }
};