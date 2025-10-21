import { layout, type RouteConfig } from "@react-router/dev/routes";
import homeRoutes from "./features/home/routes";

const routes = [
  layout("./features/navigation/main.tsx", [
    ...homeRoutes
  ]),
] satisfies RouteConfig;

export default routes;
