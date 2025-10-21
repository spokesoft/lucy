import type { FunctionComponent } from "react";
import Hero from "./Hero";
import Installation from "./Installation";
import Features from "./Features";

const HomePage: FunctionComponent = () => {
  return (
    <div>
      <Hero />
      <Features />
      <Installation />
    </div>
  );
};

export default HomePage;
