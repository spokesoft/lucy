import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

const ListCommand: FunctionComponent = () => {
  return (
    <Box>
      <Typography variant="h5">List Command</Typography>
      <Typography variant="body1">
        List all projects and tasks in the current workspace.
      </Typography>
    </Box>
  );
};

export default ListCommand;
