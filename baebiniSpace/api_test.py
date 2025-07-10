import requests
import json
import os
from dotenv import load_dotenv

# .env 파일에서 환경 변수 로드
load_dotenv(dotenv_path='eter/.env')

# --- 기본 설정 ---
API_KEY = os.getenv('etr_api')
API_BASE_URL = 'https://open-api.bser.io'
HEADERS = {
    'x-api-key': API_KEY,
    'Accept': 'application/json'
}

# --- 테스트 파라미터 ---
USER_NUM = 5445428  # 이전에 조회한 'ANONYMOUS'의 userNum
SEASON_ID = 8      # 테스트할 시즌 ID (사용자 제공)
TEAM_MODE = 1      # 1: 솔로, 2: 듀오, 3: 스쿼드
META_TYPE = 'Character' # 조회할 메타 데이터 타입

def fetch_and_save(endpoint, output_filename):
    """지정된 엔드포인트로 API를 호출하고 결과를 JSON 파일로 저장합니다."""
    url = f"{API_BASE_URL}{endpoint}"
    print(f"\n----- {output_filename} 테스트 시작-----")
    print(f"API 요청: {url}")
    
    try:
        response = requests.get(url, headers=HEADERS)
        response.raise_for_status()  # 오류 발생 시 예외 처리
        
        data = response.json()
        
        if data.get("code", 0) != 200:
            print(f"API 오류: {data.get('message')}")
            return

        with open(output_filename, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=4)
        print(f"성공: '{output_filename}' 파일에 응답을 저장했습니다.")
        
    except requests.exceptions.RequestException as e:
        print(f"API 요청 중 오류가 발생했습니다: {e}")
    except Exception as e:
        print(f"파일 저장 중 오류가 발생했습니다: {e}")

if __name__ == "__main__":
    if not API_KEY:
        print("오류: .env 파일에 etr_api 키가 설정되지 않았거나 .env 파일을 찾을 수 없습니다.")
    else:
        # # 1. 랭킹 정보 조회 (v2 API 권한 문제로 실패)
        # SERVER_CODE = 10 # 10: Asia
        # fetch_and_save(f"/v2/rank/top/{SEASON_ID}/{SERVER_CODE}/{TEAM_MODE}", "rank_v2_response.json")
        
        # # 2. 유저 통계 조회 (테스트 완료)
        # fetch_and_save(f"/v1/user/stats/{USER_NUM}/{SEASON_ID}", "stats_response.json")
        
        # # 3. 유저 최근 게임 조회 (테스트 완료)
        # fetch_and_save(f"/v1/user/games/{USER_NUM}", "games_response.json")
        
        # 4. 게임 데이터 조회 (단독 테스트)
        fetch_and_save(f"/v2/data/{META_TYPE}", "data_response.json")