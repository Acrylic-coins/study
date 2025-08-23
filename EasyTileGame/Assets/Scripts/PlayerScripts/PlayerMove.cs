using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// 플레이어의 이동을 다루는 스크립트
public class PlayerMove : MonoBehaviour
{
    private enum Arrow { NONE, UP, DOWN, LEFT, RIGHT };  // 어느 방향키가 눌렸는지 확인하기 위한 열거형

    [SerializeField] GameObject mapManager;

    private Transform playerTrans;  // 플레이어 오브젝트의 트랜스폼

    private Player playerScr;
    private MapManager mapM; // 맵 오브젝트의 MapManager클래스

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // 움직여야할 방향을 저장해둔 리스트. 최대 1개까지 가능하다.
    private List<Arrow> arrowList = new List<Arrow>();

    private Coroutine moveCo;

    private Animator playerSprAnime;

    private Vector3 moveVector; // 이번 프레임에 이동해야 하는 벡터값
    private Vector3 startPos; // 타일 이동 시작 위치

    private float timeLapse; // 경과된 시간
    private float timeGage; // 한번 이동하는데 걸리는 시간
    private float timeLine; // 현재 이동시간 진행비율

    public int nowCoordX { get; private set; }  // 현재 타일 X좌표 위치
    public int nowCoordY { get; private set; }  // 현재 타일 Y좌표 위치

    private int currentCoordX;  // 이전에 있었던 타일 X좌표 위치
    private int currentCoordY;  // 이전에 있었던 타일 Y좌표 위치
    private int endCoordX;  // 플레이어가 목표로 하는 X좌표 위치
    private int endCoordY;  // 플레이어가 목표로 하는 Y좌표 위치


    private bool isPenetrate = false;   // 이동 시, 지나간 타일을 전부 밟은 것으로 간주할지 여부
    private bool isReadyMove = false;   // 이동 가능한 상황인지를 나타냄
    private bool isMoving = false; // 현재 한 방향으로 이동중임을 나타냄

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

        playerSprAnime = this.GetComponent<Animator>(); // 플레이어의 애니메이터 컴포넌트 캐싱

        playerTrans = this.transform;   // 플레이어의 트랜스폼 캐싱
        playerScr = this.GetComponent<Player>();    // 해당 오브젝트의 'Player' 클래스 캐싱

        startPos = playerTrans.position;    // 플레이어의 시작위치를 정해줌

        nowCoordX = 5;  // 플레이어의 타일 x좌표를 정해줌
        nowCoordY = 1;  // 플레이어의 타일 y좌표를 정해줌

        currentCoordX = nowCoordX;  // 플레이어의 이전 타일 x좌표를 정해줌
        currentCoordY = nowCoordY;  // 플레이어의 이전 타일 y좌표를 정해줌

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;

