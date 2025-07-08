<template>
  <main class="flex min-h-screen flex-col items-center p-8 sm:p-16 bg-gray-900 text-white">
    <div class="w-full max-w-4xl">
      <h1 class="text-4xl font-bold mb-4 text-center">유저 정보</h1>
      
      <p v-if="loading" class="text-center">데이터를 불러오는 중입니다...</p>
      <p v-if="error" class="text-center text-red-500">오류: {{ error }}</p>

      <div v-if="user" class="bg-gray-800 p-6 rounded-lg shadow-lg mb-8">
        <p class="text-3xl text-blue-400 font-semibold text-center mb-4">{{ user.nickname }}</p>
        <div class="flex justify-center items-center gap-4 mb-6">
          <label for="season-select" class="text-lg">시즌 선택:</label>
          <select 
            id="season-select"
            v-model="selectedSeason"
            @change="fetchUserData"
            class="bg-gray-700 text-white p-2 rounded-md"
          >
            <option value="3">시즌 3</option>
            <option value="2">시즌 2</option>
            <option value="1">시즌 1</option>
          </select>
        </div>
      </div>

      <div v-if="stats.length > 0" class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div v-for="stat in stats" :key="stat.matchingMode" class="bg-gray-800 p-6 rounded-lg shadow-lg">
          <h2 class="text-2xl font-bold text-center text-green-400 mb-4">{{ getModeName(stat.matchingMode) }}</h2>
          <ul class="space-y-2">
            <li><span class="font-semibold">랭크:</span> {{ stat.rank }} 위</li>
            <li><span class="font-semibold">MMR:</span> {{ stat.mmr }}</li>
            <li><span class="font-semibold">총 게임:</span> {{ stat.totalGames }} 판</li>
            <li><span class="font-semibold">승리:</span> {{ stat.totalWins }} 회</li>
            <li><span class="font-semibold">평균 순위:</span> {{ stat.averageRank.toFixed(2) }} 위</li>
            <li><span class="font-semibold">평균 킬:</span> {{ stat.averageKills.toFixed(2) }} 킬</li>
          </ul>
        </div>
      </div>
      <p v-if="!loading && !error && stats.length === 0 && user" class="text-center text-gray-400 mb-8">해당 시즌의 통계 정보가 없습니다.</p>

      <div v-if="!loading && !error && squadAnalysis.length > 0" class="bg-gray-800 p-6 rounded-lg shadow-lg">
        <h2 class="text-3xl font-bold mb-6 text-center">최근 스쿼드 게임 스탯 밸런스 분석</h2>
        
        <div v-if="currentMmrRankInfo" class="text-center mb-4">
          <p class="text-xl font-semibold">현재 MMR 랭크: <span :style="{ color: currentMmrRankInfo.color }">{{ currentMmrRankInfo.rank }}</span></p>
        </div>
        <div class="text-center mb-6">
          <p class="text-xl font-semibold">총 MMR 변동: <span :class="mmrChangeColor">{{ totalMmrChange >= 0 ? '+' : '' }}{{ totalMmrChange }}</span></p>
        </div>

        <div class="space-y-6">
          <div v-for="analysis in squadAnalysis" :key="analysis.gameId" class="bg-gray-700 p-4 rounded-lg">
            <p class="text-xl font-semibold mb-2">게임 ID: {{ analysis.gameId }} (MMR 획득: <span :class="analysis.mmrGain >= 0 ? 'text-green-400' : 'text-red-400'">{{ analysis.mmrGain >= 0 ? '+' : '' }}{{ analysis.mmrGain }}</span>)</p>
            
            <div class="mb-4">
              <h3 class="text-lg font-semibold mb-2">캐릭터 조합:</h3>
              <ul class="list-disc list-inside ml-4">
                <li v-for="p in analysis.teamParticipants" :key="p.userNum">{{ getCharacterName(p.characterCode) }} ({{ p.nickname }})</li>
              </ul>
            </div>

            <div class="mt-4">
              <h3 class="text-lg font-semibold mb-2">팀원 평균 스탯</h3>
              <Bar
                :data="{
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
                }"
                :options="{
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
                        label: function(context) {
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
                      max: (context) => {
                        const label = context.chart.data.labels[context.index];
                        if (typeof label === 'string' && statMaxValues[label] !== undefined) {
                          return statMaxValues[label];
                        }
                        return undefined;
                      },
                    },
                  },
                }"
              />
            </div>
            
            <div class="mt-4 text-sm">
              <h3 class="text-lg font-semibold mb-2">계산식 (총합 / 팀원 수: {{ analysis.numParticipants }})</h3>
              <ul class="list-disc list-inside ml-4">
                <li>공격력: {{ analysis.totalStats.totalAttackPower.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgAttackPower.toFixed(2) }}</li>
                <li>방어력: {{ analysis.totalStats.totalDefense.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgDefense.toFixed(2) }}</li>
                <li>스킬 증폭: {{ analysis.totalStats.totalSkillAmp.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgSkillAmp.toFixed(2) }}</li>
                <li>무기 숙련도: {{ analysis.totalStats.totalWeaponMastery.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgWeaponMastery.toFixed(2) }}</li>
                <li>최대 체력: {{ analysis.totalStats.totalMaxHp.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgMaxHp.toFixed(2) }}</li>
                <li>최대 스태미나: {{ analysis.totalStats.totalMaxSp.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgMaxSp.toFixed(2) }}</li>
                <li>이동 속도: {{ analysis.totalStats.totalMoveSpeed.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgMoveSpeed.toFixed(2) }}</li>
                <li>체력 재생: {{ analysis.totalStats.totalHpRegen.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgHpRegen.toFixed(2) }}</li>
                <li>스태미나 재생: {{ analysis.totalStats.totalSpRegen.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgSpRegen.toFixed(2) }}</li>
                <li>공격 속도: {{ analysis.totalStats.totalAttackSpeed.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgAttackSpeed.toFixed(2) }}</li>
                <li>시야: {{ analysis.totalStats.totalSightRange.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgSightRange.toFixed(2) }}</li>
                <li>공격 사거리: {{ analysis.totalStats.totalAttackRange.toFixed(2) }} / {{ analysis.numParticipants }} = {{ analysis.teamStats.avgAttackRange.toFixed(2) }}</li>
              </ul>
            </div>

            <div class="mt-6">
              <h3 class="text-lg font-semibold mb-2">팀원 개별 스탯:</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div v-for="p in analysis.teamParticipants" :key="p.userNum" :class="`p-3 rounded-md ${p.userNum === user?.userNum ? 'bg-blue-600' : 'bg-gray-600'}`">
                  <p class="font-bold text-lg mb-1">{{ p.nickname }} ({{ getCharacterName(p.characterCode) }}) {{ p.userNum === user?.userNum ? '(나)' : '' }}</p>
                  <ul class="list-disc list-inside ml-4 text-sm">
                    <li>공격력: {{ p.attackPower }}</li>
                    <li>방어력: {{ p.defense }}</li>
                    <li>스킬 증폭: {{ p.skillAmp }}</li>
                    <li>무기 숙련도: {{ p.bestWeaponLevel }}</li>
                    <li>최대 체력: {{ p.maxHp }}</li>
                    <li>최대 스태미나: {{ p.maxSp }}</li>
                    <li>이동 속도: {{ p.moveSpeed.toFixed(2) }}</li>
                    <li>체력 재생: {{ p.hpRegen.toFixed(2) }}</li>
                    <li>스태미나 재생: {{ p.spRegen.toFixed(2) }}</li>
                    <li>공격 속도: {{ p.attackSpeed.toFixed(2) }}</li>
                    <li>시야: {{ p.sightRange.toFixed(2) }}</li>
                    <li>공격 사거리: {{ p.attackRange.toFixed(2) }}</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <p v-if="!loading && !error && squadAnalysis.length === 0 && user" class="text-center text-gray-400">최근 스쿼드 게임 분석 정보가 없습니다.</p>
    </div>
  </main>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import { Bar } from 'vue-chartjs';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { getCharacterName } from '~/utils/characterMapping';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const route = useRoute();
