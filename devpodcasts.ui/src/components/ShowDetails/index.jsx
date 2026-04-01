import React from 'react';
import { Paper, Box, Typography, Chip, Stack } from '@mui/material';
import { useNavigate } from 'react-router-dom';

const ShowDetails = ({ podcast }) => {
  const navigate = useNavigate();
  if (!podcast) return null;

  const handleTagClick = (tag) => {
    navigate(`/search/${encodeURIComponent(tag)}`);
  };

  return (
    <Paper elevation={0} sx={{ 
      display: 'flex', 
      flexDirection: { xs: 'column', md: 'row' }, 
      gap: 4, 
      p: 4, 
      backgroundColor: 'rgba(255, 255, 255, 0.05)',
      color: 'white',
      borderRadius: 2,
      mt: 4
    }}>
      <Box sx={{ 
        flex: '0 0 auto', 
        width: { xs: '100%', md: 300 },
        height: { xs: 'auto', md: 300 },
        borderRadius: 2,
        overflow: 'hidden',
        boxShadow: 4
      }}>
        <img 
          src={podcast.imageUrl} 
          alt={podcast.title} 
          style={{ width: '100%', height: '100%', objectFit: 'cover' }} 
        />
      </Box>
      <Box sx={{ flex: 1, textAlign: 'left' }}>
        <Typography variant="h5" gutterBottom color="primary.light" sx={{ fontWeight: 'bold' }}>
          About this Show
        </Typography>
        <Typography variant="body1" sx={{ lineHeight: 1.7, mb: 3 }}>
          {podcast.description || "No description available."}
        </Typography>

        {podcast.tags && podcast.tags.length > 0 && (
          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle2" color="text.secondary" gutterBottom>
              TAGS
            </Typography>
            <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
              {podcast.tags.map((tag) => (
                <Chip 
                  key={tag} 
                  label={tag} 
                  onClick={() => handleTagClick(tag)}
                  sx={{ cursor: 'pointer' }}
                  color="primary"
                  variant="outlined"
                />
              ))}
            </Stack>
          </Box>
        )}
      </Box>
    </Paper>
  );
};

export default ShowDetails;
