import { NextResponse } from 'next/server';
import axios from 'axios';

export async function GET() {
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return NextResponse.json({ error: 'API key is not configured' }, { status: 500 });
  }

  const API_BASE_URL = 'https://open-api.bser.io/v2/data'; // v2 API 사용

  try {
    const response = await axios.get(`${API_BASE_URL}/Character`, {
      headers: {
        'x-api-key': apiKey,
        'Accept': 'application/json',
      },
    });

    if (response.data.code !== 200) {
      return NextResponse.json(
        { error: response.data.message || 'Failed to fetch character data' },
        { status: response.data.code || 500 }
      );
    }

    return NextResponse.json(response.data.data);
  } catch (error) {
    console.error('API request error:', error);
    return NextResponse.json(
      { error: 'An error occurred while fetching character data.' },
      { status: 500 }
    );
  }
}
