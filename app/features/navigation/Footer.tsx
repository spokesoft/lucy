import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";
import { pink, amber } from "@mui/material/colors";

const Footer: FunctionComponent = () => {
  return (
    <Box
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderTop: (theme) => `2px solid ${amber[800]}`,
        color: "text.secondary",
        textAlign: "center",
        py: 3,
      }}
    >
      <Typography variant="body2">
        © 2025 Spokesoft. Made with{" "}
        <Box component="span" sx={{ color: pink[500] }}>
          &hearts;
        </Box>{" "}
        in tribute to the beloved pets who left pawprints on our souls.
      </Typography>
    </Box>
  );
};

export default Footer;
