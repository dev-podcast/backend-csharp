import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';

// Async thunk for fetching podcasts from the API
export const fetchPodcasts = createAsyncThunk(
  'podcasts/fetchPodcasts',
  async () => {
    const response = await axios.get('/v1/podcasts');
    return response.data;
  }
);

// Async thunk for fetching episodes for a specific podcast
export const fetchEpisodes = createAsyncThunk(
  'podcasts/fetchEpisodes',
  async (podcastId) => {
    const response = await axios.get(`/v1/podcasts/${podcastId}/episodes`);
    return response.data;
  }
);

// Async thunk for searching podcasts and episodes
export const searchContent = createAsyncThunk(
  'podcasts/searchContent',
  async (searchTerm) => {
    const response = await axios.get(`/v1/search/${searchTerm}`);
    return response.data;
  }
);

const podcastSlice = createSlice({
  name: 'podcasts',
  initialState: {
    items: [],
    episodes: [],
    searchResults: {
      podcasts: [],
      episodes: []
    },
    loading: false,
    error: null,
  },
  reducers: {
    clearSearchResults: (state) => {
      state.searchResults = { podcasts: [], episodes: [] };
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPodcasts.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchPodcasts.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchPodcasts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(fetchEpisodes.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchEpisodes.fulfilled, (state, action) => {
        state.loading = false;
        state.episodes = action.payload;
      })
      .addCase(fetchEpisodes.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(searchContent.pending, (state) => {
        state.loading = true;
      })
      .addCase(searchContent.fulfilled, (state, action) => {
        state.loading = false;
        state.searchResults = action.payload;
      })
      .addCase(searchContent.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      });
  },
});

export const { clearSearchResults } = podcastSlice.actions;

export default podcastSlice.reducer;
