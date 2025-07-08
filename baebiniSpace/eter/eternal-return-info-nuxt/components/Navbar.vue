<template>
  <nav class="bg-gray-800 p-4 shadow-md">
    <div class="container mx-auto flex justify-between items-center">
      <NuxtLink to="/" class="text-white text-2xl font-bold">
        이터널 리턴 정보
      </NuxtLink>
      <div class="space-x-6">
        <NuxtLink to="/" class="text-gray-300 hover:text-white transition-colors">
          홈
        </NuxtLink>
        <NuxtLink to="/characters" class="text-gray-300 hover:text-white transition-colors">
          실험체
        </NuxtLink>
      </div>
    </div>
  </nav>
</template>

<script setup>
</script>
