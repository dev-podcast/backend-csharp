import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import FontAwesome from 'react-fontawesome';
import EpisodeThumbnail from '../EpisodeThumbnail';
import './style.scss';

const EpisodeSlider = () => {
  const podcasts = useSelector((state) => state.podcasts.items);
  const [displayItems, setDisplayItems] = useState([]);

  // Mock static data for fallback or initial dev
  const mockEpisodes = [
    { id: '1', podcastId: '1', imageUrl: 'img/artwork/changelog.jpg', title: 'The Future of Open Source', latestReleaseDate: '2024-01-01' },
    { id: '2', podcastId: '2', imageUrl: 'img/artwork/developertea.jpg', title: '3 Lessons of Productivity', latestReleaseDate: '2024-01-02' },
    { id: '3', podcastId: '3', imageUrl: 'img/artwork/syntax.jpg', title: 'How to Slam Dunk Freelancing', latestReleaseDate: '2024-01-03' },
    { id: '4', podcastId: '4', imageUrl: 'img/artwork/bigwebshow.jpg', title: 'Creative Culture', latestReleaseDate: '2024-01-04' },
    { id: '5', podcastId: '5', imageUrl: 'img/artwork/takeupcode.jpg', title: 'Good Teachers From Fake', latestReleaseDate: '2024-01-05' },
  ];

  useEffect(() => {
    // If we have real podcasts, we'd ideally have recent episodes. 
    // For now, let's use podcasts as "episodes" for display if no real episodes provided
    if (podcasts && podcasts.length > 0) {
      setDisplayItems(podcasts.slice(0, 15));
    } else {
      setDisplayItems(mockEpisodes);
    }
  }, [podcasts]);

  const handleClickRight = () => {
    const items = [...displayItems];
    const first = items.shift();
    items.push(first);
    setDisplayItems(items);
  };

  const handleClickLeft = () => {
    const items = [...displayItems];
    const last = items.pop();
    items.unshift(last);
    setDisplayItems(items);
  };

  return (
    <div className="episode-slider">
      <FontAwesome className="left-nav" onClick={handleClickLeft} name='chevron-left' />
      <div className="thumbnails">
        {displayItems.map((item) => (
          <Link key={item.id} to={`/show/${item.id}`}>
            <EpisodeThumbnail
              img={item.imageUrl}
              title={item.title}
              airdate={item.latestReleaseDate}
            />
          </Link>
        ))}
      </div>
      <FontAwesome className="right-nav" onClick={handleClickRight} name='chevron-right' />
    </div>
  );
};

export default EpisodeSlider;
