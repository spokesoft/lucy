import { Outlet } from "react-router";
import Header from "./Header";
import Footer from "./Footer";
import Box from "@mui/material/Box";

const Navigation = () => {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <Header />
      <Box component="main" sx={{ flexGrow: 1 }}>
        <Outlet />
      </Box>
      <Footer />
    </Box>
  );
}

export default Navigation;
