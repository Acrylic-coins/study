import { defineEventHandler } from 'h3';
import axios from 'axios';

export default defineEventHandler(async (event) => {
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return {
      error: 'API key is not configured'
    };
  }

  const API_BASE_URL = 'https://open-api.bser.io/v2/data';

  try {
    const response = await axios.get(`${API_BASE_URL}/Character`, {
      headers: {
        'x-api-key': apiKey,
        'Accept': 'application/json',
      },
    });

    if (response.data.code !== 200) {
      return {
        error: response.data.message || 'Failed to fetch character data'
      };
    }

    return response.data.data;
  } catch (error) {
    console.error('API request error:', error);
    return {
      error: 'An error occurred while fetching character data.'
    };
  }
});
