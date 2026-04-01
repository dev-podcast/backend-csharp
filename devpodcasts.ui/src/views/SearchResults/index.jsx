import React, { useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { 
  Container, 
  Typography, 
  Box, 
  CircularProgress, 
  Grid, 
  Card, 
  CardContent, 
  CardMedia,
  Divider
} from '@mui/material';
import { searchContent } from '../../reducers/podcastSlice';
import EpisodeListItem from '../../components/EpisodeList/EpisodeListItem';

const SearchResults = () => {
  const { term } = useParams();
  const dispatch = useDispatch();
  const { searchResults, loading, error } = useSelector((state) => state.podcasts);

  useEffect(() => {
    if (term) {
      dispatch(searchContent(term));
    }
  }, [dispatch, term]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress color="primary" />
      </Box>
    );
  }

  const hasResults = (searchResults.podcasts && searchResults.podcasts.length > 0) || 
                     (searchResults.episodes && searchResults.episodes.length > 0);

  return (
    <Container maxWidth="lg" sx={{ mt: 12, mb: 4 }}>
      <Typography variant="h4" gutterBottom color="primary">
        Search Results for "{decodeURIComponent(term)}"
      </Typography>

      {!hasResults && !loading && (
        <Typography variant="h6" sx={{ mt: 4 }}>
          No results found.
        </Typography>
      )}

      {searchResults.podcasts && searchResults.podcasts.length > 0 && (
        <Box sx={{ mt: 4 }}>
          <Typography variant="h5" gutterBottom sx={{ borderBottom: '2px solid', borderColor: 'primary.main', pb: 1, width: 'fit-content' }}>
            Podcasts
          </Typography>
          <Grid container spacing={3} sx={{ mt: 1 }}>
            {searchResults.podcasts.map((podcast) => (
              <Grid item xs={12} sm={6} md={4} key={podcast.id}>
                <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column', transition: 'transform 0.2s', '&:hover': { transform: 'scale(1.02)' } }} component={Link} to={`/show/${podcast.id}`} style={{ textDecoration: 'none' }}>
                  <CardMedia
                    component="img"
                    height="200"
                    image={podcast.imageUrl}
                    alt={podcast.title}
                  />
                  <CardContent>
                    <Typography gutterBottom variant="h6" component="div" color="primary" sx={{ fontWeight: 'bold' }}>
                      {podcast.title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ 
                      display: '-webkit-box',
                      WebkitLineClamp: 2,
                      WebkitBoxOrient: 'vertical',
                      overflow: 'hidden',
                      textOverflow: 'ellipsis'
                    }}>
                      {podcast.description}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Box>
      )}

      {searchResults.episodes && searchResults.episodes.length > 0 && (
        <Box sx={{ mt: 6 }}>
          <Typography variant="h5" gutterBottom sx={{ borderBottom: '2px solid', borderColor: 'secondary.main', pb: 1, width: 'fit-content' }}>
            Episodes
          </Typography>
          <Box sx={{ mt: 2 }}>
            {searchResults.episodes.map((episode) => (
              <EpisodeListItem
                key={episode.id}
                title={episode.title}
                desc={episode.description}
                rel={episode.publishedDate}
                link={episode.sourceUrl}
                audioUrl={episode.audioUrl}
                audioType={episode.audioType}
                tags={episode.tags}
              />
            ))}
          </Box>
        </Box>
      )}
    </Container>
  );
};

export default SearchResults;
