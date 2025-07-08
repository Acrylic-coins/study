<template>
  <main class="flex min-h-screen flex-col items-center p-8 sm:p-16 bg-gray-900 text-white">
    <div class="w-full max-w-6xl">
      <h1 class="text-4xl font-bold mb-8 text-center">실험체 목록</h1>

      <p v-if="loading" class="text-center">데이터를 불러오는 중입니다...</p>
      <p v-if="error" class="text-center text-red-500">오류: {{ error }}</p>

      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
        <div v-for="char in characters" :key="char.code">
          <NuxtLink :to="`/characters/${char.code}`">
            <div class="bg-gray-800 p-4 rounded-lg shadow-lg hover:bg-gray-700 transition-colors cursor-pointer">
              <div class="aspect-square bg-gray-700 rounded-md mb-4"></div>
              <h2 class="text-lg font-semibold text-center">{{ getCharacterName(char.code) }}</h2>
            </div>
          </NuxtLink>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { getCharacterName } from '~/utils/characterMapping';

const characters = ref([]);
const loading = ref(true);
const error = ref(null);

onMounted(async () => {
  try {
    loading.value = true;
    const response = await fetch('/api/data/Character');
    if (!response.ok) {
      throw new Error('실험체 정보를 불러오는 데 실패했습니다.');
    }
    const data = await response.json();
    characters.value = data;
  } catch (err) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
});
</script>
