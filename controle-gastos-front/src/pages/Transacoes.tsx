import { useState, useEffect } from 'react';
import { 
  Button, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, 
  Dialog, DialogTitle, DialogContent, DialogActions, TextField, Typography, Box,
  MenuItem, Select, FormControl, InputLabel
} from '@mui/material';
import { transacoesService } from '../api/services/transacoesService';
import { pessoasService } from '../api/services/pessoasService';
import { categoriasService } from '../api/services/categoriasService';
import type { Transacao, Pessoa, Categoria } from '../types';
import { AppSnackbar } from '../components/AppSnackbar';

export const Transacoes = () => {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [openForm, setOpenForm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  // Campos do formulário
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState<number | ''>('');
  const [tipo, setTipo] = useState<number>(1); // 1 = Despesa, 2 = Receita
  const [categoriaId, setCategoriaId] = useState('');
  const [pessoaId, setPessoaId] = useState('');
  
  // Feedback
  const [mensagem, setMensagem] = useState('');
  const [tipoAlerta, setTipoAlerta] = useState<'success' | 'error'>('success');
  const [openSnackbar, setOpenSnackbar] = useState(false);

  const carregarDados = async () => {
    try {
      const [transacoesData, pessoasData, categoriasData] = await Promise.all([
        transacoesService.getAll(),
        pessoasService.getAll(),
        categoriasService.getAll()
      ]);
      setTransacoes(transacoesData);
      setPessoas(pessoasData);
      setCategorias(categoriasData);
    } catch (error) {
      mostrarAlerta('Erro ao carregar os dados.', 'error');
    }
  };

  useEffect(() => {
    carregarDados();
  }, []);

  const mostrarAlerta = (msg: string, tipo: 'success' | 'error') => {
    setMensagem(msg);
    setTipoAlerta(tipo);
    setOpenSnackbar(true);
  };

  const handleSalvar = async () => {
    if (!descricao || !valor || !categoriaId || !pessoaId) {
      mostrarAlerta('Preencha todos os campos.', 'error');
      return;
    }

    setIsSubmitting(true); // Bloqueia o botão

    try {
      await transacoesService.create({ 
        descricao, 
        valor: Number(valor), 
        tipo, 
        categoriaId, 
        pessoaId 
      });
      mostrarAlerta('Transação registada com sucesso!', 'success');
      setOpenForm(false);
      
      // Limpar form
      setDescricao('');
      setValor('');
      setTipo(1);
      setCategoriaId('');
      setPessoaId('');
      
      carregarDados(); // Atualiza a tabela
    } catch (error) {
      // Verifica se o erro é uma instância da classe Error (que o seu interceptor gera)
      if (error instanceof Error) {
        mostrarAlerta(error.message, 'error');
      } else {
        mostrarAlerta('Erro desconhecido ao guardar transação.', 'error');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const getNomePessoa = (id: string) => pessoas.find(p => p.id === id)?.nome || 'Desconhecida';
  const getNomeCategoria = (id: string) => categorias.find(c => c.id === id)?.descricao || 'Desconhecida';

  // Filtra as categorias para o dropdown não mostrar "Receita" se for uma "Despesa"
  const categoriasFiltradas = categorias.filter(c => {
    if (tipo === 1) return c.finalidade === 1 || c.finalidade === 3; // 1=Despesa, 3=Ambas
    if (tipo === 2) return c.finalidade === 2 || c.finalidade === 3; // 2=Receita, 3=Ambas
    return true;
  });

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Transações
        </Typography>
        <Button variant="contained" color="primary" onClick={() => setOpenForm(true)}>
          + Nova Transação
        </Button>
      </Box>

      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead sx={{ backgroundColor: '#f0f0f0' }}>
            <TableRow>
              <TableCell sx={{ fontWeight: 'bold' }}>Descrição</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Pessoa</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Categoria</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Tipo</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Valor</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {transacoes.map((t) => (
              <TableRow key={t.id} hover>
                <TableCell>{t.descricao}</TableCell>
                <TableCell>{getNomePessoa(t.pessoaId)}</TableCell>
                <TableCell>{getNomeCategoria(t.categoriaId)}</TableCell>
                <TableCell sx={{ color: t.tipo === 1 ? 'error.main' : 'success.main', fontWeight: 'bold' }}>
                  {t.tipo === 1 ? 'Despesa' : 'Receita'}
                </TableCell>
                <TableCell>
                  {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(t.valor)}
                </TableCell>
              </TableRow>
            ))}
            {transacoes.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} align="center" sx={{ py: 3 }}>
                  Nenhuma transação registada.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={openForm} onClose={() => setOpenForm(false)} maxWidth="sm" fullWidth>
        <DialogTitle fontWeight="bold">Lançar Transação</DialogTitle>
        <DialogContent dividers>
          <FormControl fullWidth margin="normal">
            <InputLabel id="pessoa-label">Pessoa</InputLabel>
            <Select
              labelId="pessoa-label"
              value={pessoaId}
              label="Pessoa"
              onChange={(e) => setPessoaId(e.target.value)}
            >
              {pessoas.map(p => (
                <MenuItem key={p.id} value={p.id}>{p.nome} ({p.idade} anos)</MenuItem>
              ))}
            </Select>
          </FormControl>

          <TextField
            fullWidth
            label="Descrição (ex: Mensalidade da academia, Compra de moto para revenda)"
            variant="outlined"
            margin="normal"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            slotProps={{ htmlInput: { maxLength: 400 } }}
          />

          <Box sx={{ display: 'flex', gap: 2, mt: 2, mb: 1 }}>
            <FormControl fullWidth>
              <InputLabel id="tipo-label">Tipo</InputLabel>
              <Select
                labelId="tipo-label"
                value={tipo}
                label="Tipo"
                onChange={(e) => {
                setTipo(Number(e.target.value));
                setCategoriaId('');
              }}
              >
                <MenuItem value={1}>Despesa</MenuItem>
                <MenuItem value={2}>Receita</MenuItem>
              </Select>
            </FormControl>

            <TextField
              fullWidth
              label="Valor (R$)"
              type="number"
              variant="outlined"
              value={valor}
              onChange={(e) => setValor(e.target.value === '' ? '' : Number(e.target.value))}
            />
          </Box>

          <FormControl fullWidth margin="normal">
            <InputLabel id="categoria-label">Categoria</InputLabel>
            <Select
              labelId="categoria-label"
              value={categoriaId}
              label="Categoria"
              onChange={(e) => setCategoriaId(e.target.value)}
            >
              {categoriasFiltradas.map(c => (
    <MenuItem key={c.id} value={c.id}>{c.descricao}</MenuItem>
  ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={() => setOpenForm(false)} color="inherit">Cancelar</Button>
          <Button onClick={handleSalvar} variant="contained" disableElevation disabled={isSubmitting}>{isSubmitting ? 'Guardando...' : 'Guardar'}</Button>
        </DialogActions>
      </Dialog>

     <AppSnackbar open={openSnackbar} message={mensagem} severity={tipoAlerta} onClose={() => setOpenSnackbar(false)} />
    </Box>
  );
};