using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.TestTools;

// 플레이어의 이동을 다루는 스크립트
public class PlayerMove : MonoBehaviour
{
    private enum State { NONE, UP, DOWN, LEFT, RIGHT, SKILL };  // 어느 방향키가 눌렸는지 확인하기 위한 열거형

    [SerializeField] GameObject mapManager;
    [SerializeField] ManaUIScripts manaUIScr;
    [SerializeField] Sprite moveSpr;
    [SerializeField] Sprite skillSpr;
    [SerializeField] Texture2D moveTex;
    [SerializeField] Texture2D skillTex;

    private Transform playerTrans;  // 플레이어 오브젝트의 트랜스폼
    private SpriteRenderer ren; // 플레이어 오브젝트의 스프라이트 렌더러

    private MapManager mapM; // 맵 오브젝트의 MapManager클래스
    private Player player;  // 플레이어 오브젝트의 Player클래스

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // 플레이어 스킬들의 내용을 저장해둔 딕셔너리
    private Dictionary<string, PlayerSkill> playerSkillDic = new Dictionary<string, PlayerSkill>();
    private Dictionary<Constant.ElementType, int> playerManaDic = new Dictionary<Constant.ElementType, int>();  // 플레이어의 마력을 기록한 딕셔너리

    private List<Dictionary<string, string>> skillList = new List<Dictionary<string, string>>(); // PlayerSkill.csv를 읽어오기 위한 리스트
    private List<Dictionary<string, string>> skillMoveList = new List<Dictionary<string, string>>();    // PlayerSkillMove.csv를 읽어오기 위한 리스트
    private List<Dictionary<string, string>> skillAttackList = new List<Dictionary<string, string>>();    // PlayerAttackMove.csv를 읽어오기 위한 리스트
    // 움직여야할 방향을 저장해둔 리스트. 최대 2개까지 가능하다.
    private List<State> stateList = new List<State>();
    // 현재 플레이어와 겹치고 있는 오브젝트들의 모음
    private List<Collider2D> colResultList = new List<Collider2D>();
    // 사용해야할 스킬을 저장해둔 리스트. 최대 2개까지 가능하다.
    private List<string> skillStateList = new List<string>();

    private Coroutine stateListCo;  // 상태를 처리하는 코루틴
    private Coroutine moveStateCo;  // 이동 상태를 실행하는 코루틴
    private Coroutine moveSkillStateCo; // 이동 스킬 상태를 실행하는 코루틴
    private Coroutine attackSkillStateCo; // 공격 스킬 상태를 실행하는 코루틴
    private Coroutine checkCoordCo; // 플레이어가 현재 어느 타일에 있는지 확인하는 코루틴

    private Animator playerSprAnime;

    private Vector3 startPos; // 타일 이동 시작 위치

    public int nowCoordX { get; private set; }  // 현재 타일 X좌표 위치
    public int nowCoordY { get; private set; }  // 현재 타일 Y좌표 위치

    private int currentCoordX;  // 최근에 있었던 타일 X좌표 위치
    private int currentCoordY;  // 최근에 있었던 타일 Y좌표 위치
    private int temCoordX;  // 임시 X좌표 위치
    private int temCoordY;  // 임시 Y좌표 위치
    private int endCoordX;  // 플레이어가 목표로 하는 X좌표 위치
    private int endCoordY;  // 플레이어가 목표로 하는 Y좌표 위치

    private int moveSpeed = 100;    // 플레이어의 이동속도. 기본속도는 100이다.

    private int temSpriteCount = 0; // 현재 사용중인 2d텍스쳐의 스프라이트 개수

