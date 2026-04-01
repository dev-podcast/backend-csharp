import React, { useEffect, useState, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { Container, Typography, Box, CircularProgress, Button, Stack } from '@mui/material';
import ClearIcon from '@mui/icons-material/Clear';
import ShowDetails from '../../components/ShowDetails';
import EpisodeList from '../../components/EpisodeList';
import { fetchPodcasts, fetchEpisodes } from '../../reducers/podcastSlice';
import './style.scss';

const Show = () => {
  const { id } = useParams();
  const dispatch = useDispatch();
  const [activeTag, setActiveTag] = useState(null);

  const podcast = useSelector((state) => 
    state.podcasts.items.find(p => p.id === id)
  );
  const episodes = useSelector((state) => state.podcasts.episodes);
  const loading = useSelector((state) => state.podcasts.loading);

  useEffect(() => {
    dispatch(fetchPodcasts());
    dispatch(fetchEpisodes(id));
  }, [dispatch, id]);

  const filteredEpisodes = useMemo(() => {
    if (!activeTag) return episodes;
    return episodes.filter(e => e.tags && e.tags.includes(activeTag));
  }, [episodes, activeTag]);

  const handleEpisodeTagClick = (tag) => {
    setActiveTag(tag);
  };

  const clearFilter = () => {
    setActiveTag(null);
  };

  if (loading && !podcast) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress color="inherit" />
      </Box>
    );
  }

  if (!podcast) {
    return (
      <Box sx={{ textAlign: 'center', mt: 10 }}>
        <Typography variant="h4" color="error">Podcast not found</Typography>
      </Box>
    );
  }

  return (
    <Container className="show" maxWidth="lg">
      <Box sx={{ py: 4, textAlign: 'center' }}>
        <Typography variant="h3" component="h1" gutterBottom color="primary">
          {podcast.title}
        </Typography>
        <Typography variant="h5" component="h2" color="text.secondary" gutterBottom>
          {podcast.artists}
        </Typography>
        
        <ShowDetails podcast={podcast} />
        
        <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ mt: 6, mb: 2, px: { xs: 2, md: 10 } }}>
          <Typography variant="h4" component="h2">
            Episodes {activeTag && <Typography component="span" variant="h5" color="secondary"> - Filtered by: {activeTag}</Typography>}
          </Typography>
          {activeTag && (
            <Button 
              startIcon={<ClearIcon />} 
              onClick={clearFilter}
              color="secondary"
              variant="outlined"
              size="small"
            >
              Clear Filter
            </Button>
          )}
        </Stack>
        <EpisodeList episodes={filteredEpisodes} onTagClick={handleEpisodeTagClick} />
      </Box>
    </Container>
  );
};

export default Show;
