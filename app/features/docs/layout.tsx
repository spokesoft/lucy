import type { FunctionComponent } from "react";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Typography from "@mui/material/Typography";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import { Link, Outlet } from "react-router";
import Grid from "@mui/material/Grid";

const commands = [
  { name: "New Command", href: "/docs/new" },
  { name: "List Command", href: "/docs/list" },
  { name: "Show Command", href: "/docs/show" },
  { name: "Update Command", href: "/docs/update" },
  { name: "Delete Command", href: "/docs/delete" },
];

const Sidebar: FunctionComponent = () => {
  return (
    <List>
      {commands.map((command, idx) => (
        <ListItem key={idx} disablePadding>
          <ListItemButton component={Link} to={command.href}>
            <ListItemText primary={command.name} />
          </ListItemButton>
        </ListItem>
      ))}
    </List>
  );
};

const Docs: FunctionComponent = () => {
  return (
    <Container>
      <Box sx={{ display: "flex", flexDirection: "row", mt: 4 }}>
        <Grid container>
          <Sidebar />
          <Grid>
            <Container>
              <Outlet />
            </Container>
          </Grid>
        </Grid>
      </Box>
    </Container>
  );
};

export default Docs;
