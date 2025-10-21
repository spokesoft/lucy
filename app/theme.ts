import { createTheme } from "@mui/material/styles";
import { blue, yellow, grey, blueGrey, indigo, amber } from "@mui/material/colors";

const theme = createTheme({
  palette: {
    mode: "dark",
    primary: blue,
    secondary: {
      main: amber[700],
      contrastText: grey[900],
      light: amber[600],
      dark: amber[800],
    },
    background: {
      paper: grey[900],
    },
    text: {
      primary: grey[200],
      secondary: grey[500],
    }
  },
  typography: {
    fontFamily: `'Fira Mono'`
  },
});

export default theme;
