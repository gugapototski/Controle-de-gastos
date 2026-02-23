// src/pages/Relatorios.tsx
import { useState, useEffect } from 'react';
import { 
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, 
  Typography, Box, Grid, Card, CardContent, CircularProgress, Alert
} from '@mui/material';
import { relatoriosService } from '../api/services/relatoriosService';
import type { RelatorioTotais } from '../types';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export const Relatorios = () => {
  const [dados, setDados] = useState<RelatorioTotais | null>(null);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');

  const carregarRelatorio = async () => {
    try {
      setLoading(true);
      const resultado = await relatoriosService.getTotaisPorPessoa();
      setDados(resultado);
    } catch (error) {
      setErro('Erro ao carregar os totais do servidor.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    carregarRelatorio();
  }, []);

  const formatarMoeda = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (erro) {
    return <Alert severity="error">{erro}</Alert>;
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" fontWeight="bold" gutterBottom>
        Dashboard de Controle
      </Typography>

      {/* Cards de Resumo Geral */}
     {dados && (
        <Grid container spacing={3} sx={{ mb: 4, mt: 1 }}>
          <Grid size={{ xs: 12, md: 4 }}>
            <Card sx={{ bgcolor: '#e8f5e9', borderLeft: '5px solid #4caf50' }}>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>Total de Receitas</Typography>
                <Typography variant="h5" color="success.main" fontWeight="bold">
                  {formatarMoeda(dados.geral.totalReceitas)}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <Card sx={{ bgcolor: '#ffebee', borderLeft: '5px solid #f44336' }}>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>Total de Despesas</Typography>
                <Typography variant="h5" color="error.main" fontWeight="bold">
                  {formatarMoeda(dados.geral.totalDespesas)}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>
            <Card sx={{ bgcolor: '#e3f2fd', borderLeft: '5px solid #2196f3' }}>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>Saldo Líquido Geral</Typography>
                <Typography 
                  variant="h5" 
                  fontWeight="bold"
                  color={dados.geral.saldo >= 0 ? 'success.main' : 'error.main'}
                >
                  {formatarMoeda(dados.geral.saldo)}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      )}

      {/* Gráfico de Barras - Receitas x Despesas por Pessoa */}
      {dados && dados.itens.length > 0 && (
        <Box sx={{ mt: 4, mb: 5 }}>
          <Typography variant="h6" fontWeight="bold" sx={{ mb: 2 }}>
            Comparativo: Receitas vs Despesas
          </Typography>
          <Paper elevation={2} sx={{ p: 3, height: 350 }}>
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={dados.itens} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="nome" />
                <YAxis tickFormatter={(value) => `R$ ${value}`} />
                <Tooltip 
                formatter={(value: any) => formatarMoeda(Number(value || 0))}
                labelStyle={{ fontWeight: 'bold', color: '#333' }}
                />
                <Legend />
                <Bar dataKey="totalReceitas" name="Receitas" fill="#4caf50" radius={[4, 4, 0, 0]} />
                <Bar dataKey="totalDespesas" name="Despesas" fill="#f44336" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Box>
      )}

      {/* Tabela de Totais por Pessoa */}
      <Typography variant="h6" fontWeight="bold" sx={{ mb: 2 }}>
        Detalhamento por Pessoa
      </Typography>
      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead sx={{ backgroundColor: '#f0f0f0' }}>
            <TableRow>
              <TableCell sx={{ fontWeight: 'bold' }}>Nome da Pessoa</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Total Receitas</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Total Despesas</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Saldo Atual</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {dados?.itens.map((item) => (
              <TableRow key={item.pessoaId} hover>
                <TableCell>{item.nome}</TableCell>
                <TableCell sx={{ color: 'success.main', fontWeight: '500' }}>
                  {formatarMoeda(item.totalReceitas)}
                </TableCell>
                <TableCell sx={{ color: 'error.main', fontWeight: '500' }}>
                  {formatarMoeda(item.totalDespesas)}
                </TableCell>
                <TableCell sx={{ 
                  fontWeight: 'bold', 
                  color: item.saldo >= 0 ? 'success.main' : 'error.main' 
                }}>
                  {formatarMoeda(item.saldo)}
                </TableCell>
              </TableRow>
            ))}
            {dados?.itens.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} align="center" sx={{ py: 3 }}>
                  Nenhum dado financeiro encontrado.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};