// src/pages/Categorias.tsx
import { useState, useEffect } from 'react';
import { 
  Button, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, 
  Dialog, DialogTitle, DialogContent, DialogActions, TextField, Snackbar, Alert, Typography, Box,
  MenuItem, Select, FormControl, InputLabel
} from '@mui/material';
import { categoriasService } from '../api/services/categoriasService';
import type { Categoria } from '../types';
import { AppSnackbar } from '../components/AppSnackbar';

export const Categorias = () => {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [openForm, setOpenForm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  // Campos do formulário
  const [descricao, setDescricao] = useState('');
  const [finalidade, setFinalidade] = useState<number>(1); // 1 = Despesa por defeito
  
  // Feedback
  const [mensagem, setMensagem] = useState('');
  const [tipoAlerta, setTipoAlerta] = useState<'success' | 'error'>('success');
  const [openSnackbar, setOpenSnackbar] = useState(false);

  const carregarCategorias = async () => {
    try {
      const dados = await categoriasService.getAll();
      setCategorias(dados);
    } catch (error) {
      mostrarAlerta('Erro ao carregar categorias.', 'error');
    }
  };

  useEffect(() => {
    carregarCategorias();
  }, []);

  const mostrarAlerta = (msg: string, tipo: 'success' | 'error') => {
    setMensagem(msg);
    setTipoAlerta(tipo);
    setOpenSnackbar(true);
  };

  const handleSalvar = async () => {
    if (!descricao) {
      mostrarAlerta('Preencha a descrição.', 'error');
      return;
    }

    setIsSubmitting(true); // Bloqueia o botão

    try {
      await categoriasService.create({ descricao, finalidade });
      mostrarAlerta('Categoria registada com sucesso!', 'success');
      setOpenForm(false);
      setDescricao('');
      setFinalidade(1);
      carregarCategorias();
    } catch (error) {
      if (error instanceof Error) {
        mostrarAlerta(error.message, 'error');
      } else {
        mostrarAlerta('Erro desconhecido ao guardar categoria.', 'error');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const getFinalidadeLabel = (valor: number) => {
    switch (valor) {
      case 1: return 'Despesa';
      case 2: return 'Receita';
      case 3: return 'Ambas';
      default: return 'Desconhecida';
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Categorias
        </Typography>
        <Button variant="contained" color="primary" onClick={() => setOpenForm(true)}>
          + Nova Categoria
        </Button>
      </Box>

      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead sx={{ backgroundColor: '#f0f0f0' }}>
            <TableRow>
              <TableCell sx={{ fontWeight: 'bold' }}>Descrição</TableCell>
              <TableCell sx={{ fontWeight: 'bold' }}>Finalidade</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {categorias.map((categoria) => (
              <TableRow key={categoria.id} hover>
                <TableCell>{categoria.descricao}</TableCell>
                <TableCell>{getFinalidadeLabel(categoria.finalidade)}</TableCell>
              </TableRow>
            ))}
            {categorias.length === 0 && (
              <TableRow>
                <TableCell colSpan={2} align="center" sx={{ py: 3 }}>
                  Nenhuma categoria registada.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Modal de Registo */}
      <Dialog open={openForm} onClose={() => setOpenForm(false)} maxWidth="sm" fullWidth>
        <DialogTitle fontWeight="bold">Criar Categoria</DialogTitle>
        <DialogContent dividers>
          <TextField
            fullWidth
            label="Descrição (ex: Supermercado, Salário)"
            variant="outlined"
            margin="normal"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            inputProps={{ maxLength: 400 }}
          />
          <FormControl fullWidth margin="normal">
            <InputLabel id="finalidade-label">Finalidade</InputLabel>
            <Select
              labelId="finalidade-label"
              value={finalidade}
              label="Finalidade"
              onChange={(e) => setFinalidade(Number(e.target.value))}
            >
              <MenuItem value={1}>Despesa</MenuItem>
              <MenuItem value={2}>Receita</MenuItem>
              <MenuItem value={3}>Ambas</MenuItem>
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