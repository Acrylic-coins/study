const l10nData = `
            Character/Name/1
재키
            Character/Name/2
아야
            Character/Name/3
피오라
            Character/Name/4
매그너스
            Character/Name/5
자히르
            Character/Name/6
나딘
            Character/Name/7
현우
            Character/Name/8
하트
            Character/Name/9
아이솔
            Character/Name/10
리 다이린
            Character/Name/11
유키
            Character/Name/12
혜진
            Character/Name/13
쇼우
            Character/Name/14
키아라
            Character/Name/15
시셀라
            Character/Name/16
실비아
            Character/Name/17
아드리아나
            Character/Name/18
쇼이치
            Character/Name/19
엠마
            Character/Name/20
레녹스
            Character/Name/21
로지
            Character/Name/22
루크
            Character/Name/23
캐시
            Character/Name/24
아델라
            Character/Name/25
버니스
            Character/Name/26
바바라
            Character/Name/27
알렉스
            Character/Name/28
수아
            Character/Name/29
레온
            Character/Name/30
일레븐
            Character/Name/31
리오
            Character/Name/32
윌리엄
            Character/Name/33
니키
            Character/Name/34
나타폰
            Character/Name/35
얀
            Character/Name/36
이바
            Character/Name/37
다니엘
            Character/Name/38
제니
            Character/Name/39
카밀로
            Character/Name/40
클로에
            Character/Name/41
요한
            Character/Name/42
비앙카
            Character/Name/43
셀린
            Character/Name/44
에키온
            Character/Name/45
마이
            Character/Name/46
에이든
            Character/Name/47
라우라
            Character/Name/48
띠아
            Character/Name/49
펠릭스
            Character/Name/50
엘레나
            Character/Name/51
프리야
            Character/Name/52
아디나
            Character/Name/53
마커스
            Character/Name/54
칼라
            Character/Name/55
에스텔
            Character/Name/56
피올로
            Character/Name/57
마르티나
            Character/Name/58
헤이즈
            Character/Name/59
아이작
            Character/Name/60
타지아
            Character/Name/61
이렘
            Character/Name/62
테오도르
            Character/Name/63
이안
            Character/Name/64
바냐
            Character/Name/65
데비&마를렌
            Character/Name/66
아르다
            Character/Name/67
아비게일
            Character/Name/68
알론소
            Character/Name/69
레니
            Character/Name/70
츠바메
            Character/Name/71
케네스
            Character/Name/72
카티야
            Character/Name/73
샬럿
            Character/Name/74
다르코
            Character/Name/75
르노어
            Character/Name/76
가넷
            Character/Name/77
유민
            Character/Name/78
히스이
            Character/Name/79
유스티나
            Character/Name/80
이슈트반
            Character/Name/81
니아
        `;

const characterMap = new Map<number, string>();

l10nData.trim().split('\n').forEach(line => {
  const trimmedLine = line.trim();
  if (trimmedLine.startsWith('Character/Name/')) {
    const parts = trimmedLine.split('┃');
    if (parts.length === 2) {
      const keyParts = parts[0].split('/');
      if (keyParts.length === 3) {
        const charCode = parseInt(keyParts[2]);
        if (!isNaN(charCode)) {
          characterMap.set(charCode, parts[1]);
        }
      }
    }
  }
});

export const getCharacterName = (code: number): string => {
  return characterMap.get(code) || `알 수 없는 캐릭터 (${code})`;
};