        endCoordX = -1; // 플레이어의 목표 타일 X좌표는 아직 정해지지 않음
        endCoordY = -1; // 플레이어의 목표 타일 Y좌표는 아직 정해지지 않음
    }

    // 이동을 할 때 호출되는 함수
    // InputAction의 Actions 중 MOVE와 연관(함수명 : On + MOVE(Action 이름)
    // 키를 누르면 1을, 떼면 0이 value에 전달됨
    public void OnMOVE(InputValue value)
    {
        // 키를 뗄 때는 즉시 반환
        if (value.Get<float>() == 0f) { return; }
        // 방향을 이미 1개 저장해두었다면 즉시 반환. 추가로 선입력 불가능
        if (arrowList.Count >= 1) { return; }
        // 이동 가능한 상황인지 체크
        isReadyMove = playerScr.isPlayerReady;

        if (!isReadyMove) { return; }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            arrowList.Add(Arrow.UP);
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            arrowList.Add(Arrow.DOWN);
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            arrowList.Add(Arrow.LEFT);
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            arrowList.Add(Arrow.RIGHT);
        }

        // 이동중 아니면 코루틴 켜기
        if (!isMoving)
        {
            moveCo = StartCoroutine(MoveCo());
        }
    }

    IEnumerator MoveCo()
    {
        // 움직일 방향이 없으면 코루틴 종료
        if (arrowList.Count <= 0) { yield return oneFrame; StopCoroutine(moveCo); }

        while (arrowList.Count > 0)
        {
            if (!isMoveUp && arrowList[0] == Arrow.UP) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveDown && arrowList[0] == Arrow.DOWN) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveLeft && arrowList[0] == Arrow.LEFT) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveRight && arrowList[0] == Arrow.RIGHT) { yield return oneFrame; arrowList.RemoveAt(0); break; }

            // 여기까지 진입한 시점부터 이동중인 것으로 된다.
            isMoving = true;
            // 목표 타일 외에 다른 타일을 건드려도 밟은것으로 간주하지 않음
            isPenetrate = false;
            InitValue();

            Arrow arr = arrowList[0];

            Vector3 end = Vector3.zero;

            float moveX = 0f;
            float moveY = 0f;

            if (arr == Arrow.UP) { end = new Vector3(0f, 16f, 0f); playerSprAnime.SetTrigger("IsFront"); endCoordX = nowCoordX; endCoordY = nowCoordY + 1; }
            else if (arr == Arrow.DOWN) { end = new Vector3(0f, -16f, 0f); playerSprAnime.SetTrigger("IsBack"); endCoordX = nowCoordX; endCoordY = nowCoordY - 1; }
            else if (arr == Arrow.LEFT) { end = new Vector3(-16f, 0f, 0f); playerSprAnime.SetTrigger("IsLeft"); endCoordX = nowCoordX - 1; endCoordY = nowCoordY; }
            else if (arr == Arrow.RIGHT) { end = new Vector3(16f, 0f, 0f); playerSprAnime.SetTrigger("IsRight"); endCoordX = nowCoordX + 1; endCoordY = nowCoordY; }

            // 현재 이동중인 방향을 리스트에서 지워줌. 자리 비워줘야 선입력(1번까지만)할수있음
            arrowList.RemoveAt(0);
            // 현재 이동해야 하는 것이 무엇인지 결정함
            // 플레이어가 특정 좌표보다 위쪽에 있다면 플레이어 대신 맵을 이동시킴
            if (currentCoordY >= 4 && arr == Arrow.UP) { isMoveMap = true; }
            else { isMoveMap = false; }

            // 매 프레임마다 이동거리 계산을 하고 플레이어의 좌표를 바꿔줌
            while (true)
            {
                // 현재 이동한 시간을 계산함                
                timeLapse += Time.deltaTime;
                // 만약 경과한 시간이 0초라면 시간이 지날 때까지 while문으로 돌아감
                if (timeLapse == 0) { continue; }
                // 시간에 따른 이동 상황이 몇%가 되어야 하는지 계산함
                timeLine = timeLapse / timeGage;

                if (arr == Arrow.UP)
                {
                    // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 1.5인 사인그래프를 구함
                    // 위 아래에 관해서는 진폭을 줄여서 움직임이 어색하지 않도록 함
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                    moveY = 1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveY, moveX);
                }
                else if (arr == Arrow.DOWN)
                {
                    // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 1.5인 사인그래프를 구함
                    // 위 아래에 관해서는 진폭을 줄여서 움직임이 어색하지 않도록 함
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                    moveY = -1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveY, moveX);
                }
                else if (arr == Arrow.LEFT)
                {
                    // 주기가 32( | moveEndVector.x * 2 | 의 수치), 진폭이 8인 사인그래프를 구함
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                    moveY = -8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveX, moveY);
                }
                else if (arr == Arrow.RIGHT)
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

                    isMoving = false;

                    break;
                }

                yield return oneFrame;
            }

        }

        playerSprAnime.SetTrigger("IsEnd");
        //yield return oneFrame;
        StopCoroutine(moveCo);

        yield return null;
    }
    public void OnSKILL(InputValue value)
    {
        // 키를 뗄 때는 즉시 반환
        if (value.Get<float>() == 0f) { return; }
        // 이동가능한 상황 아니면 반환
        if (!isReadyMove) { return; }
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

        //if (!collision.tag.Equals("COORDY") && !collision.tag.Equals("COORDX"))
        //{
        //    Debug.Log("nowCoordX : " + nowCoordX + " nowCoordY : " + nowCoordY);
        //}
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
