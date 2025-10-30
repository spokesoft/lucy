import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

const NewCommand: FunctionComponent = () => {
  return (
    <Box>
      <Typography variant="h5">New Command</Typography>
      <Typography variant="body1">
        Create a new project with the given name.
      </Typography>
    </Box>
  );
};

export default NewCommand;