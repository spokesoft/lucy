import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

const routes = [
  route("docs", "./features/docs/layout.tsx", [
    index("./features/docs/docs.tsx"),
    route("new", "./features/docs/new.tsx"),
    route("list", "./features/docs/list.tsx"),
    route("show", "./features/docs/show.tsx"),
    route("update", "./features/docs/update.tsx"),
    route("delete", "./features/docs/delete.tsx")
  ]),
] satisfies RouteConfig;

export default routes;
