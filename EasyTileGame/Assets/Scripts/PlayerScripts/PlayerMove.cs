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

    private Transform playerTrans;

    private Player playerScr;

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // 움직여야할 방향을 저장해둔 리스트. 최대 1개까지 가능하다.
    private List<Arrow> arrowList = new List<Arrow>();

    private Coroutine moveCo;

    private Vector3 moveVector; // 이번 프레임에 이동해야 하는 벡터값
    private Vector3 startPos; // 타일 이동 시작 위치

    private float timeLapse; // 경과된 시간
    private float timeGage; // 한번 이동하는데 걸리는 시간
    private float timeLine; // 현재 이동시간 진행비율

    private int nowCoordX;  // 현재 타일 X좌표 위치
    private int nowCoordY;  // 현재 타일 Y좌표 위치

    private bool isReadyMove = false;   // 이동 가능한 상황인지를 나타냄
    private bool isMoving = false; // 현재 한 방향으로 이동중임을 나타냄

    private bool isMoveUp = true;   // 위쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveDown = true; // 아래쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveLeft = true; // 왼쪽으로 이동가능한지 여부를 나타냄
    private bool isMoveRight = true;    // 오른쪽으로 이동가능한지 여부를 나타냄


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerTrans = this.transform;
        playerScr = this.GetComponent<Player>();

        startPos = playerTrans.position;

        nowCoordX = 5;
        nowCoordY = 1;

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;


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

            InitValue();
            
            Arrow arr = arrowList[0];

            Vector3 end = Vector3.zero;

            float moveX = 0f;
            float moveY = 0f;

            if (arr == Arrow.UP) { end = new Vector3(0f, 16f, 0f); }
            else if (arr == Arrow.DOWN) { end = new Vector3(0f, -16f, 0f); }
            else if (arr == Arrow.LEFT) { end = new Vector3(-16f, 0f, 0f); }
            else if (arr == Arrow.RIGHT) { end = new Vector3(16f, 0f, 0f); }

            // 현재 이동중인 방향을 리스트에서 지워줌. 자리 비워줘야 선입력(1번까지만)할수있음
            arrowList.RemoveAt(0);

            while (true)
            {
                timeLapse += Time.deltaTime;
                if (timeLapse == 0) { continue; }

                timeLine = timeLapse / timeGage;

                if (arr == Arrow.UP)
                {
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

                playerTrans.position = startPos + moveVector;
                // 목표지점에 얼추 가까워지면 도착으로 간주, 이후 상황을 결정함
                if ((float)Constant.TILESIZE - Mathf.Abs(moveX) < 0.05f)
                {
                    playerTrans.position = startPos + end;

                    isMoving = false;

                    break;
                }

                yield return oneFrame;
            }

        }

        //yield return oneFrame;
        StopCoroutine(moveCo);

        yield return null;
    }

    private void InitValue()
    {
        startPos = playerTrans.position;

        timeGage = 0.4f;
        timeLapse = 0f;
        timeLine = 0f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReadyMove) { return; }

        if (collision.tag.Equals("COORDX"))
        {
            nowCoordX = collision.GetComponent<CheckCoordinate>().coordX;

            if (nowCoordX == 9) { isMoveRight = false; }
            else if (nowCoordX == 0) { isMoveLeft = false; }
            else { isMoveRight = true; isMoveLeft = true; }
        }

        if (collision.tag.Equals("COORDY"))
        {
            nowCoordY = collision.GetComponent<CheckCoordinate>().coordY;

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
