// src/components/Layout.tsx
import { AppBar, Toolbar, Typography, Button, Box, Container } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';

export const Layout = ({ children }: { children: React.ReactNode }) => {
  const navigate = useNavigate();
  const location = useLocation();

  // Função simples para destacar o botão da página atual
  const getButtonVariant = (path: string) => 
    location.pathname === path ? 'contained' : 'text';

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      {/* Barra de Navegação Superior */}
      <AppBar position="static" color="primary" elevation={2}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1, fontWeight: 'bold' }}>
            Controle de Gastos
          </Typography>
          
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button 
              color="inherit" 
              variant={getButtonVariant('/relatorios')}
              onClick={() => navigate('/relatorios')}
              disableElevation
            >
              Dashboard
            </Button>
            <Button 
              color="inherit" 
              variant={getButtonVariant('/pessoas')}
              onClick={() => navigate('/pessoas')}
              disableElevation
            >
              Pessoas
            </Button>
            <Button 
              color="inherit" 
              variant={getButtonVariant('/categorias')}
              onClick={() => navigate('/categorias')}
              disableElevation
            >
              Categorias
            </Button>
            <Button 
              color="inherit" 
              variant={getButtonVariant('/transacoes')}
              onClick={() => navigate('/transacoes')}
              disableElevation
            >
              Transações
            </Button>
          </Box>
        </Toolbar>
      </AppBar>

      {/* Área onde o conteúdo das telas vai aparecer */}
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, flexGrow: 1 }}>
        {children}
      </Container>
    </Box>
  );
};