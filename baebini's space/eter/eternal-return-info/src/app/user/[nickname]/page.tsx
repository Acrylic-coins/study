"use client";

import { useParams } from 'next/navigation';
import { useEffect, useState, useMemo } from 'react';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  TooltipItem, // TooltipItem 임포트 추가
} from 'chart.js';
import { Bar } from 'react-chartjs-2';
import { getCharacterName } from '@/utils/characterMapping'; // 유틸리티 함수 임포트

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

// 데이터 타입 정의
interface User {
  userNum: number;
  nickname: string;
}

interface UserStat {
  seasonId: number;
  matchingMode: number;
  mmr: number;
  nickname: string;
  rank: number;
  totalGames: number;
  totalWins: number;
  averageRank: number;
  averageKills: number;
}

interface Game {
  gameId: number;
  matchingMode: number; // 3 for Squad
  mmrGain: number; // MMR 획득량
  // ... 기타 게임 정보
}

interface GameParticipant {
  userNum: number;
  nickname: string;
  teamNumber: number;
  characterCode: number; // 캐릭터 코드 추가
  attackPower: number;
  defense: number;
  skillAmp: number;
  bestWeaponLevel: number;
  maxHp: number;
  maxSp: number;
  moveSpeed: number;
  hpRegen: number;
  spRegen: number;
  attackSpeed: number;
  sightRange: number;
  attackRange: number;
  // ... 기타 참가자 스탯
}

interface SquadGameAnalysis {
  gameId: number;
  mmrGain: number;
  numParticipants: number; // 팀원 수 추가
  teamStats: {
    avgAttackPower: number;
    avgDefense: number;
    avgSkillAmp: number;
    avgWeaponMastery: number;
    avgMaxHp: number;
    avgMaxSp: number;
    avgMoveSpeed: number;
    avgHpRegen: number;
    avgSpRegen: number;
    avgAttackSpeed: number;
    avgSightRange: number;
    avgAttackRange: number;
  };
  // 각 스탯의 총합 추가
  totalStats: {
    totalAttackPower: number;
    totalDefense: number;
    totalSkillAmp: number;
    totalWeaponMastery: number;
    totalMaxHp: number;
    totalMaxSp: number;
    totalMoveSpeed: number;
    totalHpRegen: number;
    totalSpRegen: number;
    totalAttackSpeed: number;
    totalSightRange: number;
    totalAttackRange: number;
  };
  teamParticipants: GameParticipant[]; // 팀원 개개인의 스탯 추가
}

// 현재 시즌 (예시, 실제로는 API 등을 통해 동적으로 가져와야 할 수 있음)
const CURRENT_SEASON = 3;