const nickname = route.params.nickname;

const user = ref(null);
const stats = ref([]);
const selectedSeason = ref(3);
const loading = ref(true);
const error = ref(null);
const squadAnalysis = ref([]);

const fetchUserData = async () => {
  try {
    loading.value = true;
    error.value = null;

    const userRes = await fetch(`/api/user/${nickname}`);
    if (!userRes.ok) throw new Error('유저 정보를 찾을 수 없습니다.');
    const userData = await userRes.json();
    user.value = userData;

    const statsRes = await fetch(`/api/user-stats/${userData.userNum}/${selectedSeason.value}`);
    if (!statsRes.ok) throw new Error('통계 정보를 불러오는 데 실패했습니다.');
    const statsData = await statsRes.json();
    stats.value = statsData;

    const gamesRes = await fetch(`/api/user-games/${userData.userNum}`);
    if (!gamesRes.ok) throw new Error('게임 목록을 불러오는 데 실패했습니다.');
    const gamesData = await gamesRes.json();

    const squadGames = gamesData.filter(game => game.matchingMode === 3);
    const analysisResults = [];

    const delay = ms => new Promise(res => setTimeout(res, ms));

    for (const game of squadGames) {
      await delay(100); // Add a delay to prevent API rate limiting
      const gameDetailRes = await fetch(`/api/game-detail/${game.gameId}`);
      if (gameDetailRes.ok) {
        const gameDetailData = await gameDetailRes.json();
        const userTeamNumber = gameDetailData.find(p => p.userNum === userData.userNum)?.teamNumber;

        if (userTeamNumber !== undefined) {
          const teamParticipants = gameDetailData.filter(p => p.teamNumber === userTeamNumber);

          if (teamParticipants.length > 0) {
            const calculateSum = (key) => teamParticipants.reduce((sum, p) => sum + p[key], 0);
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
              teamParticipants: teamParticipants,
            });
          }
        }
      }
    }
    squadAnalysis.value = analysisResults;

  } catch (err) {
    error.value = err.message;
    user.value = null;
    stats.value = [];
    squadAnalysis.value = [];
  } finally {
    loading.value = false;
  }
};

