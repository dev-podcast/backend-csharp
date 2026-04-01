import React from 'react';
import { Box, Typography, Container } from '@mui/material';
import EpisodeListItem from './EpisodeListItem';

const EpisodeList = ({ episodes, onTagClick }) => {
  if (!episodes || episodes.length === 0) {
    return (
      <Box sx={{ mt: 4, textAlign: 'center' }}>
        <Typography variant="body1" color="text.secondary">
          No episodes found for this podcast.
        </Typography>
      </Box>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      {episodes.map(episode => (
        <EpisodeListItem
          key={episode.id}
          title={episode.title}
          desc={episode.description}
          rel={episode.publishedDate}
          link={episode.sourceUrl}
          audioUrl={episode.audioUrl}
          audioType={episode.audioType}
          tags={episode.tags}
          onTagClick={onTagClick}
        />
      ))}
    </Container>
  );
};

export default EpisodeList;
