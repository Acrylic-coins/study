import { NextResponse } from 'next/server';
import axios from 'axios';

// GET 요청을 처리하는 함수
export async function GET(
  request: Request,
  { params }: { params: { userNum: string; seasonId: string } }
) {
  const { userNum, seasonId } = params;
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return NextResponse.json(
      { error: 'API key is not configured' },
      { status: 500 }
    );
  }

  const API_BASE_URL = 'https://open-api.bser.io/v1';

  try {
    // user/stats/{userNum}/{seasonId} 엔드포인트 호출
    const response = await axios.get(`${API_BASE_URL}/user/stats/${userNum}/${seasonId}`,
      {
        headers: {
          'x-api-key': apiKey,
          'Accept': 'application/json',
        },
      }
    );

    if (response.data.code !== 200) {
      return NextResponse.json(
        { error: response.data.message || 'Failed to fetch user stats' },
        { status: response.data.code || 500 }
      );
    }

    return NextResponse.json(response.data.userStats);

  } catch (error) {
    console.error('API request error:', error);
    return NextResponse.json(
      { error: 'An error occurred while fetching user stats.' },
      { status: 500 }
    );
  }
}
