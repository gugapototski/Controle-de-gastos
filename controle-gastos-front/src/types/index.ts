// src/types/index.ts

export interface Pessoa {
  id: string;
  nome: string;
  idade: number;
}

export interface Categoria {
  id: string;
  descricao: string;
  finalidade: number; // 1 = Despesa, 2 = Receita, 3 = Ambas
}

export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: number; // 1 = Despesa, 2 = Receita
  categoriaId: string;
  pessoaId: string;
}

// Interfaces para os formulários de criação (sem o ID)
export type CreatePessoa = Omit<Pessoa, 'id'>;
export type CreateCategoria = Omit<Categoria, 'id'>;
export type CreateTransacao = Omit<Transacao, 'id'>;

// Interfaces do Relatório
export interface RelatorioItem {
  pessoaId: string;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface RelatorioGeral {
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface RelatorioTotais {
  itens: RelatorioItem[];
  geral: RelatorioGeral;
}