import { type RouteConfig } from "@react-router/dev/routes";
import homeRoutes from "./features/home/routes";

const routes = [...homeRoutes] satisfies RouteConfig;

export default routes;
