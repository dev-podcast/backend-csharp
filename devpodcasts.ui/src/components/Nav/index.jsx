import React from 'react';
import './style.scss';
import Search from '../Search';
import { Link } from 'react-router-dom';

const Nav = () => {
  return (
    <div className="nav">
      <div className="logo">
        <Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>logo</Link>
      </div>
      <div className="nav-content">
        <ul>
          <li><Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>Browse</Link></li>
          <li>About</li>
          <li className="search"><Search /></li>
        </ul>
      </div>
    </div>
  );
};

export default Nav;
