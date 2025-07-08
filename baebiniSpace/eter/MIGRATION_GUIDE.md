# Next.js에서 Nuxt.js로 마이그레이션 가이드

이 문서는 기존 `eternal-return-info` (Next.js) 프로젝트를 `eternal-return-info-nuxt` (Nuxt.js)로 마이그레이션하는 과정을 요약합니다.

## 1. Nuxt.js 프로젝트 생성

가장 먼저, Nuxt.js 프로젝트를 생성하기 위해 `nuxi` CLI를 사용했습니다.

```bash
npx nuxi init eternal-return-info-nuxt
```

## 2. 정적 파일 복사

기존 프로젝트의 `public` 폴더에 있던 SVG 파일들을 새로운 Nuxt.js 프로젝트의 `public` 폴더로 복사했습니다.

- `file.svg`
- `globe.svg`
- `next.svg`
- `vercel.svg`
- `window.svg`

## 3. 페이지 및 컴포넌트 변환

React 컴포넌트(.tsx)를 Vue 컴포넌트(.vue)로 변환했습니다. 이 과정에서 React의 Hooks (`useState`, `useEffect` 등)는 Vue의 Composition API (`ref`, `onMounted` 등)로 대체되었습니다.

- **`pages/index.vue`**: 메인 페이지, 유저 닉네임 검색 기능을 포함합니다.
- **`pages/characters/index.vue`**: 실험체 목록을 보여주는 페이지입니다.
- **`pages/user/[nickname].vue`**: 특정 유저의 상세 정보 및 최근 스쿼드 게임 분석을 차트와 함께 보여주는 동적 라우트 페이지입니다.
- **`components/Navbar.vue`**: 모든 페이지에 공통으로 사용될 네비게이션 바입니다.
- **`utils/characterMapping.ts`**: 캐릭터 코드를 이름으로 변환하는 유틸리티 함수는 그대로 `utils` 디렉토리로 옮겨 재사용했습니다.

## 4. 의존성 설치

페이지 변환 과정에서 필요한 라이브러리들을 설치했습니다.

- **`vue-chartjs` 및 `chart.js`**: 유저 상세 페이지의 스탯 분석 차트를 위해 설치했습니다.
- **`@nuxtjs/tailwindcss`**: Tailwind CSS를 Nuxt.js 프로젝트에서 사용하기 위해 설치했습니다.

```bash
# eternal-return-info-nuxt 디렉토리에서 실행
npm install vue-chartjs chart.js
npm install -D @nuxtjs/tailwindcss
```

## 5. 레이아웃 및 전역 CSS 설정

- **`app.vue`**: Nuxt.js의 메인 진입점으로, `Navbar` 컴포넌트를 포함하고 `<NuxtPage />`를 통해 각 페이지를 렌더링하도록 설정했습니다.
- **`assets/css/main.css`**: 기존 `globals.css`의 내용을 가져와 `assets/css` 디렉토리에 저장했습니다.
- **`nuxt.config.ts`**: 전역 CSS와 Tailwind CSS 모듈을 사용하도록 설정을 업데이트했습니다.

```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  devtools: { enabled: true },
  css: ["~/assets/css/main.css"],
  modules: ["@nuxtjs/tailwindcss"],
});
```

## 6. API 라우트 마이그레이션

Next.js의 API 라우트 (`src/app/api/...`)를 Nuxt.js의 서버 엔진인 Nitro에서 사용하는 `server/api/...` 형식으로 변환했습니다.

- `server/api/user/[nickname].ts`
- `server/api/user-stats/[userNum]/[seasonId].ts`
- `server/api/user-games/[userNum].ts`
- `server/api/game-detail/[gameId].ts`
- `server/api/data/Character.ts`

각 파일은 `defineEventHandler`를 사용하여 외부 API(이터널 리턴 OPEN API)를 호출하고 그 결과를 반환하도록 작성되었습니다.

## 7. 최종 설정

마지막으로, 프로젝트를 실행하기 위한 최종 단계를 안내했습니다.

1.  **환경 변수 설정**: 프로젝트 루트 디렉토리에 `.env` 파일을 생성하고 이터널 리턴 API 키를 추가해야 합니다.
    ```
    API_KEY=your_api_key_here
    ```
2.  **개발 서버 실행**: 다음 명령어를 통해 개발 서버를 시작할 수 있습니다.
    ```bash
    npm run dev
    ```
