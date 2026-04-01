import React from 'react';
import { Routes, Route } from 'react-router-dom';

import Nav from './components/Nav';
import Home from './views/Home';
import Show from './views/Show';
import SearchResults from './views/SearchResults';

const App = () => {
  return (
    <div>
      <Nav />
      <Routes>
        <Route path="/show/:id" element={<Show />} />
        <Route path="/search/:term" element={<SearchResults />} />
        <Route path="/" element={<Home />} />
      </Routes>
    </div>
  );
};

export default App;
