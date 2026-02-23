// src/pages/Pessoas.tsx
import { useState, useEffect } from "react";
import {
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Snackbar,
  Alert,
  Typography,
  Box,
  IconButton,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { pessoasService } from "../api/services/pessoasService";
import type { Pessoa } from "../types";
import { AppSnackbar } from "../components/AppSnackbar";
import { ConfirmDialog } from "../components/ConfirmDialog";

export const Pessoas = () => {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [openForm, setOpenForm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);// Bloqueia o botão

  // Modo edição
  const [idEmEdicao, setIdEmEdicao] = useState<string | null>(null);

  // Campos do formulário
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState<number | "">("");

  // Feedback
  const [mensagem, setMensagem] = useState("");
  const [tipoAlerta, setTipoAlerta] = useState<"success" | "error">("success");
  const [openSnackbar, setOpenSnackbar] = useState(false);

const [openConfirm, setOpenConfirm] = useState(false);
const [idParaExcluir, setIdParaExcluir] = useState<string | null>(null);

  const mostrarAlerta = (msg: string, tipo: "success" | "error") => {
    setMensagem(msg);
    setTipoAlerta(tipo);
    setOpenSnackbar(true);
  };

  const carregarPessoas = async () => {
    try {
      const dados = await pessoasService.getAll();
      setPessoas(dados);
    } catch {
      mostrarAlerta("Erro ao carregar pessoas.", "error");
    }
  };

  useEffect(() => {
    carregarPessoas();
  }, []);

  const resetForm = () => {
    setIdEmEdicao(null);
    setNome("");
    setIdade("");
  };

  const handleAbrirCadastro = () => {
    resetForm();
    setOpenForm(true);
  };

  const handleAbrirEdicao = (pessoa: Pessoa) => {
    setIdEmEdicao(pessoa.id);
    setNome(pessoa.nome);
    setIdade(pessoa.idade);
    setOpenForm(true);
  };

  const handleFecharModal = () => {
    setOpenForm(false);
    resetForm();
  };

  const handleSalvar = async () => {
    if (!nome || idade === "" || idade === null) {
      mostrarAlerta("Preencha todos os campos.", "error");
      return;
    }

    setIsSubmitting(true);

    try {
      const payload = { nome, idade: Number(idade) };

      if (idEmEdicao) {
        await pessoasService.update(idEmEdicao, payload);
        mostrarAlerta("Pessoa atualizada com sucesso!", "success");
      } else {
        await pessoasService.create(payload);
        mostrarAlerta("Pessoa cadastrada com sucesso!", "success");
      }

      handleFecharModal();
      carregarPessoas();
    } catch (error) {
      if (error instanceof Error) {
        mostrarAlerta(error.message, "error");
      } else {
        mostrarAlerta("Erro desconhecido ao salvar pessoa.", "error");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const solicitarExclusao = (id: string) => {
  setIdParaExcluir(id);
  setOpenConfirm(true);
};

const confirmarExclusao = async () => {
  if (!idParaExcluir) return;

  try {
    await pessoasService.delete(idParaExcluir);
    mostrarAlerta("Pessoa excluída com sucesso!", "success");
    carregarPessoas();
  } catch (error: any) {
    mostrarAlerta(error.message || "Erro ao excluir pessoa.", "error");
  } finally {
    setOpenConfirm(false);
    setIdParaExcluir(null);
  }
};

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", mb: 3 }}>
        <Typography variant="h4" component="h1" fontWeight="bold">
          Pessoas
        </Typography>
        <Button variant="contained" color="primary" onClick={handleAbrirCadastro}>
          + Nova Pessoa
        </Button>
      </Box>

      <TableContainer component={Paper} elevation={2}>
        <Table>
          <TableHead sx={{ backgroundColor: "#f0f0f0" }}>
            <TableRow>
              <TableCell sx={{ fontWeight: "bold" }}>Nome</TableCell>
              <TableCell sx={{ fontWeight: "bold" }}>Idade</TableCell>
              <TableCell sx={{ fontWeight: "bold", width: "140px" }}>
                Ações
              </TableCell>
            </TableRow>
          </TableHead>

          <TableBody>
            {pessoas.map((pessoa) => (
              <TableRow key={pessoa.id} hover>
                <TableCell>{pessoa.nome}</TableCell>
                <TableCell>{pessoa.idade} anos</TableCell>
                <TableCell>
                  <IconButton
                    color="primary"
                    onClick={() => handleAbrirEdicao(pessoa)}
                    size="small"
                    title="Editar"
                  >
                    <EditIcon />
                  </IconButton>

                  <IconButton
                    color="error"
                    onClick={() => solicitarExclusao(pessoa.id)}
                    size="small"
                    title="Excluir"
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}

            {pessoas.length === 0 && (
              <TableRow>
                <TableCell colSpan={3} align="center" sx={{ py: 3 }}>
                  Nenhuma pessoa cadastrada.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={openForm} onClose={handleFecharModal} maxWidth="sm" fullWidth>
        <DialogTitle fontWeight="bold">
          {idEmEdicao ? "Editar Pessoa" : "Cadastrar Pessoa"}
        </DialogTitle>

        <DialogContent dividers>
          <TextField
            fullWidth
            label="Nome Completo"
            variant="outlined"
            margin="normal"
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            slotProps={{ htmlInput: { maxLength: 200 } }}
          />

          <TextField
            fullWidth
            label="Idade"
            type="number"
            variant="outlined"
            margin="normal"
            value={idade}
            onChange={(e) =>
              setIdade(e.target.value === "" ? "" : Number(e.target.value))
            }
          />
        </DialogContent>

        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={handleFecharModal} color="inherit">
            Cancelar
          </Button>
          <Button onClick={handleSalvar} variant="contained" disableElevation disabled={isSubmitting}>
            {isSubmitting ? 'Salvando...' : idEmEdicao ? "Salvar Alterações" : "Salvar"}
          </Button>
        </DialogActions>
      </Dialog>

      <AppSnackbar open={openSnackbar} message={mensagem} severity={tipoAlerta} onClose={() => setOpenSnackbar(false)} />

        <ConfirmDialog
        open={openConfirm}
        title="Excluir pessoa?"
        message="Tem certeza que deseja excluir? Todas as transações desta pessoa serão apagadas."
        confirmText="Excluir"
        cancelText="Cancelar"
        onCancel={() => {
          setOpenConfirm(false);
          setIdParaExcluir(null);
        }}
        onConfirm={confirmarExclusao}/>
    </Box>
  );
};