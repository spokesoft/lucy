import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

const UpdateCommand: FunctionComponent = () => {
  return (
    <Box>
      <Typography variant="h5">Update Command</Typography>
      <Typography variant="body1">
        Update the details of a project or its tasks.
      </Typography>
    </Box>
  );
};

export default UpdateCommand;