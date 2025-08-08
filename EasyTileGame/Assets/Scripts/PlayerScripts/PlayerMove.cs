using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class PlayerMove : MonoBehaviour
{
    private enum Arrow { NONE, UP, DOWN, LEFT, RIGHT };  // ��� ����Ű�� ���ȴ��� Ȯ���ϱ� ���� ������

    private Transform playerTrans;

    private Player playerScr;

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // ���������� ������ �����ص� ����Ʈ. �ִ� 1������ �����ϴ�.
    private List<Arrow> arrowList = new List<Arrow>();

    private Coroutine moveCo;

    private Vector3 moveVector; // �̹� �����ӿ� �̵��ؾ� �ϴ� ���Ͱ�
    private Vector3 startPos; // Ÿ�� �̵� ���� ��ġ

    private float timeLapse; // ����� �ð�
    private float timeGage; // �ѹ� �̵��ϴµ� �ɸ��� �ð�
    private float timeLine; // ���� �̵��ð� �������

    private bool isReadyMove = false;   // �̵� ������ ��Ȳ������ ��Ÿ��
    private bool isMoving = false; // ���� �� �������� �̵������� ��Ÿ��


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerTrans = this.transform;
        playerScr = this.GetComponent<Player>();

        startPos = playerTrans.position;

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;


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
        Debug.Log(arrowList.Count);
        // ������ ������ ������ �ڷ�ƾ ����
        if (arrowList.Count <= 0) { yield return oneFrame; StopCoroutine(moveCo); }




        while (arrowList.Count > 0)
        {
            // ������� ������ �������� �̵����� ������ �ȴ�.
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

            // ���� �̵����� ������ ����Ʈ���� ������. �ڸ� ������ ���Է�(1��������)�Ҽ�����
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

                playerTrans.position = startPos + moveVector;
                Debug.Log(moveX - (float)Constant.TILESIZE);
                // ��ǥ������ ���� ��������� �������� ����, ���� ��Ȳ�� ������
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

}
