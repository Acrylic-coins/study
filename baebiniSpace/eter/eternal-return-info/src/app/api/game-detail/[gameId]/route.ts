import { NextResponse } from 'next/server';
import axios from 'axios';

export async function GET(
  request: Request,
  { params }: { params: { gameId: string } }
) {
  const { gameId } = params;
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return NextResponse.json({ error: 'API key is not configured' }, { status: 500 });
  }

  const API_BASE_URL = 'https://open-api.bser.io/v1';

  try {
    const response = await axios.get(`${API_BASE_URL}/games/${gameId}`, {
      headers: {
        'x-api-key': apiKey,
        'Accept': 'application/json',
      },
    });

    if (response.data.code !== 200) {
      return NextResponse.json(
        { error: response.data.message || 'Failed to fetch game detail' },
        { status: response.data.code || 500 }
      );
    }

    return NextResponse.json(response.data.userGames);
  } catch (error) {
    console.error('API request error:', error);
    return NextResponse.json(
      { error: 'An error occurred while fetching game detail.' },
      { status: 500 }
    );
  }
}
