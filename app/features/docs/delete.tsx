import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

const DeleteCommand: FunctionComponent = () => {
  return (
    <Box>
      <Typography variant="h5">Delete Command</Typography>
      <Typography variant="body1">
        Delete a project and all its associated tasks.
      </Typography>
    </Box>
  );
};

export default DeleteCommand;
