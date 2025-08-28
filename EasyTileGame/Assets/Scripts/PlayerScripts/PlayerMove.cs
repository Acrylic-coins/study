using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// �÷��̾��� �̵��� �ٷ�� ��ũ��Ʈ
public class PlayerMove : MonoBehaviour
{
    private enum State { NONE, UP, DOWN, LEFT, RIGHT, ICE1 };  // ��� ����Ű�� ���ȴ��� Ȯ���ϱ� ���� ������

    [SerializeField] GameObject mapManager;
    [SerializeField] Sprite moveSpr;
    [SerializeField] Sprite skillSpr;
    [SerializeField] Texture2D moveTex;
    [SerializeField] Texture2D skillTex;

    private Transform playerTrans;  // �÷��̾� ������Ʈ�� Ʈ������
    private SpriteRenderer ren;

    private MapManager mapM; // �� ������Ʈ�� MapManagerŬ����

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // �÷��̾� ��ų���� ������ �����ص� ��ųʸ�
    private Dictionary<string, PlayerSkill> playerSkillDic = new Dictionary<string, PlayerSkill>();

    private List<Dictionary<string, string>> skillList = new List<Dictionary<string, string>>(); // PlayerSkill.csv�� �о���� ���� ����Ʈ
    // ���������� ������ �����ص� ����Ʈ. �ִ� 1������ �����ϴ�.
    private List<State> stateList = new List<State>();

    private Coroutine skillCo;
    private Coroutine stateListCo;  // ���¸� ó���ϴ� �ڷ�ƾ
    private Coroutine moveStateCo;  // �̵� ���¸� �����ϴ� �ڷ�ƾ
    private Coroutine skillStateCo; // ��ų ���¸� �����ϴ� �ڷ�ƾ

    private Animator playerSprAnime;

    private Vector3 moveVector; // �̹� �����ӿ� �̵��ؾ� �ϴ� ���Ͱ�
    private Vector3 startPos; // Ÿ�� �̵� ���� ��ġ

    private float timeLapse; // ����� �ð�
    private float timeGage; // �ѹ� �̵��ϴµ� �ɸ��� �ð�
    private float timeLine; // ���� �̵��ð� �������
    private float skillGage;

    public int nowCoordX { get; private set; }  // ���� Ÿ�� X��ǥ ��ġ
    public int nowCoordY { get; private set; }  // ���� Ÿ�� Y��ǥ ��ġ

    private int currentCoordX;  // ������ �־��� Ÿ�� X��ǥ ��ġ
    private int currentCoordY;  // ������ �־��� Ÿ�� Y��ǥ ��ġ
    private int endCoordX;  // �÷��̾ ��ǥ�� �ϴ� X��ǥ ��ġ
    private int endCoordY;  // �÷��̾ ��ǥ�� �ϴ� Y��ǥ ��ġ

    private bool isPenetrate = false;   // �̵� ��, ������ Ÿ���� ���� ���� ������ �������� ����
    private bool isReadyMove = false;   // �̵� ������ ��Ȳ������ ��Ÿ��
    private bool isSkill = false;
    private bool isStateProcess = false;    // stateList�� ��� ����� ó���������� ��Ÿ��
    private bool isMoveCoroutine = false;   // �̵� ���¸� ���������� ��Ÿ��
    private bool isSkillCoroutine = false;  // ��ų ���¸� ���������� ��Ÿ��

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
        ren = this.gameObject.GetComponent<SpriteRenderer>();

        playerSprAnime = this.GetComponent<Animator>(); // �÷��̾��� �ִϸ����� ������Ʈ ĳ��

        playerTrans = this.transform;   // �÷��̾��� Ʈ������ ĳ��

        startPos = playerTrans.position;    // �÷��̾��� ������ġ�� ������

        nowCoordX = 5;  // �÷��̾��� Ÿ�� x��ǥ�� ������
        nowCoordY = 1;  // �÷��̾��� Ÿ�� y��ǥ�� ������

        currentCoordX = nowCoordX;  // �÷��̾��� ���� Ÿ�� x��ǥ�� ������
        currentCoordY = nowCoordY;  // �÷��̾��� ���� Ÿ�� y��ǥ�� ������

        timeGage = 1f;
        timeLapse = 0f;
        timeLine = 0f;
        skillGage = 1f;

