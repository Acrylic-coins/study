
import requests
import os
from dotenv import load_dotenv

load_dotenv(dotenv_path='eter/.env')
API_KEY = os.getenv('etr_api')
NICKNAME = 'baebini'

url = f'https://open-api.bser.io/v1/user/nickname?query={NICKNAME}'
headers = {'x-api-key': API_KEY}

response = requests.get(url, headers=headers)
print(response.json())
