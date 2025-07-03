import { NextResponse } from 'next/server';
import axios from 'axios';

// GET 요청을 처리하는 함수
export async function GET(
  request: Request,
  { params }: { params: { nickname: string } }
) {
  const { nickname } = params;
  const apiKey = process.env.API_KEY;

  if (!apiKey) {
    return NextResponse.json(
      { error: 'API key is not configured' },
      { status: 500 }
    );
  }

  // 이터널 리턴 API의 기본 URL
  const API_BASE_URL = 'https://open-api.bser.io/v1';

  try {
    // 닉네임으로 유저 정보(userNum) 조회
    const response = await axios.get(`${API_BASE_URL}/user/nickname?query=${encodeURIComponent(nickname)}`, {
      headers: {
        'x-api-key': apiKey,
        'Accept': 'application/json',
      },
    });

    // API 응답이 성공적이지 않은 경우
    if (response.data.code !== 200) {
      return NextResponse.json(
        { error: response.data.message || 'Failed to fetch user data' },
        { status: response.data.code || 500 }
      );
    }

    // 성공적으로 데이터를 받아오면 클라이언트에 전달
    return NextResponse.json(response.data.user);

  } catch (error) {
    console.error('API request error:', error);
    return NextResponse.json(
      { error: 'An error occurred while fetching user data.' },
      { status: 500 }
    );
  }
}
