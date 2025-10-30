import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

const ShowCommand: FunctionComponent = () => {
  return (
    <Box>
      <Typography variant="h5">Show Command</Typography>
      <Typography variant="body1">
        Show details of a specific project.
      </Typography>
    </Box>
  );
};

export default ShowCommand;