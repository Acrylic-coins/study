import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import Navbar from "@/components/Navbar"; // 수정된 경로

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "이터널 리턴 정보 사이트",
  description: "이터널 리턴의 모든 정보를 확인하세요.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ko">
      <body className={inter.className}>
        <Navbar />
        {children}
      </body>
    </html>
  );
}