    private bool isPenetrate = false;   // 이동 시, 지나간 타일을 전부 밟은 것으로 간주할지 여부
    private bool isReadyMove = false;   // 이동 가능한 상황인지를 나타냄
    private bool isStateProcess = false;    // stateList에 담긴 명령을 처리중인지를 나타냄
    private bool isMoveCoroutine = false;   // 이동 상태를 실행중인지 나타냄
    private bool isSkillCoroutine = false;  // 스킬 상태를 실행중인지 나타냄
    private bool isPlayerAir = false;   // 플레이어가 어떠한 타일도 밟지 않은 상태인지 확인함
    private bool isStepUpdate = false;  // 플레이어가 움직일 때 

    private bool isMoveUp = true;   // 위쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveDown = true; // 아래쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveLeft = true; // 왼쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveRight = true;    // 오른쪽으로 이동가능한지 여부를 나타냄

    private bool isMoveMap = false; // 현재 좌표를 이동시켜야 하는 것이 플레이어인지 맵인지 여부를 나타냄

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 초기화
        mapM = mapManager.GetComponent<MapManager>();
        ren = this.gameObject.GetComponent<SpriteRenderer>();
        player = this.gameObject.GetComponent<Player>();

        playerSprAnime = this.GetComponent<Animator>(); // 플레이어의 애니메이터 컴포넌트 캐싱

        playerTrans = this.transform;   // 플레이어의 트랜스폼 캐싱

        startPos = playerTrans.position;    // 플레이어의 시작위치를 정해줌

        nowCoordX = 5;  // 플레이어의 타일 x좌표를 정해줌
        nowCoordY = 1;  // 플레이어의 타일 y좌표를 정해줌

        currentCoordX = nowCoordX;  // 플레이어의 이전 타일 x좌표를 정해줌
        currentCoordY = nowCoordY;  // 플레이어의 이전 타일 y좌표를 정해줌

        endCoordX = -1; // 플레이어의 목표 타일 X좌표는 아직 정해지지 않음
        endCoordY = -1; // 플레이어의 목표 타일 Y좌표는 아직 정해지지 않음

        ren.sprite = moveSpr;

        InitSkillSetting();

        // 플레이어의 마력 초기화
        for (int i = 0; i < 8; i++)
        {
            playerManaDic.Add((Constant.ElementType)i, 100);
        }