export default function UserProfile() {
  const params = useParams();
  const { nickname } = params;
  
  const [user, setUser] = useState<User | null>(null);
  const [stats, setStats] = useState<UserStat[]>([]);
  const [selectedSeason, setSelectedSeason] = useState(CURRENT_SEASON);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [squadAnalysis, setSquadAnalysis] = useState<SquadGameAnalysis[]>([]);

  useEffect(() => {
    if (nickname) {
      const fetchUserData = async () => {
        try {
          setLoading(true);
          setError(null);

          // 1. 닉네임으로 유저 번호 조회
          const userRes = await fetch(`/api/user/${nickname}`);
          if (!userRes.ok) {
            throw new Error('유저 정보를 찾을 수 없습니다.');
          }
          const userData = await userRes.json();
          setUser(userData);

          // 2. 유저 번호로 통계 정보 조회
          const statsRes = await fetch(`/api/user-stats/${userData.userNum}/${selectedSeason}`);
          if (!statsRes.ok) {
            throw new Error('통계 정보를 불러오는 데 실패했습니다.');
          }
          const statsData = await statsRes.json();
          setStats(statsData);

          // 3. 유저의 최근 게임 목록 조회
          const gamesRes = await fetch(`/api/user-games/${userData.userNum}`);
          if (!gamesRes.ok) {
            throw new Error('게임 목록을 불러오는 데 실패했습니다.');
          }
          const gamesData: Game[] = await gamesRes.json();

          // 스쿼드 게임만 필터링
          const squadGames = gamesData.filter(game => game.matchingMode === 3);

          const analysisResults: SquadGameAnalysis[] = [];

          // 각 스쿼드 게임의 상세 정보 조회 및 분석
          for (const game of squadGames) {
            const gameDetailRes = await fetch(`/api/game-detail/${game.gameId}`);
            if (gameDetailRes.ok) {
              const gameDetailData: GameParticipant[] = await gameDetailRes.json();
              
              // 해당 유저의 팀 번호 찾기
              const userTeamNumber = gameDetailData.find(p => p.userNum === userData.userNum)?.teamNumber;

              if (userTeamNumber !== undefined) {
                const teamParticipants = gameDetailData.filter(p => p.teamNumber === userTeamNumber);

                if (teamParticipants.length > 0) {
                  const calculateSum = (key: keyof GameParticipant) => {
                    return teamParticipants.reduce((sum, p) => sum + (p[key] as number), 0);
                  };

                  const numParticipants = teamParticipants.length;

                  const totalAttackPower = calculateSum('attackPower');
                  const totalDefense = calculateSum('defense');
                  const totalSkillAmp = calculateSum('skillAmp');
                  const totalWeaponMastery = calculateSum('bestWeaponLevel');
                  const totalMaxHp = calculateSum('maxHp');
                  const totalMaxSp = calculateSum('maxSp');
                  const totalMoveSpeed = calculateSum('moveSpeed');
                  const totalHpRegen = calculateSum('hpRegen');
                  const totalSpRegen = calculateSum('spRegen');
                  const totalAttackSpeed = calculateSum('attackSpeed');
                  const totalSightRange = calculateSum('sightRange');
                  const totalAttackRange = calculateSum('attackRange');

                  analysisResults.push({
                    gameId: game.gameId,
                    mmrGain: game.mmrGain,
                    numParticipants,
                    teamStats: {
                      avgAttackPower: totalAttackPower / numParticipants,
                      avgDefense: totalDefense / numParticipants,
                      avgSkillAmp: totalSkillAmp / numParticipants,
                      avgWeaponMastery: totalWeaponMastery / numParticipants,
                      avgMaxHp: totalMaxHp / numParticipants,
                      avgMaxSp: totalMaxSp / numParticipants,
                      avgMoveSpeed: totalMoveSpeed / numParticipants,
                      avgHpRegen: totalHpRegen / numParticipants,
                      avgSpRegen: totalSpRegen / numParticipants,
                      avgAttackSpeed: totalAttackSpeed / numParticipants,
                      avgSightRange: totalSightRange / numParticipants,
                      avgAttackRange: totalAttackRange / numParticipants,
                    },
                    totalStats: {
                      totalAttackPower,
                      totalDefense,
                      totalSkillAmp,
                      totalWeaponMastery,
                      totalMaxHp,
                      totalMaxSp,
                      totalMoveSpeed,
                      totalHpRegen,
                      totalSpRegen,
                      totalAttackSpeed,
                      totalSightRange,
                      totalAttackRange,
                    },
                    teamParticipants: teamParticipants, // 팀원 개개인의 스탯 저장
                  });
                }
              }
            }
          }
          setSquadAnalysis(analysisResults);

        } catch (err: any) {
          setError(err.message);
          setUser(null);
          setStats([]);
          setSquadAnalysis([]);
        } finally {
          setLoading(false);
        }
      };

      fetchUserData();
    }
  }, [nickname, selectedSeason]);

  // MMR 총합 계산
  const totalMmrChange = useMemo(() => {
    return squadAnalysis.reduce((sum, analysis) => sum + analysis.mmrGain, 0);
  }, [squadAnalysis]);

  const mmrChangeColor = totalMmrChange >= 0 ? 'text-green-500' : 'text-red-500';

  const handleSeasonChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedSeason(Number(e.target.value));
  };

  const getModeName = (mode: number) => {
    switch (mode) {
      case 1: return '솔로';
      case 2: return '듀오';
      case 3: return '스쿼드';
      default: return '알 수 없음';
    }
  };

  // MMR 랭크 및 색상 결정 함수 (tier.png 정보가 들어오면 업데이트 예정)
  const getMmrRankInfo = (mmr: number) => {
    if (mmr >= 2000) return { rank: '이터널', color: '#FFD700' }; // Gold
    if (mmr >= 1800) return { rank: '데미갓', color: '#C0C0C0' }; // Silver
    if (mmr >= 1600) return { rank: '다이아몬드', color: '#00BFFF' }; // Deep Sky Blue
    if (mmr >= 1400) return { rank: '플래티넘', color: '#00CED1' }; // Dark Cyan
    if (mmr >= 1200) return { rank: '골드', color: '#FF8C00' }; // Dark Orange
    return { rank: '실버/브론즈', color: '#A9A9A9' }; // Dark Gray
  };

  const currentMmrRankInfo = user && stats.length > 0 && stats[0].mmr !== undefined ? getMmrRankInfo(stats[0].mmr) : null;

  // 그래프 라벨과 해당 라벨의 최대값 매핑
  const statMaxValues: { [key: string]: number } = {
    '공격력': 2000,
    '방어력': 2000,
    '스킬 증폭': 2000,
    '무기 숙련도': 2000,
    '최대 체력': 2000,
    '최대 스태미나': 2000,
    '이동 속도': 20,
    '체력 재생': 20,
    '스태미나 재생': 20,
    '공격 속도': 20,
    '시야': 20,
    '공격 사거리': 20,
  };

  return (
    <main className="flex min-h-screen flex-col items-center p-8 sm:p-16 bg-gray-900 text-white">
      <div className="w-full max-w-4xl">
        <h1 className="text-4xl font-bold mb-4 text-center">유저 정보</h1>
        
        {loading && <p className="text-center">데이터를 불러오는 중입니다...</p>}
        {error && <p className="text-center text-red-500">오류: {error}</p>}

        {user && (
          <div className="bg-gray-800 p-6 rounded-lg shadow-lg mb-8">
            <p className="text-3xl text-blue-400 font-semibold text-center mb-4">{user.nickname}</p>
            <div className="flex justify-center items-center gap-4 mb-6">
              <label htmlFor="season-select" className="text-lg">시즌 선택:</label>
              <select 
                id="season-select"
                value={selectedSeason}
                onChange={handleSeasonChange}
                className="bg-gray-700 text-white p-2 rounded-md"
              >
                {/* 시즌 목록 (예시) */}
                <option value="3">시즌 3</option>
                <option value="2">시즌 2</option>
                <option value="1">시즌 1</option>
              </select>
            </div>
          </div>
        )}

        {stats.length > 0 && (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            {stats.map((stat) => (
              <div key={stat.matchingMode} className="bg-gray-800 p-6 rounded-lg shadow-lg">
                <h2 className="text-2xl font-bold text-center text-green-400 mb-4">{getModeName(stat.matchingMode)}</h2>
                <ul className="space-y-2">
                  <li><span className="font-semibold">랭크:</span> {stat.rank} 위</li>
                  <li><span className="font-semibold">MMR:</span> {stat.mmr}</li>
                  <li><span className="font-semibold">총 게임:</span> {stat.totalGames} 판</li>
                  <li><span className="font-semibold">승리:</span> {stat.totalWins} 회</li>
                  <li><span className="font-semibold">평균 순위:</span> {stat.averageRank.toFixed(2)} 위</li>
                  <li><span className="font-semibold">평균 킬:</span> {stat.averageKills.toFixed(2)} 킬</li>
                </ul>
              </div>
            ))}
          </div>
        )}
        {!loading && !error && stats.length === 0 && user && (
            <p className="text-center text-gray-400 mb-8">해당 시즌의 통계 정보가 없습니다.</p>
        )}

        {/* 스쿼드 스탯 밸런스 분석 섹션 */}
        {!loading && !error && squadAnalysis.length > 0 && (
          <div className="bg-gray-800 p-6 rounded-lg shadow-lg">
            <h2 className="text-3xl font-bold mb-6 text-center">최근 스쿼드 게임 스탯 밸런스 분석</h2>
            
            {/* MMR 랭크 및 총합 표시 */}
            {currentMmrRankInfo && (
              <div className="text-center mb-4">
                <p className="text-xl font-semibold">현재 MMR 랭크: <span style={{ color: currentMmrRankInfo.color }}>{currentMmrRankInfo.rank}</span></p>
              </div>
            )}
            <div className="text-center mb-6">
              <p className="text-xl font-semibold">총 MMR 변동: <span className={`${mmrChangeColor}`}>{totalMmrChange >= 0 ? '+' : ''}{totalMmrChange}</span></p>
            </div>

            <div className="space-y-6">
              {squadAnalysis.map((analysis) => (
                <div key={analysis.gameId} className="bg-gray-700 p-4 rounded-lg">
                  <p className="text-xl font-semibold mb-2">게임 ID: {analysis.gameId} (MMR 획득: <span className={`${analysis.mmrGain >= 0 ? 'text-green-400' : 'text-red-400'}`}>{analysis.mmrGain >= 0 ? '+' : ''}{analysis.mmrGain}</span>)</p>
                  
                  {/* 캐릭터 조합 표시 */}
                  <div className="mb-4">
                    <h3 className="text-lg font-semibold mb-2">캐릭터 조합:</h3>
                    <ul className="list-disc list-inside ml-4">
                      {analysis.teamParticipants.map(p => (
                        <li key={p.userNum}>{getCharacterName(p.characterCode)} ({p.nickname})</li>
                      ))}
                    </ul>
                  </div>

                  <div className="mt-4">
                    <h3 className="text-lg font-semibold mb-2">팀원 평균 스탯</h3>
                    <Bar
                      data={{
                        labels: [
                          '공격력', '방어력', '스킬 증폭', '무기 숙련도',
                          '최대 체력', '최대 스태미나', '이동 속도',
                          '체력 재생', '스태미나 재생', '공격 속도',
                          '시야', '공격 사거리',
                        ],
                        datasets: [
                          {
                            label: '평균 스탯',
                            data: [
                              analysis.teamStats.avgAttackPower,
                              analysis.teamStats.avgDefense,
                              analysis.teamStats.avgSkillAmp,
                              analysis.teamStats.avgWeaponMastery,
                              analysis.teamStats.avgMaxHp,
                              analysis.teamStats.avgMaxSp,
                              analysis.teamStats.avgMoveSpeed,
                              analysis.teamStats.avgHpRegen,
                              analysis.teamStats.avgSpRegen,
                              analysis.teamStats.avgAttackSpeed,
                              analysis.teamStats.avgSightRange,
                              analysis.teamStats.avgAttackRange,
                            ],
                            backgroundColor: 'rgba(75, 192, 192, 0.6)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1,
                          },
                        ],
                      }}
                      options={{
                        responsive: true,
                        plugins: {
                          legend: {
                            position: 'top',
                            labels: { color: 'white' },
                          },
                          title: {
                            display: true,
                            text: '팀원 평균 스탯',
                            color: 'white',
                          },
                          tooltip: {
                            callbacks: {
                              label: function(context: TooltipItem<'bar'>) { 
                                return `${context.dataset.label}: ${context.raw.toFixed(2)}`;
                              }
                            }
                          }
                        },
                        scales: {
                          x: {
                            ticks: { color: 'white' },
                            grid: { color: 'rgba(255,255,255,0.1)' },
                          },
                          y: {
                            ticks: { color: 'white' },
                            grid: { color: 'rgba(255,255,255,0.1)' },
                            // 스탯별 최대 눈금 고정
                            max: (context) => {
                              const label = context.chart.data.labels[context.index];
                              if (typeof label === 'string' && statMaxValues[label] !== undefined) {
                                return statMaxValues[label];
                              }
                              return undefined; // 기본값
                            },
                          },
                        },
                      }}
                    />
                  </div>
                  
                  <div className="mt-4 text-sm">
                    <h3 className="text-lg font-semibold mb-2">계산식 (총합 / 팀원 수: {analysis.numParticipants})</h3>
                    <ul className="list-disc list-inside ml-4">
                      <li>공격력: {analysis.totalStats.totalAttackPower.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgAttackPower.toFixed(2)}</li>
                      <li>방어력: {analysis.totalStats.totalDefense.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgDefense.toFixed(2)}</li>
                      <li>스킬 증폭: {analysis.totalStats.totalSkillAmp.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgSkillAmp.toFixed(2)}</li>
                      <li>무기 숙련도: {analysis.totalStats.totalWeaponMastery.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgWeaponMastery.toFixed(2)}</li>
                      <li>최대 체력: {analysis.totalStats.totalMaxHp.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgMaxHp.toFixed(2)}</li>
                      <li>최대 스태미나: {analysis.totalStats.totalMaxSp.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgMaxSp.toFixed(2)}</li>
                      <li>이동 속도: {analysis.totalStats.totalMoveSpeed.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgMoveSpeed.toFixed(2)}</li>
                      <li>체력 재생: {analysis.totalStats.totalHpRegen.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgHpRegen.toFixed(2)}</li>
                      <li>스태미나 재생: {analysis.totalStats.totalSpRegen.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgSpRegen.toFixed(2)}</li>
                      <li>공격 속도: {analysis.totalStats.totalAttackSpeed.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgAttackSpeed.toFixed(2)}</li>
                      <li>시야: {analysis.totalStats.totalSightRange.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgSightRange.toFixed(2)}</li>
                      <li>공격 사거리: {analysis.totalStats.totalAttackRange.toFixed(2)} / {analysis.numParticipants} = {analysis.teamStats.avgAttackRange.toFixed(2)}</li>
                    </ul>
                  </div>

                  <div className="mt-6">
                    <h3 className="text-lg font-semibold mb-2">팀원 개별 스탯:</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      {analysis.teamParticipants.map(p => (
                        <div key={p.userNum} className={`p-3 rounded-md ${p.userNum === user?.userNum ? 'bg-blue-600' : 'bg-gray-600'}`}>
                          <p className="font-bold text-lg mb-1">{p.nickname} ({getCharacterName(p.characterCode)}) {p.userNum === user?.userNum && '(나)'}</p>
                          <ul className="list-disc list-inside ml-4 text-sm">
                            <li>공격력: {p.attackPower}</li>
                            <li>방어력: {p.defense}</li>
                            <li>스킬 증폭: {p.skillAmp}</li>
                            <li>무기 숙련도: {p.bestWeaponLevel}</li>
                            <li>최대 체력: {p.maxHp}</li>
                            <li>최대 스태미나: {p.maxSp}</li>
                            <li>이동 속도: {p.moveSpeed.toFixed(2)}</li>
                            <li>체력 재생: {p.hpRegen.toFixed(2)}</li>
                            <li>스태미나 재생: {p.spRegen.toFixed(2)}</li>
                            <li>공격 속도: {p.attackSpeed.toFixed(2)}</li>
                            <li>시야: {p.sightRange.toFixed(2)}</li>
                            <li>공격 사거리: {p.attackRange.toFixed(2)}</li>
                          </ul>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
        {!loading && !error && squadAnalysis.length === 0 && user && (
            <p className="text-center text-gray-400">최근 스쿼드 게임 분석 정보가 없습니다.</p>
        )}
      </div>
    </main>
  );
}