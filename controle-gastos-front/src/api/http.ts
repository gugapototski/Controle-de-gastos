import axios from 'axios';

export const http = axios.create({
  baseURL: "http://localhost:5001/api",
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para padronizar os erros que vêm do nosso GlobalExceptionMiddleware do backend
http.interceptors.response.use(
  (response) => response,
  (error) => {
    // Se a API retornar a mensagem de Regra de Negócio (Status 400)
    if (error.response && error.response.data && error.response.data.mensagem) {
      return Promise.reject(new Error(error.response.data.mensagem));
    }
    // Erro genérico (API fora do ar, erro 500, etc)
    return Promise.reject(new Error('Ocorreu um erro inesperado ao se comunicar com o servidor.'));
  }
);