        endCoordX = -1; // �÷��̾��� ��ǥ Ÿ�� X��ǥ�� ���� �������� ����
        endCoordY = -1; // �÷��̾��� ��ǥ Ÿ�� Y��ǥ�� ���� �������� ����

        ren.sprite = moveSpr;

        InitSkillSetting();
    }

    // ��ų�� �����ϱ� ���� �Լ�
    private void InitSkillSetting()
    {
        skillList = CSVReader.ReadCSV("PlayerSkill.csv");

        string type = "";

        // ��ų ������ŭ ���ʷ� ������ �ҷ���
        for (int i = 0; i < skillList.Count; i++)
        {
            // ��ų�� Ÿ���� �о��
            type = skillList[i]["PlayerSkillType"];

            // � ��ų Ÿ�������� ���� �������� Ŭ������ �ٸ�

            var t = TypeFinder.FindDerivedType<PlayerSkill>(type + "Skill");
            if (t != null)
            {
                var cl = Activator.CreateInstance(t) as PlayerSkill;
                
            }
        }
    }

    // �̵��� �� �� ȣ��Ǵ� �Լ�
    // InputAction�� Actions �� MOVE�� ����(�Լ��� : On + MOVE(Action �̸�)
    // Ű�� ������ 1��, ���� 0�� value�� ���޵�
    public void OnMOVE(InputValue value)
    {
        // Ű�� �� ���� ��� ��ȯ
        if (value.Get<float>() == 0f) { return; }
        // ���¸� �̹� 1�� �����صξ��ٸ� ��� ��ȯ. �߰��� ���Է� �Ұ���
        if (stateList.Count >= 2) { return; }

        // �ֱٿ� ����(��X) ��ư�� �Ʒ� ����Ű �� �� 
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            // ���� ����Ʈ�� '�Ʒ� ����'�� �߰�
            stateList.Add(State.DOWN);
        }
        // �ֱٿ� ����(��X) ��ư�� �� ����Ű �� ��
        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            // ���� ����Ʈ�� '�� ����'�� �߰�
            stateList.Add(State.UP);
        }
        // �ֱٿ� ����(��X) ��ư�� ������ ����Ű �� ��
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            // ���� ����Ʈ�� '������ ����'�� �߰�
            stateList.Add(State.RIGHT);
        }
        // �ֱٿ� ����(��X) ��ư�� ���� ����Ű �� ��
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            // ���� ����Ʈ�� '���� ����'�� �߰�
            stateList.Add(State.LEFT);
        }

        // ���� ó�� �ڷ�ƾ�� �̹� �ߵ����̸� �ڷ�ƾ�� �������� �ʴ´�.
        if (!isStateProcess)
        {
            // ���� ó�� �ڷ�ƾ�� �ߵ���Ų��.
            stateListCo = StartCoroutine(StateProcess());
        }
    }
    public void OnSKILL(InputValue value)
    {
        // Ű�� �� ���� ��� ��ȯ
        if (value.Get<float>() == 0f) { return; }
        // ���¸� �̹� 1�� �����صξ��ٸ� ��� ��ȯ. �߰��� ���Է� �Ұ���
        if (stateList.Count >= 2) { return; }
        
        // �ֱٿ� ����(��X) ��ư�� ZŰ �� �� 
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            // ���� ����Ʈ�� '���� ��ų1'�� �߰�
            stateList.Add(State.ICE1);
        }

        // ���� ó�� �ڷ�ƾ�� �̹� �ߵ����̸� �ڷ�ƾ�� �������� �ʴ´�.
        if (!isStateProcess)
        {
            // ���� ó�� �ڷ�ƾ�� �ߵ���Ų��.
            stateListCo = StartCoroutine(StateProcess());
        }

        return;
        // Ű�� �� ���� ��� ��ȯ
        if (value.Get<float>() == 0f) { return; }
        // ���¸� �̹� 1�� �����صξ��ٸ� ��� ��ȯ. �߰��� ���Է� �Ұ���
        if (stateList.Count >= 1) { return; }
        // �̵������� ��Ȳ �ƴϸ� ��ȯ
        if (!isReadyMove) { return; }

        // zŰ�� ���� �� �ߵ���
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            stateList.Add(State.ICE1);
        }

        // �̵��� �ƴϸ� �ڷ�ƾ �ѱ�
        if (!isSkill)
        {
            skillCo = StartCoroutine(SkillCo());
        }
    }

    IEnumerator SkillCo()
    {
        // ���°� ������ �ڷ�ƾ ����
        if (stateList.Count <= 0) { yield return oneFrame; StopCoroutine(skillCo); }

        while (stateList.Count > 0)
        {
            if ((int)stateList[0] < 5) { yield return oneFrame; continue; }

            if (!isMoveLeft && stateList[0] == State.ICE1) { yield return oneFrame; stateList.RemoveAt(0); break; }

            // ������� ������ �������� ��ų ������� ������ �ȴ�.
            isSkill = true;
            // ��ǥ Ÿ�� �ܿ� �ٸ� Ÿ���� �ǵ���� ���������� �������� ����
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

            // ���� ��ų�� ����Ʈ���� ������. �ڸ� ������ ���Է�(1��������)�Ҽ�����
            stateList.RemoveAt(0);

            if (currentCoordY >= 4 && ski == State.UP) { isMoveMap = true; }
            else { isMoveMap = false; }

            while(true)
            {
                // ���� �̵��� �ð��� �����                
                timeLapse += Time.deltaTime;
                // ���� ����� �ð��� 0�ʶ�� �ð��� ���� ������ while������ ���ư�
                if (timeLapse == 0) { continue; }
                // �ð��� ���� �̵� ��Ȳ�� ��%�� �Ǿ�� �ϴ��� �����
                timeLine = timeLapse / skillGage;

                if (ski == State.ICE1)
                {
                    moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);

                    moveVector = new Vector3(moveX, moveY);
                }

                // �÷��̾ ����� ��ġ��ŭ ���������� �̵���Ŵ
                UpdatePosition(moveVector, false);

                if ((float)Constant.TILESIZE - Mathf.Abs(moveX) < 0.05f)
                {
                    // ��ǥ ������ �÷��̾� ��ġ�� �̵���Ŵ
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

    // stateList�� ����� ���¸� �Է� ������� �ߵ���Ų��.
    IEnumerator StateProcess()
    {
        // �ش� �ڷ�ƾ�� �ߵ��ǰ� ������ ��Ÿ��
        isStateProcess = true;

        // ���¸� ��� �ߵ���ų ������ �����Ѵ�.
        while (stateList.Count > 0)
        {
            // ���� �տ� �ִ� ���¸� �ߵ���Ų��.
            // ���� �տ� �ִ� ���°� �̵� ���� ������ ��
            if ((int)stateList[0] < 5)
            {
                moveStateCo = StartCoroutine(MoveCoroutine());
            }
            // ���� �տ� �ִ� ���°� ��ų ���� ������ ��
            else
            {
                skillStateCo = StartCoroutine(SkillCoroutine());
            }

            // ��� ���´�, �ڷ�ƾ(Move, Skill)�� ����ǰ� �ִٸ� �ش� ������ ��� �����Ŵ
            // �ڷ�ƾ�� ���� ������ �ٸ� ���� ����� ����Ǵ� �� ���� ����
            while (isMoveCoroutine || isSkillCoroutine)
            {
                // 1������ ��Ⱑ ������ ���α׷� ����
                yield return oneFrame;
            }

            // ������ ���� ���¸� ������
            stateList.RemoveAt(0);
            // �� ������ ���°� ���ٸ� ��� ������ Ż����
            if (stateList.Count < 1)
            {
                break;
            }

            yield return oneFrame;
        }

        // ���۰� ���ÿ� ��ȯ�Ǵ� ���� �����ϱ� ����(���ϸ� nullReference ������) 1������ �����
        yield return oneFrame;
        // �ش� �ڷ�ƾ�� �������� ��Ÿ��
        isStateProcess = false;
        // �ڷ�ƾ ����
        yield return null;
    }

    // �̵� ���¸� �����Ű�� �ڷ�ƾ
    IEnumerator MoveCoroutine()
    {
        // �ش� �ڷ�ƾ�� ���������� ��Ÿ��
        isMoveCoroutine = true;

        // �������� �� �ö��� ���ϳ�, ���� ������ �ԷµǾ��ٸ� �ൿ�� ��� ������.
        if (!isMoveUp && stateList[0] == State.UP) { yield return oneFrame; yield return null; }
        // �Ʒ������� �� �ö��� ���ϳ�, �Ʒ��� ������ �ԷµǾ��ٸ� �ൿ�� ��� ������.
        if (!isMoveDown && stateList[0] == State.DOWN) { yield return oneFrame; yield return null; }
        // �������� �� �ö��� ���ϳ�, ���� ������ �ԷµǾ��ٸ� �ൿ�� ��� ������.
        if (!isMoveLeft && stateList[0] == State.LEFT) { yield return oneFrame; yield return null; }
        // ���������� �� �ö��� ���ϳ�, ������ ������ �ԷµǾ��ٸ� �ൿ�� ��� ������.
        if (!isMoveRight && stateList[0] == State.RIGHT) { yield return oneFrame; yield return null; }

        // ��ǥ Ÿ�� �ܿ� �ٸ� Ÿ���� �ǵ���� ���������� �������� ����
        isPenetrate = false;
        // �ʿ��� ������ �ʱ�ȭ��
        InitValue();
        // ��ǥ������ �˱� ���� ����
        Vector3 end = Vector3.zero;
        // ���� ������������ �� ������ ��ŭ x,y�������� �̵��ؾ� ������ ���ϱ� ����
        float moveX = 0f;
        float moveY = 0f;

        // ������������ ��ǥ���������� ��ǥ ���̸� ����
        if (stateList[0] == State.UP) { end = new Vector3(0f, 16f, 0f); playerSprAnime.SetTrigger("IsFront"); endCoordX = nowCoordX; endCoordY = nowCoordY + 1; }
        else if (stateList[0] == State.DOWN) { end = new Vector3(0f, -16f, 0f); playerSprAnime.SetTrigger("IsBack"); endCoordX = nowCoordX; endCoordY = nowCoordY - 1; }
        else if (stateList[0] == State.LEFT) { end = new Vector3(-16f, 0f, 0f); playerSprAnime.SetTrigger("IsLeft"); endCoordX = nowCoordX - 1; endCoordY = nowCoordY; }
        else if (stateList[0] == State.RIGHT) { end = new Vector3(16f, 0f, 0f); playerSprAnime.SetTrigger("IsRight"); endCoordX = nowCoordX + 1; endCoordY = nowCoordY; }

        // �� �����Ӹ��� �̵��Ÿ� ����� �ϰ� �÷��̾��� ��ǥ�� �ٲ���
        while (true)
        {
            // ���� �̵��� �ð��� �����                
            timeLapse += Time.deltaTime;
            // ���� ����� �ð��� 0�ʶ�� �ð��� ���� ������ while������ ���ư�
            if (timeLapse == 0) { continue; }
            // �ð��� ���� �̵� ��Ȳ�� ��%�� �Ǿ�� �ϴ��� �����
            timeLine = timeLapse / timeGage;

            if (stateList[0] == State.UP)
            {
                // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 1.5�� ���α׷����� ����
                // �� �Ʒ��� ���ؼ��� ������ �ٿ��� �������� ������� �ʵ��� ��
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE, timeLine);
                moveY = 1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveY, moveX);
            }
            else if (stateList[0] == State.DOWN)
            {
                // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 1.5�� ���α׷����� ����
                // �� �Ʒ��� ���ؼ��� ������ �ٿ��� �������� ������� �ʵ��� ��
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                moveY = -1.5f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveY, moveX);
            }
            else if (stateList[0] == State.LEFT)
            {
                // �ֱⰡ 32( | moveEndVector.x * 2 | �� ��ġ), ������ 8�� ���α׷����� ����
                moveX = Mathf.Lerp(moveX, Constant.TILESIZE * -1f, timeLine);
                moveY = -8f * Mathf.Sin(Mathf.PI / 16f * moveX);
                moveVector = new Vector3(moveX, moveY);
            }
            else if (stateList[0] == State.RIGHT)
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
                // �ش� �ڷ�ƾ�� �������� ��Ÿ��
                isMoveCoroutine = false;
                // �ش� �ڷ�ƾ�� �����Ŵ
                StopCoroutine(moveStateCo);

                yield return null;

                break;
            }

            // 1������ �����
            yield return oneFrame;
        }

        yield return null;
    }

    // ��ų ���¸� �����Ű�� �ڷ�ƾ
    IEnumerator SkillCoroutine()
    {
        // �ش� �ڷ�ƾ�� ���������� ��Ÿ��
        isSkillCoroutine = true;

        while (true)
        {

            yield return oneFrame;
        }


        yield return null;
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
        skillGage = 0.3f;
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
