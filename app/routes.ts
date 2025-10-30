import { layout, type RouteConfig } from "@react-router/dev/routes";
import homeRoutes from "./features/home/routes";
import docsRoutes from "./features/docs/routes";

const routes = [
  layout("./features/navigation/main.tsx", [
    ...homeRoutes,
    ...docsRoutes,
  ]),
] satisfies RouteConfig;

export default routes;
