import Link from 'next/link';

export default function Navbar() {
  return (
    <nav className="bg-gray-800 p-4 shadow-md">
      <div className="container mx-auto flex justify-between items-center">
        <Link href="/" className="text-white text-2xl font-bold">
          이터널 리턴 정보
        </Link>
        <div className="space-x-6">
          <Link href="/" className="text-gray-300 hover:text-white transition-colors">
            홈
          </Link>
          <Link href="/characters" className="text-gray-300 hover:text-white transition-colors">
            실험체
          </Link>
          {/* TODO: 루트 등 다른 카테고리 링크 추가 */}
        </div>
      </div>
    </nav>
  );
}
