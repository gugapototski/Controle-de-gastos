// src/App.tsx
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme, CssBaseline } from '@mui/material';
import { Layout } from './components/Layout';
import { Pessoas } from './pages/Pessoas';
import { Categorias } from './pages/Categorias';
import { Transacoes } from './pages/Transacoes';
import { Relatorios } from './pages/Relatorios';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2', // Azul corporativo clássico
    },
    background: {
      default: '#f5f5f5',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Layout>
          <Routes>
            {/* O conteúdo destas rotas vai ser injetado dentro do {children} do Layout */}
            <Route path="/pessoas" element={<Pessoas />} />
            <Route path="/categorias" element={<Categorias />} />
            <Route path="/transacoes" element={<Transacoes />} />
            <Route path="/relatorios" element={<Relatorios />} />
            
            <Route path="*" element={<Navigate to="/relatorios" replace />} />
          </Routes>
        </Layout>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;