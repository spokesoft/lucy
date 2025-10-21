import type { FunctionComponent } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import { useTheme } from "@mui/material/styles";
import useMediaQuery from "@mui/material/useMediaQuery";
import { Link as RouterLink } from "react-router";

const Hero: FunctionComponent = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  return (
    <Box
      sx={{
        background: "linear-gradient(90deg, #1b1e2b 60%, #23233a 100%)",
        color: "primary.main",
        px: { xs: 2, md: 0 },
        pt: { xs: 1, md: 9 },
        pb: { xs: 5, md: 9 },
        textAlign: "center",
      }}
    >
      <Box
        component="pre"
        sx={{
          fontFamily: "inherit",
          color: "secondary.main",
          fontSize: "2rem",
          opacity: 0.5,
          mb: 1,
        }}
      >
        {`/\\_/\\\n( =•ω•= )`}
      </Box>

      <Typography
        variant={isMobile ? "h4" : "h3"}
        component="h1"
        sx={{ mb: 1, color: "primary.main" }}
      >
        Project Management,
        <br />
        Without Leaving Your Terminal
      </Typography>

      <Typography variant="subtitle1" sx={{ color: "text.primary", mb: 4 }}>
        Fast, friendly, SQLite-powered task & project tracking.
        <br />
        No browser tabs. No accounts. Just you and your keyboard.
      </Typography>

      <Button
        variant="contained"
        size="large"
        component={RouterLink}
        to="#install"
        sx={{
          backgroundColor: "secondary.main",
          color: "secondary.contrastText",
          borderRadius: 24,
          fontWeight: "bold",
          fontSize: "1.2rem",
          letterSpacing: 1,
          boxShadow: "0 2px 12px rgba(0, 0, 0, 0.3)",
          "&:hover": {
            backgroundColor: "secondary.dark"
          },
        }}
      >
        Install Lucy
      </Button>
    </Box>
  );
};

export default Hero;
