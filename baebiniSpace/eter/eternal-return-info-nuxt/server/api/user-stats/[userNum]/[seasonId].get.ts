import { defineEventHandler, getQuery } from 'h3';
import axios from 'axios';

export default defineEventHandler(async (event) => {
  const { userNum, seasonId } = getQuery(event);
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return {
      error: 'API key is not configured'
    };
  }

  const API_BASE_URL = 'https://open-api.bser.io/v1';

  try {
    console.log(`Fetching user stats for userNum: ${userNum}, seasonId: ${seasonId}`);
    const response = await axios.get(`${API_BASE_URL}/user/stats/${userNum}/${seasonId}`,
      {
        headers: {
          'x-api-key': apiKey,
          'Accept': 'application/json',
        },
      }
    );

    console.log('API Response Status:', response.status);
    console.log('API Response Data:', response.data);

    if (response.data.code !== 200) {
      console.error('API returned an error:', response.data.message);
      return {
        error: response.data.message || 'Failed to fetch user stats'
      };
    }

    return response.data.userStats;

  } catch (error) {
    console.error('API request error:', error);
    return {
      error: 'An error occurred while fetching user stats.'
    };
  }
});
