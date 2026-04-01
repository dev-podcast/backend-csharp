import React from 'react';
import './style.scss';

const EpisodeThumbnail = ({ img, title, airdate }) => {
  return (
    <div className="episode-thumbnail">
      <div className="artwork">
        <img src={img} alt={title} />
      </div>
      <div className="episode-details">
        <div className="episode-title">
          <p>{title}</p>
        </div>
        <div className="episode-airdate">
          <p>{airdate}</p>
        </div>
      </div>
    </div>
  );
};

export default EpisodeThumbnail;
