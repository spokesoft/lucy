import type { FunctionComponent } from "react";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";

import TerminalIcon from "@mui/icons-material/Terminal";
import StorageIcon from "@mui/icons-material/Storage";
import BoltIcon from "@mui/icons-material/Bolt";

interface Feature {
  icon: React.ReactNode;
  title: string;
  content: React.ReactNode;
}

const features: Feature[] = [
  {
    icon: <TerminalIcon sx={{ fontSize: 50, color: "secondary.main" }} />,
    title: "Native CLI",
    content: "Organize projects, tasks, and notes in seconds using simple commands. Zero mouse required.",
  },
  {
    icon: <StorageIcon sx={{ fontSize: 50, color: "secondary.main" }} />,
    title: "SQLite Simplicity",
    content: (
      <>
        Your project data lives in a fast, portable <strong>SQLite</strong> file. No setup, no cloud, no nonsense.
      </>
    ),
  },
  {
    icon: <BoltIcon sx={{ fontSize: 50, color: "secondary.main" }} />,
    title: "Lightning Fast",
    content: "Instant startup. Blazing command response. Built in .NET 8 for performance and joy.",
  },
];

const Features: FunctionComponent = () => {
  return (
    <Box
      sx={{ backgroundColor: (theme) => theme.palette.background.paper, py: { xs: 4, md: 6 } }}
      id="features"
    >
      <Container>
        <Grid container spacing={2}>
          {features.map((feature, idx) => (
            <Grid size={{ xs: 12, md: 4 }} key={idx}>
              <Box textAlign="center">
                {feature.icon}
                <Typography
                  variant="h6"
                  sx={{
                    fontWeight: "bold",
                    color: "primary.main",
                  }}
                >
                  {feature.title}
                </Typography>
                <Typography variant="body1" sx={{ color: "text.primary" }}>
                  {feature.content}
                </Typography>
              </Box>
            </Grid>
          ))}
        </Grid>
      </Container>
    </Box>
  );
};

export default Features;