onMounted(fetchUserData);

const totalMmrChange = computed(() => squadAnalysis.value.reduce((sum, analysis) => sum + analysis.mmrGain, 0));
const mmrChangeColor = computed(() => totalMmrChange.value >= 0 ? 'text-green-500' : 'text-red-500');

const getModeName = (mode) => {
  switch (mode) {
    case 1: return '솔로';
    case 2: return '듀오';
    case 3: return '스쿼드';
    default: return '알 수 없음';
  }
};

const getMmrRankInfo = (mmr) => {
  if (mmr >= 2000) return { rank: '이터널', color: '#FFD700' };
  if (mmr >= 1800) return { rank: '데미갓', color: '#C0C0C0' };
  if (mmr >= 1600) return { rank: '다이아몬드', color: '#00BFFF' };
  if (mmr >= 1400) return { rank: '플래티넘', color: '#00CED1' };
  if (mmr >= 1200) return { rank: '골드', color: '#FF8C00' };
  return { rank: '실버/브론즈', color: '#A9A9A9' };
};

const currentMmrRankInfo = computed(() => {
  if (user.value && stats.value.length > 0 && stats.value[0].mmr !== undefined) {
    return getMmrRankInfo(stats.value[0].mmr);
  }
  return null;
});

const statMaxValues = {
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
</script>
