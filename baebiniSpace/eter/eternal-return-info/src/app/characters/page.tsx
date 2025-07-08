"use client";

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { getCharacterName } from '@/utils/characterMapping'; // 유틸리티 함수 임포트

// 실험체 데이터 타입 정의
interface Character {
  code: number;
  name: string;
  // 필요한 다른 속성들을 여기에 추가할 수 있습니다.
}

export default function CharactersPage() {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCharacters = async () => {
      try {
        setLoading(true);
        const response = await fetch('/api/data/Character');
        if (!response.ok) {
          throw new Error('실험체 정보를 불러오는 데 실패했습니다.');
        }
        const data = await response.json();
        setCharacters(data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchCharacters();
  }, []);

  return (
    <main className="flex min-h-screen flex-col items-center p-8 sm:p-16 bg-gray-900 text-white">
      <div className="w-full max-w-6xl">
        <h1 className="text-4xl font-bold mb-8 text-center">실험체 목록</h1>

        {loading && <p className="text-center">데이터를 불러오는 중입니다...</p>}
        {error && <p className="text-center text-red-500">오류: {error}</p>}

        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
          {characters.map((char) => (
            <Link key={char.code} href={`/characters/${char.code}`}>
              <div className="bg-gray-800 p-4 rounded-lg shadow-lg hover:bg-gray-700 transition-colors cursor-pointer">
                 {/* TODO: 실험체 이미지 API가 있다면 이미지를 표시합니다. */}
                <div className="aspect-square bg-gray-700 rounded-md mb-4"></div>
                <h2 className="text-lg font-semibold text-center">{getCharacterName(char.code)}</h2> {/* getCharacterName 사용 */}
              </div>
            </Link>
          ))}
        </div>
      </div>
    </main>
  );
}