        checkCoordCo = StartCoroutine(CheckPlayerCoord());
    }

    // 스킬을 생성하기 위한 함수
    private void InitSkillSetting()
    {
        skillList = CSVReader.ReadCSV("PlayerSkill.csv");
        skillMoveList = CSVReader.ReadCSV("PlayerSkillMove.csv");
        //skillAttackList = CSVReader.ReadCSV("PlayerSkillAttack.csv");

        string type = "";
        // 스킬 관련 정보가 csv 파일 몇 라인에 있는 지 알기 위함
        int ind = -1;

        // 스킬 개수만큼 차례로 정보를 불러옴
        for (int i = 0; i < skillList.Count; i++)
        {
            // 스킬의 타입을 읽어옴
            type = skillList[i]["PlayerSkillElementType"];

            // 어떤 스킬 타입에 맞는 클래스를 가져옴
            var t = TypeFinder.FindDerivedType<PlayerSkill>(type + "Skill");
            if (t != null)
            {
                // PlayerSkill을 상속한 t 클래스의 정보가 cl에 들어감
                var cl = Activator.CreateInstance(t) as PlayerSkill;
                // -------------------------------------------------------------------------------------------------------------------------------
                // ---------------------------------빈 껍데기뿐인 클래스에 스킬 정보를 입력-------------------------------------------------------
                // -------------------------------------------------------------------------------------------------------------------------------
                // 스킬의 속성을 갱신. 비교할 때 문자는 전부 대문자로 바꿔줘야함
                cl.elementType = (Constant.ElementType)Enum.Parse(typeof(Constant.ElementType), skillList[i]["PlayerSkillElementType"].ToUpper());
                // 스킬의 이름을 갱신
                cl.skillName = skillList[i]["PlayerSkillName"];
                // 스킬의 발동키를 갱신
                cl.activateKey = skillList[i]["PlayerSkillActiveKey"];
                // 스킬의 마나소모량을 갱신
                cl.skillMana = Int32.Parse(skillList[i]["PlayerSkillMana"]);
                // 스킬의 관통 여부를 갱신
                cl.isPenetrate = bool.Parse(skillList[i]["IsPenetrate"]);
                // 스킬의 스프라이트 변경 트리거 변수 이름을 갱신
                cl.playerSkillSpriteTrigger = skillList[i]["PlayerSkillSpriteTrigger"];
                // 스킬의 방식(MOVE, ATTACK 등)을 갱신
                cl.skillType = (Constant.SkillType)Enum.Parse(typeof(Constant.SkillType), skillList[i]["PlayerSkillType"]);
                // -------------------------------------------------------------------------------------------------------------------------------
                // ---------------------------------빈 껍데기뿐인 클래스에 스킬 정보를 입력-------------------------------------------------------
                // -------------------------------------------------------------------------------------------------------------------------------

                // 스킬 딕셔너리에 스킬에 관한 정보가 담긴 클래스를 추가함
                playerSkillDic.Add(skillList[i]["PlayerSkillCode"], cl);
            }
        }
    }

    // 이동을 할 때 호출되는 함수
    // InputAction의 Actions 중 MOVE와 연관(함수명 : On + MOVE(Action 이름)
    // 키를 누르면 1을, 떼면 0이 value에 전달됨
    public void OnMOVE(InputAction.CallbackContext ctx)
    {
        // 이동가능한 상태가 아니면 즉시 반환
        if (!player.isPlayerReady) { return; }
        // 키를 뗄 때는 즉시 반환
        if (ctx.action.WasReleasedThisFrame()) { return; }
        // 상태를 이미 1개 저장해두었다면 즉시 반환. 추가로 선입력 불가능
        if (stateList.Count >= 2) { return; }

        // 최근에 누른(뗀X) 버튼이 아래 방향키 일 때 
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            // 상태 리스트에 '아래 상태'를 추가
            stateList.Add(State.DOWN);
        }
        // 최근에 누른(뗀X) 버튼이 위 방향키 일 때
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            // 상태 리스트에 '위 상태'를 추가
            stateList.Add(State.UP);
        }
        // 최근에 누른(뗀X) 버튼이 오른쪽 방향키 일 때
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            // 상태 리스트에 '오른쪽 상태'를 추가
            stateList.Add(State.RIGHT);
        }
        // 최근에 누른(뗀X) 버튼이 왼쪽 방향키 일 때
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            // 상태 리스트에 '왼쪽 상태'를 추가
            stateList.Add(State.LEFT);
        }

        // 상태 처리 코루틴이 이미 발동중이면 코루틴을 실행하지 않는다.
        if (!isStateProcess)
        {
            // 상태 처리 코루틴을 발동시킨다.
            stateListCo = StartCoroutine(StateProcess());
        }
    }
    public void OnSKILL(InputAction.CallbackContext ctx)
    {
        // 이동가능한 상태가 아니면 즉시 반환
        if (!player.isPlayerReady) { return; }
        // 키를 뗄 때는 즉시 반환
        if (ctx.action.WasReleasedThisFrame()) { return; }
        // 상태를 이미 1개 저장해두었다면 즉시 반환. 추가로 선입력 불가능
        if (stateList.Count >= 2) { return; }

        // 발동키에 맞는 스킬이 리스트 몇번째 인덱스에 있는지 찾음 
        int inx = skillList.FindIndex(x => x["PlayerSkillActiveKey"].Equals(ctx.control.displayName));
        // 스킬 코드번호 (int형 아님)
        string skillNum = "";

        // 최근에 누른(뗀X) 버튼이 스킬 발동키일 때 
        if (inx != -1)
        {
            // 스킬 번호를 가져옴
            skillNum = skillList[inx]["PlayerSkillCode"];
            // 스킬에 필요한 마나를 충분히 가지고 있는지 확인(없으면 반환)

            // 상태 리스트에 '스킬'을 추가
            stateList.Add(State.SKILL);
            // 스킬 상태 리스트에 '스킬 코드번호' 추가
            skillStateList.Add(skillNum);
        }
        else { return; }

        // 상태 처리 코루틴이 이미 발동중이면 코루틴을 실행하지 않는다.
        if (!isStateProcess)
        {
            // 상태 처리 코루틴을 발동시킨다.
            stateListCo = StartCoroutine(StateProcess());
        }

        return;
    }

    // stateList에 저장된 상태를 입력 순서대로 발동시킨다.
    IEnumerator StateProcess()
    {
        // 해당 코루틴이 발동되고 있음을 나타냄
        isStateProcess = true;

        // 상태를 모두 발동시킬 때까지 실행한다.
        while (stateList.Count > 0)
        {
            // 가장 앞에 있는 상태를 발동시킨다.
            // 가장 앞에 있는 상태가 이동 관련 상태일 때
            if ((int)stateList[0] < 5)
            {
                moveStateCo = StartCoroutine(MoveCoroutine());
            }
            // 가장 앞에 있는 상태가 스킬 관련 상태일 때
            else
            {
                // 스킬의 타입이 '이동'일 때
                if (playerSkillDic[skillStateList[0]].skillType == Constant.SkillType.MOVE)
                {
                    moveSkillStateCo = StartCoroutine(MoveSkillCoroutine());
                }
                // 스킬의 타입이 '공격'일 때
                else if (playerSkillDic[skillStateList[0]].skillType == Constant.SkillType.ATTACK)
                {
                    attackSkillStateCo = StartCoroutine(AttackSkillCoroutine());
                }
            }

            // 어느 상태던, 코루틴(Move, Skill)이 실행되고 있다면 해당 루프를 계속 실행시킴
            // 코루틴이 끝날 때까지 다른 상태 명령이 실행되는 걸 막기 위함
            while (isMoveCoroutine || isSkillCoroutine)
            {
                // 1프레임 대기가 없으면 프로그램 멈춤
                yield return oneFrame;
            }

            // 실행이 끝난 상태를 종료함
            stateList.RemoveAt(0);
            // 더 실행할 상태가 없다면 즉시 코루틴 종료
            if (stateList.Count < 1)
            {
                // 해당 코루틴이 끝났음을 나타냄
                isStateProcess = false;
                // 코루틴 종료
                StopCoroutine(stateListCo);

                yield return null;

                break;
            }

            yield return oneFrame;
        }

        // 시작과 동시에 반환되는 것을 방지하기 위해(안하면 nullReference 오류뜸) 1프레임 대기함
        yield return oneFrame;
        // 해당 코루틴이 끝났음을 나타냄
        isStateProcess = false;
        // 코루틴 종료
        StopCoroutine(stateListCo);

        yield return null;
    }

    // 이동 상태를 실행시키는 코루틴
    IEnumerator MoveCoroutine()
    {
        // 해당 코루틴이 실행중임을 나타냄
        isMoveCoroutine = true;

        // 맵 끝부분 밖으로는 나갈 수 없음
        if ((!isMoveUp && stateList[0] == State.UP) || (!isMoveDown && stateList[0] == State.DOWN) ||
            (!isMoveLeft && stateList[0] == State.LEFT) || (!isMoveRight && stateList[0] == State.RIGHT))
        {
            yield return oneFrame;

            isMoveCoroutine = false;
            // 코루틴을 종료시킴
            StopCoroutine(moveStateCo);

            yield return null;
        }

        // 목표 타일 외에 다른 타일을 건드려도 밟은것으로 간주하지 않음
        isPenetrate = false;
        // 필요한 변수를 초기화함
        InitValue();
        // 목표지점을 알기 위한 변수
        Vector3 end = Vector3.zero;
        // 각각 시작지점에서 매 프레임 얼만큼 x,y방향으로 이동해야 할지를 정하기 위함
        float moveX = 0f;
        float moveY = 0f;
        // 이동속도에 따른 한 칸 이동에 걸리는 시간
        float moveTime = 100f / (moveSpeed * 3.5f);
        // 현재 이동 비율
        float moveRate = 0f;
        // 시작지점과 목표지점 사이의 비율을 나타냄
        float rate = 0;
        // 이동을 가로 혹은 세로로 하는지 결정하기 위함
        bool isWidth = false;
        // 이동 방식을 나타냄
        Constant.LineType lineType = Constant.LineType.CURVE;
        // 시작지점에서 목표지점까지의 좌표 차이를 구함
        if (stateList[0] == State.UP)
        {
            end = new Vector3(0f, 16f, 0f); playerSprAnime.SetTrigger("IsFront");
            endCoordX = nowCoordX; endCoordY = nowCoordY + 1; isWidth = false;
            if (endCoordY > 4) { endCoordY = 4; }
        }
        else if (stateList[0] == State.DOWN)
        {
            end = new Vector3(0f, -16f, 0f); playerSprAnime.SetTrigger("IsBack");
            endCoordX = nowCoordX; endCoordY = nowCoordY - 1; isWidth = false;
        }
        else if (stateList[0] == State.LEFT)
        {
            end = new Vector3(-16f, 0f, 0f); playerSprAnime.SetTrigger("IsLeft");
            endCoordX = nowCoordX - 1; endCoordY = nowCoordY; isWidth = true;
        }
        else if (stateList[0] == State.RIGHT)
        {
            end = new Vector3(16f, 0f, 0f); playerSprAnime.SetTrigger("IsRight");
            endCoordX = nowCoordX + 1; endCoordY = nowCoordY; isWidth = true;
        }

        State dir = stateList[0];
        // 목표지점이 같은 좌표라도 해당 변수가 true가 되지 않으면 갱신되지 않음
        isStepUpdate = false;

        // 매 프레임마다 이동거리 계산을 하고 플레이어의 좌표를 바꿔줌
        while (true)
        {
            // 1프레임 대기함
            yield return oneFrame;

            moveRate += Time.deltaTime;
            rate = moveRate / moveTime;
            
            // 이동 진행비율중 20~80%까지는 플레이어가 어느 타일도 밟지 않은 상태임
            if (rate < 0.8f && rate > 0.2f)
            {
                isPlayerAir = true;
            }
            else
            {
                isPlayerAir = false;
            }

            if (rate < 0.99f)
            {
                // 계산한 비율만큼 이동할 거리 좌표 계산
                UpdatePosition(MoveCalculating(lineType, end, isWidth, rate), false, dir);
            }
            else
            {
                // 목표 위치로 이동
                UpdatePosition(end, true, dir);
                // 시작 지점을 갱신
                startPos = playerTrans.position;
                // 해당 스킬 관련 스프라이트 종료
                playerSprAnime.SetTrigger("IsEnd");
                // 해당 코루틴이 끝났음을 나타냄
                isMoveCoroutine = false;
                // 해당 코루틴을 종료시킴
                StopCoroutine(moveStateCo);

                yield return null;
                
                break;
            }
        }
        // 코루틴을 종료시킴
        StopCoroutine(moveStateCo);

        yield return null;
    }

    // 이동 스킬 상태를 실행시키는 코루틴
    IEnumerator MoveSkillCoroutine()
    {
        // 해당 스킬 정보가 몇번 째 줄에 담겨있는지 확인하기 위함 
        int inx = skillMoveList.FindIndex(x => x["PlayerSkillCode"] == skillStateList[0]);
        // 정보가 없다면 코루틴 즉시 종료
        if (inx == -1) { yield return oneFrame; StopCoroutine(moveSkillStateCo); yield return null; }
        // 어느 방향으로 이동해야 하는지를 가져옴
        State dir = (State)Enum.Parse(typeof(State), skillMoveList[inx]["MoveDirection"]);
        // 얼만큼 이동해야 하는 지를 가져옴
        int dist = Int32.Parse(skillMoveList[inx]["MoveDistance"]);
        // 필요한 변수를 초기화함
        InitValue();
        // 목표지점을 알기 위한 변수
        Vector3 end = Vector3.zero;

        // 이동속도에 따른 한 칸 이동에 걸리는 시간
        float moveTime = 100f / (moveSpeed * 3.5f);
        // 현재 이동 비율
        float moveRate = 0f;
        // 각각 시작지점에서 매 프레임 얼만큼 x,y방향으로 이동해야 할지를 정하기 위함
        float moveX = 0f;
        float moveY = 0f;
        // moveX, moveY 값에 곱하여 방향을 결정해주기 위함
        int xDir = 0;
        int yDir = 0;
        // 이동을 가로 혹은 세로로 하는지 결정하기 위함
        bool isWidth = false;
        
        // 해당 방향으로 원하는 거리만큼 이동 가능한지를 판단함
        // 나중에 방해물 변수도 포함해서 다시 코딩해야됨
        switch (dir)
        {
            // 왼쪽 이동일 떄
            case State.LEFT:
                // 왼쪽 끝을 넘어가면 이동 불가능
                if (nowCoordX - dist < 0) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // 이동 가능하다면 가야할 거리를 정해줌
                else { end = new Vector3(-dist * Constant.TILESIZE, 0f); xDir = -1; isWidth = true; endCoordX = nowCoordX - dist; endCoordY = nowCoordY; }
                break;
            // 오른쪽 이동일 때
            case State.RIGHT:
                // 오른쪽 끝을 넘어가면 이동 불가능
                if (nowCoordX + dist > 9) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // 이동 가능하다면 가야할 거리를 정해줌
                else { end = new Vector3(dist * Constant.TILESIZE, 0f); xDir = 1; isWidth = true; endCoordX = nowCoordX + dist; endCoordY = nowCoordY; }
                break;
            // 위쪽 이동일 때
            case State.UP:
                end = new Vector3(0f, dist * Constant.TILESIZE); yDir = 1; isWidth = false;
                endCoordX = nowCoordX; endCoordY = nowCoordY + dist;
                if (endCoordY > 4) { endCoordY = 4; }
                break;
            // 아래쪽 이동일 때
            case State.DOWN:
                // 아래쪽 끝을 넘어가면 이동 불가능
                if (nowCoordY - dist < 0) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // 이동 가능하다면 가야할 거리를 정해줌
                else { end = new Vector3(0f, -dist * Constant.TILESIZE); yDir = -1; isWidth = false; endCoordX = nowCoordX; endCoordY = nowCoordY - dist; }
                break;
        }

        // 모든 스킬 사용 조건이 만족되면 해당 코루틴이 실행중임을 나타냄
        isSkillCoroutine = true;
        // 목표 타일 외에 다른 타일을 건드려도 밟은것으로 간주할지 말지를 결정함 
        isPenetrate = playerSkillDic[skillStateList[0]].isPenetrate;


        // 이동 방식을 나타냄
        Constant.LineType lineType = (Constant.LineType)Enum.Parse(typeof(Constant.LineType), skillMoveList[inx]["PlayerSkillLine"]);
        // 시작지점과 목표지점 사이의 비율을 나타냄
        float rate = 0;
        // 스킬에 맞는 스프라이트로 변경함
        playerSprAnime.SetTrigger(playerSkillDic[skillMoveList[inx]["PlayerSkillCode"]].playerSkillSpriteTrigger);
        // 목표지점이 같은 좌표라도 해당 변수가 true가 되지 않으면 갱신되지 않음
        isStepUpdate = false;

        // 플레이어를 이동시킴
        while (true)
        {
            yield return oneFrame;

            moveRate += Time.deltaTime;
            rate = moveRate / moveTime;

            // 이동 진행비율중 20~80%까지는 플레이어가 어느 타일도 밟지 않은 상태임
            if (rate < 0.8f && rate > 0.2f)
            {
                isPlayerAir = true;
            }
            else
            {
                isPlayerAir = false;
            }

            if (rate < 0.99f)
            {
                // 계산한 비율만큼 이동할 거리 좌표 계산
                UpdatePosition(MoveCalculating(lineType, end, isWidth, rate), false, dir);
            }
            else
            {
                // 목표 위치로 이동
                UpdatePosition(end, true, dir);
                // 시작 지점을 갱신
                startPos = playerTrans.position;
                // 해당 스킬 관련 스프라이트 종료
                playerSprAnime.SetTrigger("IsEnd");
                // 현재 스킬 상태를 리스트에서 지워줌
                skillStateList.RemoveAt(0);
                // 현재 스킬 관련 코루틴이 끝났음을 알림
                isSkillCoroutine = false;
                // 해당 코루틴을 종료시킴
                StopCoroutine(moveSkillStateCo);

                yield return null;
                break;
            }
        }

        yield return oneFrame;

        // 해당 코루틴을 종료시킴
        StopCoroutine(moveSkillStateCo);

        yield return null;
    }

    // 공격 스킬 상태를 실행시키는 코루틴
    IEnumerator AttackSkillCoroutine()
    {
        // 해당 코루틴이 실행중임을 나타냄
        isSkillCoroutine = true;

        while (true)
        {

            yield return oneFrame;
        }

        yield return oneFrame;

        isSkillCoroutine = false;

        skillStateList.RemoveAt(0);

        StopCoroutine(attackSkillStateCo);

        yield return null;
    }

    // 시작점(startPos)에서 얼만큼 이동해야 하는지 계산하는 함수
    private Vector3 MoveCalculating(Constant.LineType lineType, Vector3 end, bool isWidth, float rate)
    {
        float isMove = 0f; // 이동하는 쪽
        float notMove = 0f;    // 안움직이는 쪽
        float dist = 0f;    // 이동해야 할 거리
        float amplitude = 0f;   // 곡선 이동시 곡선의 크기

        // 가로로 이동해야 하는지, 세로로 이동해야 하는지를 판단함
        // amplitude는 진폭. 작을수록 완만하게 휘어지며 이동
        if (isWidth) { dist = end.x; amplitude = 8f; }
        else { dist = end.y; amplitude = 1.5f; }
        
        switch (lineType)
        {
            // 직선일 때
            case Constant.LineType.BEELINE:
                // 직선 거리만 비율에 따라 계산함
                isMove = Mathf.Lerp(0, dist, rate);
                break;

            //곡선일 때
            case Constant.LineType.CURVE:
                // 직선 거리를 우선 계산함
                isMove = Mathf.Lerp(0, dist, rate);
                // 이후 높낮이를 계산하여 곡선 경로로 이동하게끔 함
                notMove = Mathf.Abs(amplitude * Mathf.Sin(Mathf.PI / 16f * isMove));
                break;
        }

        Vector3 vec = Vector3.zero;

        if (isWidth) { vec = new Vector3(isMove, notMove); }
        else { vec = new Vector3(notMove, isMove); }

        return vec;
    }

    private void InitValue()
    {
        // 시작지점을 정해줌
        startPos = playerTrans.position;
        // 이동해야 하는 것을 플레이어 오브젝트로 초기화
        isMoveMap = false;
    }
    // isUpdate는 맵을 이동시킬 때 원점으로 다시 position값을 맞춰주어야할 때 true로 설정함
    private void UpdatePosition(Vector3 move, bool isUpdate, State dir)
    {


        // 플레이어 오브젝트를 이동시켜야 할 때
        if (playerTrans.localPosition.y < 48 || dir != State.UP)
        {
            // 플레이어의 위치를 갱신한다.
            playerTrans.position = startPos + move;
        }
        // 맵 오브젝트를 이동시켜야 할 때
        else
        {
            if (dir == State.UP)
            {
                playerTrans.position = startPos + new Vector3(move.x, 0f);
                // 맵의 위치를 갱신한다.
                mapM.UpdateMapPosition(move, isUpdate);
            }
        }
    }

    public void UpdateSpriteCount(int cnt)
    {
        // 사용 중인 2d텍스쳐의 스프라이트 개수가 변동될 때만 발동
        if (cnt == temSpriteCount) { return; }
        else { temSpriteCount = cnt; }

        ren.material.SetFloat("_MultiSpriteCnt", cnt);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.isPlayerReady) { return; }

        if (collision.CompareTag("TILE"))
        {
            Collider2D myCol = GetComponent<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D().NoFilter();

            int count = myCol.Overlap(filter, colResultList);
            for (int i = 0; i < colResultList.Count; i++)
            {
                if (colResultList[i] != myCol && colResultList[i].CompareTag("COORDX"))
                {
                    int x = colResultList[i].GetComponent<CheckCoordinate>().coordX;
                    if (endCoordX == x) { temCoordX = x; }
                }
                if (colResultList[i] != myCol && colResultList[i].CompareTag("COORDY"))
                {
                    int y = colResultList[i].GetComponent<CheckCoordinate>().coordY;
                    if (endCoordY == y) { temCoordY = y; }
                }
            }
            //Debug.Log($"temCoordX : {temCoordX} temCoordY : {temCoordY} nowCoordX : {nowCoordX} nowCoordY : {nowCoordY} endCoordX : {endCoordX} endCoordY : {endCoordY}");
            colResultList.Clear();
        }
    }
    
    IEnumerator CheckPlayerCoord()
    {
        while (true)
        {
            if (isPlayerAir)
            {
                nowCoordX = -1; nowCoordY = -1;
                isStepUpdate = true;
            }
            else
            {
                if (endCoordX == temCoordX && endCoordY == temCoordY && isStepUpdate == true)
                {
                    nowCoordX = temCoordX; nowCoordY = temCoordY;
                    currentCoordX = nowCoordX; currentCoordY = nowCoordY;
                    temCoordX = -100; temCoordY = -100;
                    isStepUpdate = false;

                    if (nowCoordX >= 9) { isMoveRight = false; }
                    else if (nowCoordX <= 0) { isMoveLeft = false; }
                    else { isMoveRight = true; isMoveLeft = true; }

                    if (nowCoordY >= 9) { isMoveUp = false; }
                    else if (nowCoordY <= 0) { isMoveDown = false; }
                    else { isMoveUp = true; isMoveDown = true; }
                }
            }


            yield return null;
        }
    }
    // 플레이어의 마력 수치를 갱신시킴
    public void UpdatePlayerMana(Constant.ElementType type, int manaAmount)
    {
        // 해당 속성이 없거나 전(全)속성이 아니면 반환
        if (!playerManaDic.ContainsKey(type) && type != Constant.ElementType.ALL) { return; }

        Constant.ElementType t = Constant.ElementType.NONE;

        // 전속성 타일을 밟았다면 랜덤한 마력속성을 결정
        if (type == Constant.ElementType.ALL) 
        {
            t = playerManaDic.ElementAt(UnityEngine.Random.Range(0, playerManaDic.Count)).Key;
        }
        // 그렇지 않다면 타일에 맞는 마력속성 찾음
        else
        {
            t = type;
            //Debug.Log(t);
        }

        // 해당 속성의 마력 수치가 999를 넘어가면 999로 고정
        if (playerManaDic[t] + manaAmount > 999)
        {
            playerManaDic[t] = 999;
        }
        // 그렇지 않다면 수치만큼 더해줌
        else
        {
            playerManaDic[t] += manaAmount;
        }

        // ManaUI에 수치를 전달함
        manaUIScr.UpdateManaAmountText(t, playerManaDic[t]);
    }
}
