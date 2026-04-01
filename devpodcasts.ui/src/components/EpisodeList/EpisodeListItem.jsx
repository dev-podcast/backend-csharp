import React, { useState } from 'react';
import moment from 'moment';
import { 
  Card, 
  CardContent, 
  CardActions, 
  Typography, 
  IconButton, 
  Collapse,
  Box,
  Divider,
  Chip,
  Stack
} from '@mui/material';
import PlayCircleOutlineIcon from '@mui/icons-material/PlayCircleOutline';
import StopCircleIcon from '@mui/icons-material/StopCircle';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import { useNavigate } from 'react-router-dom';

const EpisodeListItem = (props) => {
  const [showPlayer, setShowPlayer] = useState(false);
  const navigate = useNavigate();

  const togglePlayer = () => {
    setShowPlayer(!showPlayer);
  };

  const handleTagClick = (tag) => {
    if (props.onTagClick) {
      props.onTagClick(tag);
    } else {
      navigate(`/search/${encodeURIComponent(tag)}`);
    }
  };

  const formattedDate = props.rel ? moment(props.rel).format('MMMM Do, YYYY') : 'Unknown Date';

  return (
    <Card sx={{ marginBottom: 3, boxShadow: 3, borderRadius: 2 }}>
      <Box sx={{ display: 'flex', flexDirection: 'column' }}>
        <CardContent sx={{ pb: 1 }}>
          <Typography component="div" variant="h6" color="primary" gutterBottom sx={{ fontWeight: 'bold' }}>
            {props.title}
          </Typography>
          <Typography variant="subtitle2" color="text.secondary" component="div">
            Released on {formattedDate}
          </Typography>
        </CardContent>

        <CardActions disableSpacing sx={{ px: 2, pb: 1 }}>
          <IconButton 
            aria-label={showPlayer ? "stop" : "play"} 
            onClick={togglePlayer}
            color={showPlayer ? "secondary" : "primary"}
          >
            {showPlayer ? <StopCircleIcon fontSize="large" /> : <PlayCircleOutlineIcon fontSize="large" />}
          </IconButton>
          
          <Typography variant="button" color="text.secondary" sx={{ ml: 1, cursor: 'pointer' }} onClick={togglePlayer}>
            {showPlayer ? "STOP LISTENING" : "LISTEN NOW"}
          </Typography>

          <Box sx={{ flexGrow: 1 }} />

          {props.link && (
            <IconButton 
              aria-label="open source" 
              href={props.link} 
              target="_blank" 
              rel="noopener noreferrer"
              size="small"
            >
              <OpenInNewIcon />
            </IconButton>
          )}
        </CardActions>

        <Collapse in={showPlayer} timeout="auto" unmountOnExit>
          <Box sx={{ px: 2, pb: 2 }}>
            <Divider sx={{ mb: 2 }} />
            {props.audioUrl ? (
              <audio controls autoPlay style={{ width: '100%' }}>
                <source src={props.audioUrl} type={props.audioType || 'audio/mpeg'} />
                Your browser does not support the audio element.
              </audio>
            ) : (
              <Typography variant="body2" color="error">
                Audio source not available.
              </Typography>
            )}
          </Box>
        </Collapse>

        <Divider />
        
        <CardContent>
          <Typography variant="body2" color="text.primary" sx={{ 
            display: '-webkit-box',
            WebkitLineClamp: 3,
            WebkitBoxOrient: 'vertical',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            mb: 2
          }}>
            {props.desc || "No description provided for this episode."}
          </Typography>

          {props.tags && props.tags.length > 0 && (
            <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
              {props.tags.map((tag) => (
                <Chip 
                  key={tag} 
                  label={tag} 
                  size="small" 
                  onClick={() => handleTagClick(tag)}
                  sx={{ mt: 1, cursor: 'pointer' }}
                  color="secondary"
                  variant="outlined"
                />
              ))}
            </Stack>
          )}
        </CardContent>
      </Box>
    </Card>
  );
};

export default EpisodeListItem;
