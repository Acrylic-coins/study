"use client";

import { useParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import { getCharacterName } from '@/utils/characterMapping'; // 유틸리티 함수 임포트

// 실험체 상세 정보 타입 정의 (API 응답에 맞춰 확장 필요)
interface CharacterDetail {
  code: number;
  name: string;
  attackPower: number;
  defense: number;
  skillAmp: number;
  // ... 기타 등등 API에서 제공하는 모든 스탯
}

export default function CharacterDetailPage() {
  const params = useParams();
  const { code } = params;
  const [character, setCharacter] = useState<CharacterDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (code) {
      const fetchCharacterDetail = async () => {
        try {
          setLoading(true);
          // 모든 캐릭터 정보를 가져와서 필터링합니다.
          const response = await fetch('/api/data/Character');
          if (!response.ok) {
            throw new Error('실험체 정보를 불러오는 데 실패했습니다.');
          }
          const data = await response.json();
          const foundCharacter = data.find((c: CharacterDetail) => c.code === Number(code));
          
          if (foundCharacter) {
            setCharacter(foundCharacter);
          } else {
            throw new Error('해당 실험체를 찾을 수 없습니다.');
          }

        } catch (err: any) {
          setError(err.message);
        } finally {
          setLoading(false);
        }
      };

      fetchCharacterDetail();
    }
  }, [code]);

  return (
    <main className="flex min-h-screen flex-col items-center p-8 sm:p-16 bg-gray-900 text-white">
      <div className="w-full max-w-4xl">
        {loading && <p className="text-center">데이터를 불러오는 중입니다...</p>}
        {error && <p className="text-center text-red-500">오류: {error}</p>}

        {character && (
          <div className="bg-gray-800 p-8 rounded-lg shadow-lg">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
              <div className="md:col-span-1 flex justify-center">
                 {/* TODO: 실험체 이미지 */}
                <div className="w-64 h-64 bg-gray-700 rounded-full"></div>
              </div>
              <div className="md:col-span-2">
                <h1 className="text-4xl font-bold mb-4">{getCharacterName(character.code)}</h1> {/* getCharacterName 사용 */}
                <p className="text-lg text-gray-400 mb-6">코드: {character.code}</p>
                <div className="grid grid-cols-2 gap-4 text-lg">
                  {/* API 응답에 따라 표시할 스탯 추가 */}
                  <p><span className="font-semibold">공격력:</span> {character.attackPower}</p>
                  <p><span className="font-semibold">방어력:</span> {character.defense}</p>
                  <p><span className="font-semibold">스킬 증폭:</span> {character.skillAmp}</p>
                </div>
                 {/* TODO: 스킬 정보, 스토리 등 추가 정보 표시 */}
              </div>
            </div>
          </div>
        )}
      </div>
    </main>
  );
}