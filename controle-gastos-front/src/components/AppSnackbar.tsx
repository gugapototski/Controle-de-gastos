// src/components/AppSnackbar.tsx
import { Snackbar, Alert } from "@mui/material";

type Props = {
  open: boolean;
  message: string;
  severity: "success" | "error";
  onClose: () => void;
};

export function AppSnackbar({ open, message, severity, onClose }: Props) {
  return (
    <Snackbar
      open={open}
      autoHideDuration={4000}
      onClose={onClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
    >
      <Alert
        onClose={onClose}
        severity={severity}
        variant="filled"
        sx={{ width: "100%" }}
      >
        {message}
      </Alert>
    </Snackbar>
  );
}