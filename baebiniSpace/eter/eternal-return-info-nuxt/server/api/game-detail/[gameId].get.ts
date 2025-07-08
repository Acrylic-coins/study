import { defineEventHandler } from 'h3';
import axios from 'axios';

export default defineEventHandler(async (event) => {
  const gameId = event.context.params.gameId;
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return {
      error: 'API key is not configured'
    };
  }

  const API_BASE_URL = 'https://open-api.bser.io/v1';

  try {
    const response = await axios.get(`${API_BASE_URL}/games/${gameId}`, {
      headers: {
        'x-api-key': apiKey,
        'Accept': 'application/json',
      },
    });

    console.log('API Response Data:', response.data); // Add this line to log the data

    if (response.data.code !== 200) {
      return {
        error: response.data.message || 'Failed to fetch game detail'
      };
    }

    return response.data.userGames;
  } catch (error) {
    console.error('API request error:', error);
    return {
      error: 'An error occurred while fetching game detail.'
    };
  }
});
