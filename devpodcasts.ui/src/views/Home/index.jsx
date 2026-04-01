import React, { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Typography, Container, Box } from '@mui/material';
import EpisodeSlider from '../../components/EpisodeSlider';
import { fetchPodcasts } from '../../reducers/podcastSlice';

import './style.scss';

const Home = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(fetchPodcasts());
  }, [dispatch]);

  return (
    <Container className="home" maxWidth="lg">
      <Box sx={{ mt: 15, mb: 4 }}>
        <Typography variant="h3" component="h1" gutterBottom color="primary" sx={{ fontWeight: 'bold' }}>
          NEW EPISODES
        </Typography>
        <EpisodeSlider />
      </Box>
    </Container>
  );
};

export default Home;
