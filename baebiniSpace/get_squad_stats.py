
import requests
import os
import json
from dotenv import load_dotenv

load_dotenv(dotenv_path='eter/.env')
API_KEY = os.getenv('etr_api')
USER_NUM = 989063
SEASON_ID = 7

url = f'https://open-api.bser.io/v1/user/stats/{USER_NUM}/{SEASON_ID}'
headers = {'x-api-key': API_KEY}

response = requests.get(url, headers=headers)
stats_data = response.json()

# 스쿼드(matchingTeamMode=3) 데이터만 필터링
squad_stats = [stat for stat in stats_data.get('userStats', []) if stat.get('matchingTeamMode') == 3]

if squad_stats:
    print(json.dumps(squad_stats[0], indent=4, ensure_ascii=False))
else:
    print("해당 시즌의 스쿼드 모드 데이터가 없습니다.")
