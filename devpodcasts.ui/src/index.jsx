import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { BrowserRouter as Router } from "react-router-dom";
import { configureStore } from "@reduxjs/toolkit";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";
import App from "./App";
import rootReducer from "./reducers";

import "./styles/main.scss";

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#FF9400', // Matches $orange from _colors.scss
    },
    secondary: {
      main: '#F1FF2D', // Matches $yellow-green
    },
    background: {
      default: '#29323c', // Matches $dark-gray
      paper: '#1e252d',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  },
});

const store = configureStore({
  reducer: rootReducer,
});

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <React.StrictMode>
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Router>
          <App />
        </Router>
      </ThemeProvider>
    </Provider>
  </React.StrictMode>
);
