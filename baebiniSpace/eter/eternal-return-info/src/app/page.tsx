"use client";

import { useState } from 'react';
import { useRouter } from 'next/navigation';

export default function Home() {
  const [nickname, setNickname] = useState('');
  const router = useRouter();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (nickname.trim()) {
      router.push(`/user/${nickname}`);
    }
  };

  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-24 bg-gray-900 text-white">
      <div className="text-center">
        <h1 className="text-5xl font-bold mb-8">이터널 리턴 정보</h1>
        <p className="text-lg text-gray-400 mb-10">
          게임 내 다양한 정보를 확인하고 유저 전적을 검색해보세요.
        </p>
        <form onSubmit={handleSearch} className="flex justify-center">
          <input
            type="text"
            value={nickname}
            onChange={(e) => setNickname(e.target.value)}
            placeholder="유저 닉네임을 입력하세요"
            className="w-full max-w-md px-4 py-3 rounded-l-md bg-gray-100 text-gray-900 focus:outline-none"
          />
          <button
            type="submit"
            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-r-md"
          >
            검색
          </button>
        </form>
      </div>
    </main>
  );
}