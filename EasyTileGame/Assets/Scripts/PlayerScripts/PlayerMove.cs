using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// 플레이어의 이동을 다루는 스크립트
public class PlayerMove : MonoBehaviour
{
    private enum State { NONE, UP, DOWN, LEFT, RIGHT, ICE1 };  // 어느 방향키가 눌렸는지 확인하기 위한 열거형

    [SerializeField] GameObject mapManager;
    [SerializeField] Sprite moveSpr;
    [SerializeField] Sprite skillSpr;
    [SerializeField] Texture2D moveTex;
    [SerializeField] Texture2D skillTex;

    private Transform playerTrans;  // 플레이어 오브젝트의 트랜스폼
    private SpriteRenderer ren;

    private MapManager mapM; // 맵 오브젝트의 MapManager클래스

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // 플레이어 스킬들의 내용을 저장해둔 딕셔너리
    private Dictionary<string, PlayerSkill> playerSkillDic = new Dictionary<string, PlayerSkill>();

    private List<Dictionary<string, string>> skillList = new List<Dictionary<string, string>>(); // PlayerSkill.csv를 읽어오기 위한 리스트
    // 움직여야할 방향을 저장해둔 리스트. 최대 1개까지 가능하다.
    private List<State> stateList = new List<State>();

    private Coroutine skillCo;
    private Coroutine stateListCo;  // 상태를 처리하는 코루틴
    private Coroutine moveStateCo;  // 이동 상태를 실행하는 코루틴
    private Coroutine skillStateCo; // 스킬 상태를 실행하는 코루틴

    private Animator playerSprAnime;

    private Vector3 moveVector; // 이번 프레임에 이동해야 하는 벡터값
    private Vector3 startPos; // 타일 이동 시작 위치

    private float timeLapse; // 경과된 시간
    private float timeGage; // 한번 이동하는데 걸리는 시간
    private float timeLine; // 현재 이동시간 진행비율
    private float skillGage;

    public int nowCoordX { get; private set; }  // 현재 타일 X좌표 위치
    public int nowCoordY { get; private set; }  // 현재 타일 Y좌표 위치

    private int currentCoordX;  // 이전에 있었던 타일 X좌표 위치
    private int currentCoordY;  // 이전에 있었던 타일 Y좌표 위치
    private int endCoordX;  // 플레이어가 목표로 하는 X좌표 위치
    private int endCoordY;  // 플레이어가 목표로 하는 Y좌표 위치

    private bool isPenetrate = false;   // 이동 시, 지나간 타일을 전부 밟은 것으로 간주할지 여부
    private bool isReadyMove = false;   // 이동 가능한 상황인지를 나타냄
    private bool isSkill = false;
    private bool isStateProcess = false;    // stateList에 담긴 명령을 처리중인지를 나타냄
    private bool isMoveCoroutine = false;   // 이동 상태를 실행중인지 나타냄
    private bool isSkillCoroutine = false;  // 스킬 상태를 실행중인지 나타냄

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

        playerSprAnime = this.GetComponent<Animator>(); // 플레이어의 애니메이터 컴포넌트 캐싱

        playerTrans = this.transform;   // 플레이어의 트랜스폼 캐싱

        startPos = playerTrans.position;    // 플레이어의 시작위치를 정해줌

        nowCoordX = 5;  // 플레이어의 타일 x좌표를 정해줌
        nowCoordY = 1;  // 플레이어의 타일 y좌표를 정해줌

        currentCoordX = nowCoordX;  // 플레이어의 이전 타일 x좌표를 정해줌
        currentCoordY = nowCoordY;  // 플레이어의 이전 타일 y좌표를 정해줌

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;
        skillGage = 1f;

        endCoordX = -1; // 플레이어의 목표 타일 X좌표는 아직 정해지지 않음
        endCoordY = -1; // 플레이어의 목표 타일 Y좌표는 아직 정해지지 않음

        ren.sprite = moveSpr;

