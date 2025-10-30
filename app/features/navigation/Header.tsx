import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Link from "@mui/material/Link";
import Stack from "@mui/material/Stack";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import { Link as RouterLink } from "react-router";

import GithubIcon from "@mui/icons-material/GitHub";

const Header = () => {
  return (
    <Box component="header">
      {/* Navigation */}
      <AppBar
        position="static"
        elevation={0}
        sx={{
          backgroundColor: (theme) => theme.palette.background.default,
        }}
      >
        <Container>
          <Toolbar disableGutters sx={{ justifyContent: "space-between" }}>
            <Stack
              component={RouterLink}
              to="/"
              direction="row"
              alignItems="center"
              sx={{
                fontWeight: "bold",
                letterSpacing: 2,
                color: "primary.dark",
                textDecoration: "none",
                "&:hover": { color: "secondary.main" },
              }}
            >
              <Typography sx={{ fontSize: "1.25rem" }}>🐈‍⬛</Typography>
              <Typography
                sx={{ ml: 0.5, fontSize: "1.25rem", fontWeight: "bold" }}
              >
                Lucy
              </Typography>
            </Stack>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              <Typography
                component={RouterLink}
                to="#features"
                sx={{
                  mx: 1,
                  color: "primary.main",
                  textDecoration: "none",
                  "&:hover": { color: "secondary.main" },
                }}
              >
                Features
              </Typography>
              <Typography
                component={RouterLink}
                to="#install"
                sx={{
                  mx: 1,
                  color: "primary.main",
                  textDecoration: "none",
                  "&:hover": { color: "secondary.main" },
                }}
              >
                Install
              </Typography>
              <Typography
                component={RouterLink}
                to="/docs"
                sx={{
                  mx: 1,
                  color: "primary.main",
                  textDecoration: "none",
                  "&:hover": { color: "secondary.main" },
                }}
              >
                Docs
              </Typography>
              <Link
                href="https://github.com/spokesoft/lucy"
                target="_blank"
                rel="noopener noreferrer"
                sx={{
                  mx: 1,
                  color: "primary.main",
                  textDecoration: "none",
                  "&:hover": { color: "secondary.main" },
                }}
              >
                <GithubIcon />
              </Link>
            </Box>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
};

export default Header;
