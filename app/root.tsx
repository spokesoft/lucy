import type { Route } from "./+types/root";
import type { FunctionComponent } from "react";
import { Outlet } from "react-router";
import { ThemeProvider } from "@mui/material/styles";
import { CssBaseline } from "@mui/material";
import ErrorBoundary from "./components/ErrorBoundary";
import Layout from "./components/Layout";
import theme from "./theme";

import '@fontsource/fira-mono/400.css';
import '@fontsource/fira-mono/700.css';

const App: FunctionComponent = () => {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Outlet />
    </ThemeProvider>
  );
};

const HydrateFallback: FunctionComponent = () => {
  return <p>Loading...</p>;
};

export default App;
export { Layout, ErrorBoundary, HydrateFallback };
