<template>
  <main class="flex min-h-screen flex-col items-center justify-center p-24 bg-gray-900 text-white">
    <div class="text-center">
      <h1 class="text-5xl font-bold mb-8">이터널 리턴 정보</h1>
      <p class="text-lg text-gray-400 mb-10">
        게임 내 다양한 정보를 확인하고 유저 전적을 검색해보세요.
      </p>
      <form @submit.prevent="handleSearch" class="flex justify-center">
        <input
          type="text"
          v-model="nickname"
          placeholder="유저 닉네임을 입력하세요"
          class="w-full max-w-md px-4 py-3 rounded-l-md bg-gray-100 text-gray-900 focus:outline-none"
        />
        <button
          type="submit"
          class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-r-md"
        >
          검색
        </button>
      </form>
    </div>
  </main>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const nickname = ref('');
const router = useRouter();

const handleSearch = () => {
  if (nickname.value.trim()) {
    router.push(`/user/${nickname.value}`);
  }
};
</script>
