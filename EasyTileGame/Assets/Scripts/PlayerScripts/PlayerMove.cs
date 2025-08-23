using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// �÷��̾��� �̵��� �ٷ�� ��ũ��Ʈ
public class PlayerMove : MonoBehaviour
{
    private enum Arrow { NONE, UP, DOWN, LEFT, RIGHT };  // ��� ����Ű�� ���ȴ��� Ȯ���ϱ� ���� ������

    [SerializeField] GameObject mapManager;

    private Transform playerTrans;  // �÷��̾� ������Ʈ�� Ʈ������

    private Player playerScr;
    private MapManager mapM; // �� ������Ʈ�� MapManagerŬ����

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // ���������� ������ �����ص� ����Ʈ. �ִ� 1������ �����ϴ�.
    private List<Arrow> arrowList = new List<Arrow>();

    private Coroutine moveCo;

    private Animator playerSprAnime;

    private Vector3 moveVector; // �̹� �����ӿ� �̵��ؾ� �ϴ� ���Ͱ�
    private Vector3 startPos; // Ÿ�� �̵� ���� ��ġ

    private float timeLapse; // ����� �ð�
    private float timeGage; // �ѹ� �̵��ϴµ� �ɸ��� �ð�
    private float timeLine; // ���� �̵��ð� �������

    public int nowCoordX { get; private set; }  // ���� Ÿ�� X��ǥ ��ġ
    public int nowCoordY { get; private set; }  // ���� Ÿ�� Y��ǥ ��ġ

    private int currentCoordX;  // ������ �־��� Ÿ�� X��ǥ ��ġ
    private int currentCoordY;  // ������ �־��� Ÿ�� Y��ǥ ��ġ
    private int endCoordX;  // �÷��̾ ��ǥ�� �ϴ� X��ǥ ��ġ
    private int endCoordY;  // �÷��̾ ��ǥ�� �ϴ� Y��ǥ ��ġ


    private bool isPenetrate = false;   // �̵� ��, ������ Ÿ���� ���� ���� ������ �������� ����
    private bool isReadyMove = false;   // �̵� ������ ��Ȳ������ ��Ÿ��
    private bool isMoving = false; // ���� �� �������� �̵������� ��Ÿ��

    private bool isMoveUp = true;   // �������� �̵��������� ���θ� ��Ÿ��
    private bool isMoveDown = true; // �Ʒ������� �̵��������� ���θ� ��Ÿ��
    private bool isMoveLeft = true; // �������� �̵��������� ���θ� ��Ÿ��
    private bool isMoveRight = true;    // ���������� �̵��������� ���θ� ��Ÿ��

    private bool isMoveMap = false; // ���� ��ǥ�� �̵����Ѿ� �ϴ� ���� �÷��̾����� ������ ���θ� ��Ÿ��


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // �ʱ�ȭ
        mapM = mapManager.GetComponent<MapManager>();

        playerSprAnime = this.GetComponent<Animator>(); // �÷��̾��� �ִϸ����� ������Ʈ ĳ��

        playerTrans = this.transform;   // �÷��̾��� Ʈ������ ĳ��
        playerScr = this.GetComponent<Player>();    // �ش� ������Ʈ�� 'Player' Ŭ���� ĳ��

        startPos = playerTrans.position;    // �÷��̾��� ������ġ�� ������

        nowCoordX = 5;  // �÷��̾��� Ÿ�� x��ǥ�� ������
        nowCoordY = 1;  // �÷��̾��� Ÿ�� y��ǥ�� ������

        currentCoordX = nowCoordX;  // �÷��̾��� ���� Ÿ�� x��ǥ�� ������
        currentCoordY = nowCoordY;  // �÷��̾��� ���� Ÿ�� y��ǥ�� ������

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;