        InitSkillSetting();
    }

    // 스킬을 생성하기 위한 함수
    private void InitSkillSetting()
    {
        skillList = CSVReader.ReadCSV("PlayerSkill.csv");

        string type = "";

        // 스킬 개수만큼 차례로 정보를 불러옴
        for (int i = 0; i < skillList.Count; i++)
        {
            // 스킬의 타입을 읽어옴
            type = skillList[i]["PlayerSkillType"];

            // 어떤 스킬 타입인지에 따라 가져오는 클래스가 다름

            var t = TypeFinder.FindDerivedType<PlayerSkill>(type + "Skill");
            if (t != null)
            {
                var cl = Activator.CreateInstance(t) as PlayerSkill;
                
            }
        }
    }

    // 이동을 할 때 호출되는 함수
    // InputAction의 Actions 중 MOVE와 연관(함수명 : On + MOVE(Action 이름)
    // 키를 누르면 1을, 떼면 0이 value에 전달됨
    public void OnMOVE(InputValue value)
    {
        // 키를 뗄 때는 즉시 반환
        if (value.Get<float>() == 0f) { return; }
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
    public void OnSKILL(InputValue value)
    {
        // 키를 뗄 때는 즉시 반환
        if (value.Get<float>() == 0f) { return; }
        // 상태를 이미 1개 저장해두었다면 즉시 반환. 추가로 선입력 불가능
        if (stateList.Count >= 2) { return; }
        
        // 최근에 누른(뗀X) 버튼이 Z키 일 때 
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            // 상태 리스트에 '얼음 스킬1'을 추가
            stateList.Add(State.ICE1);
        }

        // 상태 처리 코루틴이 이미 발동중이면 코루틴을 실행하지 않는다.
        if (!isStateProcess)
        {
            // 상태 처리 코루틴을 발동시킨다.
            stateListCo = StartCoroutine(StateProcess());
        }

        return;
        // 키를 뗄 때는 즉시 반환
        if (value.Get<float>() == 0f) { return; }
        // 상태를 이미 1개 저장해두었다면 즉시 반환. 추가로 선입력 불가능
        if (stateList.Count >= 1) { return; }
        // 이동가능한 상황 아니면 반환
        if (!isReadyMove) { return; }

        // z키를 누를 때 발동함
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            stateList.Add(State.ICE1);
        }

        // 이동중 아니면 코루틴 켜기
        if (!isSkill)
        {
            skillCo = StartCoroutine(SkillCo());
        }
    }

    IEnumerator SkillCo()
    {
        // 상태가 없으면 코루틴 종료
        if (stateList.Count <= 0) { yield return oneFrame; StopCoroutine(skillCo); }

        while (stateList.Count > 0)
        {
            if ((int)stateList[0] < 5) { yield return oneFrame; continue; }

            if (!isMoveLeft && stateList[0] == State.ICE1) { yield return oneFrame; stateList.RemoveAt(0); break; }

            // 여기까지 진입한 시점부터 스킬 사용중인 것으로 된다.
            isSkill = true;
            // 목표 타일 외에 다른 타일을 건드려도 밟은것으로 간주하지 않음
            isPenetrate = false;
            InitValue();

            State ski = stateList[0];

            Vector3 end = Vector3.zero;

            float moveX = 0f;
            float moveY = 0f;

            if (ski == State.ICE1)
            {
                end = new Vector3(-16f, 0f, 0f); playerSprAnime.SetTrigger("IsICE1");
                //ren.sprite = skillSpr;
                ren.material.SetTexture("_MainTex", skillTex);
                ren.material.SetFloat("_MultiSpriteCnt", 4f);



                endCoordX = nowCoordX - 1;
                endCoordY = nowCoordY;
            }

            // 현재 스킬을 리스트에서 지워줌. 자리 비워줘야 선입력(1번까지만)할수있음
            stateList.RemoveAt(0);

            if (currentCoordY >= 4 && ski == State.UP) { isMoveMap = true; }
            else { isMoveMap = false; }

            while(true)
            {
                // 현재 이동한 시간을 계산함                
                timeLapse += Time.deltaTime;
                // 만약 경과한 시간이 0초라면 시간이 지날 때까지 while문으로 돌아감
                if (timeLapse == 0) { continue; }
                // 시간에 따른 이동 상황이 몇%가 되어야 하는지 계산함
                timeLine = timeLapse / skillGage;

                if (ski == State.ICE1)
                {
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);

                    moveVector = new Vector3(moveX, moveY);
                }

                // 플레이어를 계산한 수치만큼 기준점에서 이동시킴
                UpdatePosition(moveVector, false);

                if ((float)Constant.TILESIZE - Mathf.Abs(moveX) < 0.05f)
                {
                    // 목표 지점에 플레이어 위치를 이동시킴
                    UpdatePosition(end, true);

                    isSkill = false;
                    ren.sprite = moveSpr;
                    playerSprAnime.SetTrigger("IsEnd");
                    ren.material.SetTexture("_MainTex", moveTex);
                    break;
                }

                yield return oneFrame;
            }

            yield return oneFrame;
        }

        ren.material.SetFloat("_MultiSpriteCnt", 21f);

        StopCoroutine(skillCo);

        yield return null;
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
                skillStateCo = StartCoroutine(SkillCoroutine());
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
            // 더 실행할 상태가 없다면 즉시 루프를 탈출함
            if (stateList.Count < 1)
            {
                break;
            }

            yield return oneFrame;
        }

        // 시작과 동시에 반환되는 것을 방지하기 위해(안하면 nullReference 오류뜸) 1프레임 대기함
        yield return oneFrame;
        // 해당 코루틴이 끝났음을 나타냄
        isStateProcess = false;
        // 코루틴 종료
        yield return null;
    }

    // 이동 상태를 실행시키는 코루틴
    IEnumerator MoveCoroutine()
    {
        // 해당 코루틴이 실행중임을 나타냄
        isMoveCoroutine = true;

        // 위쪽으로 더 올라가지 못하나, 위쪽 방향이 입력되었다면 행동을 즉시 끝낸다.
        if (!isMoveUp && stateList[0] == State.UP) { yield return oneFrame; yield return null; }
        // 아래쪽으로 더 올라가지 못하나, 아래쪽 방향이 입력되었다면 행동을 즉시 끝낸다.
        if (!isMoveDown && stateList[0] == State.DOWN) { yield return oneFrame; yield return null; }
        // 왼쪽으로 더 올라가지 못하나, 왼쪽 방향이 입력되었다면 행동을 즉시 끝낸다.
        if (!isMoveLeft && stateList[0] == State.LEFT) { yield return oneFrame; yield return null; }
        // 오른쪽으로 더 올라가지 못하나, 오른쪽 방향이 입력되었다면 행동을 즉시 끝낸다.
        if (!isMoveRight && stateList[0] == State.RIGHT) { yield return oneFrame; yield return null; }

        // 목표 타일 외에 다른 타일을 건드려도 밟은것으로 간주하지 않음
        isPenetrate = false;
        // 필요한 변수를 초기화함
        InitValue();
        // 목표지점을 알기 위한 변수
        Vector3 end = Vector3.zero;
        // 각각 시작지점에서 매 프레임 얼만큼 x,y방향으로 이동해야 할지를 정하기 위함
        float moveX = 0f;
        float moveY = 0f;

        // 시작지점에서 목표지점까지의 좌표 차이를 구함
        if (stateList[0] == State.UP) { end = new Vector3(0f, 16f, 0f); playerSprAnime.SetTrigger("IsFront"); endCoordX = nowCoordX; endCoordY = nowCoordY + 1; }
        else if (stateList[0] == State.DOWN) { end = new Vector3(0f, -16f, 0f); playerSprAnime.SetTrigger("IsBack"); endCoordX = nowCoordX; endCoordY = nowCoordY - 1; }
        else if (stateList[0] == State.LEFT) { end = new Vector3(-16f, 0f, 0f); playerSprAnime.SetTrigger("IsLeft"); endCoordX = nowCoordX - 1; endCoordY = nowCoordY; }
        else if (stateList[0] == State.RIGHT) { end = new Vector3(16f, 0f, 0f); playerSprAnime.SetTrigger("IsRight"); endCoordX = nowCoordX + 1; endCoordY = nowCoordY; }

        // 매 프레임마다 이동거리 계산을 하고 플레이어의 좌표를 바꿔줌
        while (true)
        {
            // 현재 이동한 시간을 계산함                
            timeLapse += Time.deltaTime;
            // 만약 경과한 시간이 0초라면 시간이 지날 때까지 while문으로 돌아감
            if (timeLapse == 0) { continue; }
            // 시간에 따른 이동 상황이 몇%가 되어야 하는지 계산함
            timeLine = timeLapse / timeGage;

            if (stateList[0] == State.UP)
            {
                // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 1.5인 사인그래프를 구함
                // 위 아래에 관해서는 진폭을 줄여서 움직임이 어색하지 않도록 함
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                moveY = 1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveY, moveX);
            }
            else if (stateList[0] == State.DOWN)
            {
                // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 1.5인 사인그래프를 구함
                // 위 아래에 관해서는 진폭을 줄여서 움직임이 어색하지 않도록 함
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                moveY = -1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveY, moveX);
            }
            else if (stateList[0] == State.LEFT)
            {
                // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 8인 사인그래프를 구함
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                moveY = -8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveX, moveY);
            }
            else if (stateList[0] == State.RIGHT)
            {
                // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 8인 사인그래프를 구함
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                moveY = 8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveX, moveY);
            }

            // 플레이어를 계산한 수치만큼 기준점에서 이동시킴
            UpdatePosition(moveVector, false);
            // 목표지점에 얼추 가까워지면 도착으로 간주, 이후 상황을 결정함
            if ((float)Constant.TILESIZE - Mathf.Abs(moveX) < 0.05f)
            {
                // 목표 지점에 플레이어 위치를 이동시킴
                UpdatePosition(end, true);
                // 해당 코루틴이 끝났음을 나타냄
                isMoveCoroutine = false;
                // 해당 코루틴을 종료시킴
                StopCoroutine(moveStateCo);

                yield return null;

                break;
            }

            // 1프레임 대기함
            yield return oneFrame;
        }

        yield return null;
    }

    // 스킬 상태를 실행시키는 코루틴
    IEnumerator SkillCoroutine()
    {
        // 해당 코루틴이 실행중임을 나타냄
        isSkillCoroutine = true;

        while (true)
        {

            yield return oneFrame;
        }


        yield return null;
    }

    private void InitValue()
    {
        // 시작지점을 정해줌
        startPos = playerTrans.position;
        // 이동해야 하는 것을 플레이어 오브젝트로 초기화
        isMoveMap = false;

        timeGage = 0.4f;
        timeLapse = 0f;
        timeLine = 0f;
        skillGage = 0.3f;
    }

    private void UpdatePosition(Vector3 move, bool isUpdate)
    {
        // 플레이어 오브젝트를 이동시켜야 할 때
        if (!isMoveMap)
        {
            // 플레이어의 위치를 갱신한다.
            playerTrans.position = startPos + move;
        }
        // 맵 오브젝트를 이동시켜야 할 때
        else
        {
            // 맵의 위치를 갱신한다.
            mapM.UpdateMapPosition(move, isUpdate);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReadyMove) { return; }

        if (collision.tag.Equals("COORDX"))
        {
            int x = collision.GetComponent<CheckCoordinate>().coordX;

            if (!isPenetrate && x != endCoordX) { nowCoordX = -1; }
            else { nowCoordX = x; }

            if (nowCoordX != -1) { currentCoordX = nowCoordX; }

            if (nowCoordX == 9) { isMoveRight = false; }
            else if (nowCoordX == 0) { isMoveLeft = false; }
            else { isMoveRight = true; isMoveLeft = true; }
        }

        if (collision.tag.Equals("COORDY"))
        {
            int y = collision.GetComponent<CheckCoordinate>().coordY;

            if (!isPenetrate && y != endCoordY) { nowCoordY = -1; }
            else { nowCoordY = y; }

            if (nowCoordY != -1) { currentCoordY = nowCoordY; }

            if (nowCoordY == 9) { isMoveUp = false; }
            else if (nowCoordY == 0) { isMoveDown = false; }
            else { isMoveUp = true; isMoveDown = true; }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!isReadyMove) { return; }

        if (collision.tag.Equals("COORDX"))
        {
            nowCoordX = -1;
        }

        if (collision.tag.Equals("COORDY"))
        {
            nowCoordY = -1;
        }
    }
}
