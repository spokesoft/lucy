import type { Route } from "./+types/root";
import type { FunctionComponent } from "react";
import { Outlet } from "react-router";
import ErrorBoundary from "./components/ErrorBoundary";
import Layout from "./components/Layout";

const App: FunctionComponent = () => {
  return <Outlet />;
}

const HydrateFallback: FunctionComponent = () => {
  return <p>Loading...</p>;
};

export default App;
export { Layout, ErrorBoundary, HydrateFallback };