        endCoordX = -1; // �÷��̾��� ��ǥ Ÿ�� X��ǥ�� ���� �������� ����
        endCoordY = -1; // �÷��̾��� ��ǥ Ÿ�� Y��ǥ�� ���� �������� ����
    }

    // �̵��� �� �� ȣ��Ǵ� �Լ�
    // InputAction�� Actions �� MOVE�� ����(�Լ��� : On + MOVE(Action �̸�)
    // Ű�� ������ 1��, ���� 0�� value�� ���޵�
    public void OnMOVE(InputValue value)
    {
        // Ű�� �� ���� ��� ��ȯ
        if (value.Get<float>() == 0f) { return; }
        // ������ �̹� 1�� �����صξ��ٸ� ��� ��ȯ. �߰��� ���Է� �Ұ���
        if (arrowList.Count >= 1) { return; }
        // �̵� ������ ��Ȳ���� üũ
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

        // �̵��� �ƴϸ� �ڷ�ƾ �ѱ�
        if (!isMoving)
        {
            moveCo = StartCoroutine(MoveCo());
        }
    }

    IEnumerator MoveCo()
    {
        // ������ ������ ������ �ڷ�ƾ ����
        if (arrowList.Count <= 0) { yield return oneFrame; StopCoroutine(moveCo); }

        while (arrowList.Count > 0)
        {
            if (!isMoveUp && arrowList[0] == Arrow.UP) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveDown && arrowList[0] == Arrow.DOWN) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveLeft && arrowList[0] == Arrow.LEFT) { yield return oneFrame; arrowList.RemoveAt(0); break; }
            if (!isMoveRight && arrowList[0] == Arrow.RIGHT) { yield return oneFrame; arrowList.RemoveAt(0); break; }

            // ������� ������ �������� �̵����� ������ �ȴ�.
            isMoving = true;
            // ��ǥ Ÿ�� �ܿ� �ٸ� Ÿ���� �ǵ���� ���������� �������� ����
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

            // ���� �̵����� ������ ����Ʈ���� ������. �ڸ� ������ ���Է�(1��������)�Ҽ�����
            arrowList.RemoveAt(0);
            // ���� �̵��ؾ� �ϴ� ���� �������� ������
            // �÷��̾ Ư�� ��ǥ���� ���ʿ� �ִٸ� �÷��̾� ��� ���� �̵���Ŵ
            if (currentCoordY >= 4 && arr == Arrow.UP) { isMoveMap = true; }
            else { isMoveMap = false; }

            // �� �����Ӹ��� �̵��Ÿ� ����� �ϰ� �÷��̾��� ��ǥ�� �ٲ���
            while (true)
            {
                // ���� �̵��� �ð��� �����                
                timeLapse += Time.deltaTime;
                // ���� ����� �ð��� 0�ʶ�� �ð��� ���� ������ while������ ���ư�
                if (timeLapse == 0) { continue; }
                // �ð��� ���� �̵� ��Ȳ�� ��%�� �Ǿ�� �ϴ��� �����
                timeLine = timeLapse / timeGage;

                if (arr == Arrow.UP)
                {
                    // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 1.5�� ���α׷����� ����
                    // �� �Ʒ��� ���ؼ��� ������ �ٿ��� �������� ������� �ʵ��� ��
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                    moveY = 1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveY, moveX);
                }
                else if (arr == Arrow.DOWN)
                {
                    // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 1.5�� ���α׷����� ����
                    // �� �Ʒ��� ���ؼ��� ������ �ٿ��� �������� ������� �ʵ��� ��
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                    moveY = -1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveY, moveX);
                }
                else if (arr == Arrow.LEFT)
                {
                    // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 8�� ���α׷����� ����
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                    moveY = -8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveX, moveY);
                }
                else if (arr == Arrow.RIGHT)
                {
                    // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 8�� ���α׷����� ����
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                    moveY = 8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                    moveVector = new Vector3(moveX, moveY);
                }

                // �÷��̾ ����� ��ġ��ŭ ���������� �̵���Ŵ
                UpdatePosition(moveVector, false);
                // ��ǥ������ ���� ��������� �������� ����, ���� ��Ȳ�� ������
                if ((float)Constant.TILESIZE - Mathf.Abs(moveX) < 0.05f)
                {
                    // ��ǥ ������ �÷��̾� ��ġ�� �̵���Ŵ
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
        // Ű�� �� ���� ��� ��ȯ
        if (value.Get<float>() == 0f) { return; }
        // �̵������� ��Ȳ �ƴϸ� ��ȯ
        if (!isReadyMove) { return; }
    }
    private void InitValue()
    {
        // ���������� ������
        startPos = playerTrans.position;
        // �̵��ؾ� �ϴ� ���� �÷��̾� ������Ʈ�� �ʱ�ȭ
        isMoveMap = false;

        timeGage = 0.4f;
        timeLapse = 0f;
        timeLine = 0f;
    }

    private void UpdatePosition(Vector3 move, bool isUpdate)
    {
        // �÷��̾� ������Ʈ�� �̵����Ѿ� �� ��
        if (!isMoveMap)
        {
            // �÷��̾��� ��ġ�� �����Ѵ�.
            playerTrans.position = startPos + move;
        }
        // �� ������Ʈ�� �̵����Ѿ� �� ��
        else
        {
            // ���� ��ġ�� �����Ѵ�.
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
