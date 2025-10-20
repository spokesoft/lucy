import type { FunctionComponent } from "react";

const HomePage: FunctionComponent = () => {
  return (
    <div>
      <title>Lucy</title>
      <h1>
        <span className="emoji">🐈‍⬛</span>Lucy
      </h1>
      <p>
        Lucy is a modern .NET application designed to provide project management
        from the command line. This project is built using .NET 8.0 and follows
        best practices for code quality and maintainability.
      </p>
    </div>
  );
};

export default HomePage;
