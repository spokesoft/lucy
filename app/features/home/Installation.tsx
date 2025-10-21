import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";
import type { FunctionComponent } from "react";

import GitHubIcon from "@mui/icons-material/GitHub";

const Installation: FunctionComponent = () => {
  return (
    <Box
      sx={{
        py: { xs: 4, md: 6 },
        textAlign: "center",
      }}
      id="install"
    >
      <Container>
        
        <Typography variant="h4" sx={{ mb: 2 }}>
          Get Started
        </Typography>

        <Typography variant="subtitle1" sx={{ mb: 2 }}>
          To install Lucy, you'll need the .NET SDK.
          <br />
          You can then install it as a global tool:
        </Typography>

        <Box>
          <Box
            component="pre"
            sx={{
              display: "inline-block",
              backgroundColor: "#23233a",
              color: "primary.main",
              p: 2,
              borderRadius: 1,
              mb: 2,
            }}
          >
            &gt; dotnet tool install --global spokesoft.lucy
          </Box>
        </Box>

        <Typography>
          <Link
            href="https://github.com/spokesoft/lucy"
            target="_blank"
            rel="noopener noreferrer"
            sx={{
              color: "primary.main",
              textDecoration: "none",
              display: "inline-flex",
              alignItems: "center",
              mt: 2,
              "&:hover": { color: "secondary.main" },
            }}
          >
            <GitHubIcon sx={{ mr: 1 }} /> View Lucy on GitHub →
          </Link>
        </Typography>

      </Container>
    </Box>
  );
};

export default Installation;
