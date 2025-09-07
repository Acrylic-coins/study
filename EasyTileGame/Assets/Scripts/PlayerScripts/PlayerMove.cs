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

// �÷��̾��� �̵��� �ٷ�� ��ũ��Ʈ
public class PlayerMove : MonoBehaviour
{
    private enum State { NONE, UP, DOWN, LEFT, RIGHT, SKILL };  // ��� ����Ű�� ���ȴ��� Ȯ���ϱ� ���� ������

    [SerializeField] GameObject mapManager;
    [SerializeField] ManaUIScripts manaUIScr;
    [SerializeField] Sprite moveSpr;
    [SerializeField] Sprite skillSpr;
    [SerializeField] Texture2D moveTex;
    [SerializeField] Texture2D skillTex;

    private Transform playerTrans;  // �÷��̾� ������Ʈ�� Ʈ������
    private SpriteRenderer ren; // �÷��̾� ������Ʈ�� ��������Ʈ ������

    private MapManager mapM; // �� ������Ʈ�� MapManagerŬ����
    private Player player;  // �÷��̾� ������Ʈ�� PlayerŬ����

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();

    // �÷��̾� ��ų���� ������ �����ص� ��ųʸ�
    private Dictionary<string, PlayerSkill> playerSkillDic = new Dictionary<string, PlayerSkill>();
    private Dictionary<Constant.ElementType, int> playerManaDic = new Dictionary<Constant.ElementType, int>();  // �÷��̾��� ������ ����� ��ųʸ�

    private List<Dictionary<string, string>> skillList = new List<Dictionary<string, string>>(); // PlayerSkill.csv�� �о���� ���� ����Ʈ
    private List<Dictionary<string, string>> skillMoveList = new List<Dictionary<string, string>>();    // PlayerSkillMove.csv�� �о���� ���� ����Ʈ
    private List<Dictionary<string, string>> skillAttackList = new List<Dictionary<string, string>>();    // PlayerAttackMove.csv�� �о���� ���� ����Ʈ
    // ���������� ������ �����ص� ����Ʈ. �ִ� 2������ �����ϴ�.
    private List<State> stateList = new List<State>();
    // ���� �÷��̾�� ��ġ�� �ִ� ������Ʈ���� ����
    private List<Collider2D> colResultList = new List<Collider2D>();
    // ����ؾ��� ��ų�� �����ص� ����Ʈ. �ִ� 2������ �����ϴ�.
    private List<string> skillStateList = new List<string>();

    private Coroutine stateListCo;  // ���¸� ó���ϴ� �ڷ�ƾ
    private Coroutine moveStateCo;  // �̵� ���¸� �����ϴ� �ڷ�ƾ
    private Coroutine moveSkillStateCo; // �̵� ��ų ���¸� �����ϴ� �ڷ�ƾ
    private Coroutine attackSkillStateCo; // ���� ��ų ���¸� �����ϴ� �ڷ�ƾ
    private Coroutine checkCoordCo; // �÷��̾ ���� ��� Ÿ�Ͽ� �ִ��� Ȯ���ϴ� �ڷ�ƾ

    private Animator playerSprAnime;

    private Vector3 startPos; // Ÿ�� �̵� ���� ��ġ

    public int nowCoordX { get; private set; }  // ���� Ÿ�� X��ǥ ��ġ
    public int nowCoordY { get; private set; }  // ���� Ÿ�� Y��ǥ ��ġ

    private int currentCoordX;  // �ֱٿ� �־��� Ÿ�� X��ǥ ��ġ
    private int currentCoordY;  // �ֱٿ� �־��� Ÿ�� Y��ǥ ��ġ
    private int temCoordX;  // �ӽ� X��ǥ ��ġ
    private int temCoordY;  // �ӽ� Y��ǥ ��ġ
    private int endCoordX;  // �÷��̾ ��ǥ�� �ϴ� X��ǥ ��ġ
    private int endCoordY;  // �÷��̾ ��ǥ�� �ϴ� Y��ǥ ��ġ

    private int moveSpeed = 100;    // �÷��̾��� �̵��ӵ�. �⺻�ӵ��� 100�̴�.

    private int temSpriteCount = 0; // ���� ������� 2d�ؽ����� ��������Ʈ ����

    private bool isPenetrate = false;   // �̵� ��, ������ Ÿ���� ���� ���� ������ �������� ����
    private bool isReadyMove = false;   // �̵� ������ ��Ȳ������ ��Ÿ��
    private bool isStateProcess = false;    // stateList�� ��� ����� ó���������� ��Ÿ��
    private bool isMoveCoroutine = false;   // �̵� ���¸� ���������� ��Ÿ��
    private bool isSkillCoroutine = false;  // ��ų ���¸� ���������� ��Ÿ��
    private bool isPlayerAir = false;   // �÷��̾ ��� Ÿ�ϵ� ���� ���� �������� Ȯ����
    private bool isStepUpdate = false;  // �÷��̾ ������ �� 

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
        player = this.gameObject.GetComponent<Player>();

        playerSprAnime = this.GetComponent<Animator>(); // �÷��̾��� �ִϸ����� ������Ʈ ĳ��

        playerTrans = this.transform;   // �÷��̾��� Ʈ������ ĳ��

        startPos = playerTrans.position;    // �÷��̾��� ������ġ�� ������

        nowCoordX = 5;  // �÷��̾��� Ÿ�� x��ǥ�� ������
        nowCoordY = 1;  // �÷��̾��� Ÿ�� y��ǥ�� ������

        currentCoordX = nowCoordX;  // �÷��̾��� ���� Ÿ�� x��ǥ�� ������
        currentCoordY = nowCoordY;  // �÷��̾��� ���� Ÿ�� y��ǥ�� ������

        endCoordX = -1; // �÷��̾��� ��ǥ Ÿ�� X��ǥ�� ���� �������� ����
        endCoordY = -1; // �÷��̾��� ��ǥ Ÿ�� Y��ǥ�� ���� �������� ����

        ren.sprite = moveSpr;

        InitSkillSetting();

        // �÷��̾��� ���� �ʱ�ȭ
        for (int i = 0; i < 8; i++)
        {
            playerManaDic.Add((Constant.ElementType)i, 100);
        }

        checkCoordCo = StartCoroutine(CheckPlayerCoord());
    }

    // ��ų�� �����ϱ� ���� �Լ�
    private void InitSkillSetting()
    {
        skillList = CSVReader.ReadCSV("PlayerSkill.csv");
        skillMoveList = CSVReader.ReadCSV("PlayerSkillMove.csv");
        //skillAttackList = CSVReader.ReadCSV("PlayerSkillAttack.csv");

        string type = "";
        // ��ų ���� ������ csv ���� �� ���ο� �ִ� �� �˱� ����
        int ind = -1;

        // ��ų ������ŭ ���ʷ� ������ �ҷ���
        for (int i = 0; i < skillList.Count; i++)
        {
            // ��ų�� Ÿ���� �о��
            type = skillList[i]["PlayerSkillElementType"];

            // � ��ų Ÿ�Կ� �´� Ŭ������ ������
            var t = TypeFinder.FindDerivedType<PlayerSkill>(type + "Skill");
            if (t != null)
            {
                // PlayerSkill�� ����� t Ŭ������ ������ cl�� ��
                var cl = Activator.CreateInstance(t) as PlayerSkill;
                // -------------------------------------------------------------------------------------------------------------------------------
                // ---------------------------------�� ��������� Ŭ������ ��ų ������ �Է�-------------------------------------------------------
                // -------------------------------------------------------------------------------------------------------------------------------
                // ��ų�� �Ӽ��� ����. ���� �� ���ڴ� ���� �빮�ڷ� �ٲ������
                cl.elementType = (Constant.ElementType)Enum.Parse(typeof(Constant.ElementType), skillList[i]["PlayerSkillElementType"].ToUpper());
                // ��ų�� �̸��� ����
                cl.skillName = skillList[i]["PlayerSkillName"];
                // ��ų�� �ߵ�Ű�� ����
                cl.activateKey = skillList[i]["PlayerSkillActiveKey"];
                // ��ų�� �����Ҹ��� ����
                cl.skillMana = Int32.Parse(skillList[i]["PlayerSkillMana"]);
                // ��ų�� ���� ���θ� ����
                cl.isPenetrate = bool.Parse(skillList[i]["IsPenetrate"]);
                // ��ų�� ��������Ʈ ���� Ʈ���� ���� �̸��� ����
                cl.playerSkillSpriteTrigger = skillList[i]["PlayerSkillSpriteTrigger"];
                // ��ų�� ���(MOVE, ATTACK ��)�� ����
                cl.skillType = (Constant.SkillType)Enum.Parse(typeof(Constant.SkillType), skillList[i]["PlayerSkillType"]);
                // -------------------------------------------------------------------------------------------------------------------------------
                // ---------------------------------�� ��������� Ŭ������ ��ų ������ �Է�-------------------------------------------------------
                // -------------------------------------------------------------------------------------------------------------------------------

                // ��ų ��ųʸ��� ��ų�� ���� ������ ��� Ŭ������ �߰���
                playerSkillDic.Add(skillList[i]["PlayerSkillCode"], cl);
            }
        }
    }

    // �̵��� �� �� ȣ��Ǵ� �Լ�
    // InputAction�� Actions �� MOVE�� ����(�Լ��� : On + MOVE(Action �̸�)
    // Ű�� ������ 1��, ���� 0�� value�� ���޵�
    public void OnMOVE(InputAction.CallbackContext ctx)
    {
        // �̵������� ���°� �ƴϸ� ��� ��ȯ
        if (!player.isPlayerReady) { return; }
        // Ű�� �� ���� ��� ��ȯ
        if (ctx.action.WasReleasedThisFrame()) { return; }
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
    public void OnSKILL(InputAction.CallbackContext ctx)
    {
        // �̵������� ���°� �ƴϸ� ��� ��ȯ
        if (!player.isPlayerReady) { return; }
        // Ű�� �� ���� ��� ��ȯ
        if (ctx.action.WasReleasedThisFrame()) { return; }
        // ���¸� �̹� 1�� �����صξ��ٸ� ��� ��ȯ. �߰��� ���Է� �Ұ���
        if (stateList.Count >= 2) { return; }

        // �ߵ�Ű�� �´� ��ų�� ����Ʈ ���° �ε����� �ִ��� ã�� 
        int inx = skillList.FindIndex(x => x["PlayerSkillActiveKey"].Equals(ctx.control.displayName));
        // ��ų �ڵ��ȣ (int�� �ƴ�)
        string skillNum = "";

        // �ֱٿ� ����(��X) ��ư�� ��ų �ߵ�Ű�� �� 
        if (inx != -1)
        {
            // ��ų ��ȣ�� ������
            skillNum = skillList[inx]["PlayerSkillCode"];
            // ��ų�� �ʿ��� ������ ����� ������ �ִ��� Ȯ��(������ ��ȯ)

            // ���� ����Ʈ�� '��ų'�� �߰�
            stateList.Add(State.SKILL);
            // ��ų ���� ����Ʈ�� '��ų �ڵ��ȣ' �߰�
            skillStateList.Add(skillNum);
        }
        else { return; }

        // ���� ó�� �ڷ�ƾ�� �̹� �ߵ����̸� �ڷ�ƾ�� �������� �ʴ´�.
        if (!isStateProcess)
        {
            // ���� ó�� �ڷ�ƾ�� �ߵ���Ų��.
            stateListCo = StartCoroutine(StateProcess());
        }

        return;
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
                // ��ų�� Ÿ���� '�̵�'�� ��
                if (playerSkillDic[skillStateList[0]].skillType == Constant.SkillType.MOVE)
                {
                    moveSkillStateCo = StartCoroutine(MoveSkillCoroutine());
                }
                // ��ų�� Ÿ���� '����'�� ��
                else if (playerSkillDic[skillStateList[0]].skillType == Constant.SkillType.ATTACK)
                {
                    attackSkillStateCo = StartCoroutine(AttackSkillCoroutine());
                }
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
            // �� ������ ���°� ���ٸ� ��� �ڷ�ƾ ����
            if (stateList.Count < 1)
            {
                // �ش� �ڷ�ƾ�� �������� ��Ÿ��
                isStateProcess = false;
                // �ڷ�ƾ ����
                StopCoroutine(stateListCo);

                yield return null;

                break;
            }

            yield return oneFrame;
        }

        // ���۰� ���ÿ� ��ȯ�Ǵ� ���� �����ϱ� ����(���ϸ� nullReference ������) 1������ �����
        yield return oneFrame;
        // �ش� �ڷ�ƾ�� �������� ��Ÿ��
        isStateProcess = false;
        // �ڷ�ƾ ����
        StopCoroutine(stateListCo);

        yield return null;
    }

    // �̵� ���¸� �����Ű�� �ڷ�ƾ
    IEnumerator MoveCoroutine()
    {
        // �ش� �ڷ�ƾ�� ���������� ��Ÿ��
        isMoveCoroutine = true;

        // �� ���κ� �����δ� ���� �� ����
        if ((!isMoveUp && stateList[0] == State.UP) || (!isMoveDown && stateList[0] == State.DOWN) ||
            (!isMoveLeft && stateList[0] == State.LEFT) || (!isMoveRight && stateList[0] == State.RIGHT))
        {
            yield return oneFrame;

            isMoveCoroutine = false;
            // �ڷ�ƾ�� �����Ŵ
            StopCoroutine(moveStateCo);

            yield return null;
        }

        // ��ǥ Ÿ�� �ܿ� �ٸ� Ÿ���� �ǵ���� ���������� �������� ����
        isPenetrate = false;
        // �ʿ��� ������ �ʱ�ȭ��
        InitValue();
        // ��ǥ������ �˱� ���� ����
        Vector3 end = Vector3.zero;
        // ���� ������������ �� ������ ��ŭ x,y�������� �̵��ؾ� ������ ���ϱ� ����
        float moveX = 0f;
        float moveY = 0f;
        // �̵��ӵ��� ���� �� ĭ �̵��� �ɸ��� �ð�
        float moveTime = 100f / (moveSpeed * 3.5f);
        // ���� �̵� ����
        float moveRate = 0f;
        // ���������� ��ǥ���� ������ ������ ��Ÿ��
        float rate = 0;
        // �̵��� ���� Ȥ�� ���η� �ϴ��� �����ϱ� ����
        bool isWidth = false;
        // �̵� ����� ��Ÿ��
        Constant.LineType lineType = Constant.LineType.CURVE;
        // ������������ ��ǥ���������� ��ǥ ���̸� ����
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
        // ��ǥ������ ���� ��ǥ�� �ش� ������ true�� ���� ������ ���ŵ��� ����
        isStepUpdate = false;

        // �� �����Ӹ��� �̵��Ÿ� ����� �ϰ� �÷��̾��� ��ǥ�� �ٲ���
        while (true)
        {
            // 1������ �����
            yield return oneFrame;

            moveRate += Time.deltaTime;
            rate = moveRate / moveTime;
            
            // �̵� ��������� 20~80%������ �÷��̾ ��� Ÿ�ϵ� ���� ���� ������
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
                // ����� ������ŭ �̵��� �Ÿ� ��ǥ ���
                UpdatePosition(MoveCalculating(lineType, end, isWidth, rate), false, dir);
            }
            else
            {
                // ��ǥ ��ġ�� �̵�
                UpdatePosition(end, true, dir);
                // ���� ������ ����
                startPos = playerTrans.position;
                // �ش� ��ų ���� ��������Ʈ ����
                playerSprAnime.SetTrigger("IsEnd");
                // �ش� �ڷ�ƾ�� �������� ��Ÿ��
                isMoveCoroutine = false;
                // �ش� �ڷ�ƾ�� �����Ŵ
                StopCoroutine(moveStateCo);

                yield return null;
                
                break;
            }
        }
        // �ڷ�ƾ�� �����Ŵ
        StopCoroutine(moveStateCo);

        yield return null;
    }

    // �̵� ��ų ���¸� �����Ű�� �ڷ�ƾ
    IEnumerator MoveSkillCoroutine()
    {
        // �ش� ��ų ������ ��� ° �ٿ� ����ִ��� Ȯ���ϱ� ���� 
        int inx = skillMoveList.FindIndex(x => x["PlayerSkillCode"] == skillStateList[0]);
        // ������ ���ٸ� �ڷ�ƾ ��� ����
        if (inx == -1) { yield return oneFrame; StopCoroutine(moveSkillStateCo); yield return null; }
        // ��� �������� �̵��ؾ� �ϴ����� ������
        State dir = (State)Enum.Parse(typeof(State), skillMoveList[inx]["MoveDirection"]);
        // ��ŭ �̵��ؾ� �ϴ� ���� ������
        int dist = Int32.Parse(skillMoveList[inx]["MoveDistance"]);
        // �ʿ��� ������ �ʱ�ȭ��
        InitValue();
        // ��ǥ������ �˱� ���� ����
        Vector3 end = Vector3.zero;

        // �̵��ӵ��� ���� �� ĭ �̵��� �ɸ��� �ð�
        float moveTime = 100f / (moveSpeed * 3.5f);
        // ���� �̵� ����
        float moveRate = 0f;
        // ���� ������������ �� ������ ��ŭ x,y�������� �̵��ؾ� ������ ���ϱ� ����
        float moveX = 0f;
        float moveY = 0f;
        // moveX, moveY ���� ���Ͽ� ������ �������ֱ� ����
        int xDir = 0;
        int yDir = 0;
        // �̵��� ���� Ȥ�� ���η� �ϴ��� �����ϱ� ����
        bool isWidth = false;
        
        // �ش� �������� ���ϴ� �Ÿ���ŭ �̵� ���������� �Ǵ���
        // ���߿� ���ع� ������ �����ؼ� �ٽ� �ڵ��ؾߵ�
        switch (dir)
        {
            // ���� �̵��� ��
            case State.LEFT:
                // ���� ���� �Ѿ�� �̵� �Ұ���
                if (nowCoordX - dist < 0) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // �̵� �����ϴٸ� ������ �Ÿ��� ������
                else { end = new Vector3(-dist * Constant.TILESIZE, 0f); xDir = -1; isWidth = true; endCoordX = nowCoordX - dist; endCoordY = nowCoordY; }
                break;
            // ������ �̵��� ��
            case State.RIGHT:
                // ������ ���� �Ѿ�� �̵� �Ұ���
                if (nowCoordX + dist > 9) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // �̵� �����ϴٸ� ������ �Ÿ��� ������
                else { end = new Vector3(dist * Constant.TILESIZE, 0f); xDir = 1; isWidth = true; endCoordX = nowCoordX + dist; endCoordY = nowCoordY; }
                break;
            // ���� �̵��� ��
            case State.UP:
                end = new Vector3(0f, dist * Constant.TILESIZE); yDir = 1; isWidth = false;
                endCoordX = nowCoordX; endCoordY = nowCoordY + dist;
                if (endCoordY > 4) { endCoordY = 4; }
                break;
            // �Ʒ��� �̵��� ��
            case State.DOWN:
                // �Ʒ��� ���� �Ѿ�� �̵� �Ұ���
                if (nowCoordY - dist < 0) { yield return oneFrame; skillStateList.RemoveAt(0); StopCoroutine(moveSkillStateCo); yield return null; }
                // �̵� �����ϴٸ� ������ �Ÿ��� ������
                else { end = new Vector3(0f, -dist * Constant.TILESIZE); yDir = -1; isWidth = false; endCoordX = nowCoordX; endCoordY = nowCoordY - dist; }
                break;
        }

        // ��� ��ų ��� ������ �����Ǹ� �ش� �ڷ�ƾ�� ���������� ��Ÿ��
        isSkillCoroutine = true;
        // ��ǥ Ÿ�� �ܿ� �ٸ� Ÿ���� �ǵ���� ���������� �������� ������ ������ 
        isPenetrate = playerSkillDic[skillStateList[0]].isPenetrate;


        // �̵� ����� ��Ÿ��
        Constant.LineType lineType = (Constant.LineType)Enum.Parse(typeof(Constant.LineType), skillMoveList[inx]["PlayerSkillLine"]);
        // ���������� ��ǥ���� ������ ������ ��Ÿ��
        float rate = 0;
        // ��ų�� �´� ��������Ʈ�� ������
        playerSprAnime.SetTrigger(playerSkillDic[skillMoveList[inx]["PlayerSkillCode"]].playerSkillSpriteTrigger);
        // ��ǥ������ ���� ��ǥ�� �ش� ������ true�� ���� ������ ���ŵ��� ����
        isStepUpdate = false;

        // �÷��̾ �̵���Ŵ
        while (true)
        {
            yield return oneFrame;

            moveRate += Time.deltaTime;
            rate = moveRate / moveTime;

            // �̵� ��������� 20~80%������ �÷��̾ ��� Ÿ�ϵ� ���� ���� ������
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
                // ����� ������ŭ �̵��� �Ÿ� ��ǥ ���
                UpdatePosition(MoveCalculating(lineType, end, isWidth, rate), false, dir);
            }
            else
            {
                // ��ǥ ��ġ�� �̵�
                UpdatePosition(end, true, dir);
                // ���� ������ ����
                startPos = playerTrans.position;
                // �ش� ��ų ���� ��������Ʈ ����
                playerSprAnime.SetTrigger("IsEnd");
                // ���� ��ų ���¸� ����Ʈ���� ������
                skillStateList.RemoveAt(0);
                // ���� ��ų ���� �ڷ�ƾ�� �������� �˸�
                isSkillCoroutine = false;
                // �ش� �ڷ�ƾ�� �����Ŵ
                StopCoroutine(moveSkillStateCo);

                yield return null;
                break;
            }
        }

        yield return oneFrame;

        // �ش� �ڷ�ƾ�� �����Ŵ
        StopCoroutine(moveSkillStateCo);

        yield return null;
    }

    // ���� ��ų ���¸� �����Ű�� �ڷ�ƾ
    IEnumerator AttackSkillCoroutine()
    {
        // �ش� �ڷ�ƾ�� ���������� ��Ÿ��
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

    // ������(startPos)���� ��ŭ �̵��ؾ� �ϴ��� ����ϴ� �Լ�
    private Vector3 MoveCalculating(Constant.LineType lineType, Vector3 end, bool isWidth, float rate)
    {
        float isMove = 0f; // �̵��ϴ� ��
        float notMove = 0f;    // �ȿ����̴� ��
        float dist = 0f;    // �̵��ؾ� �� �Ÿ�
        float amplitude = 0f;   // � �̵��� ��� ũ��

        // ���η� �̵��ؾ� �ϴ���, ���η� �̵��ؾ� �ϴ����� �Ǵ���
        // amplitude�� ����. �������� �ϸ��ϰ� �־����� �̵�
        if (isWidth) { dist = end.x; amplitude = 8f; }
        else { dist = end.y; amplitude = 1.5f; }
        
        switch (lineType)
        {
            // ������ ��
            case Constant.LineType.BEELINE:
                // ���� �Ÿ��� ������ ���� �����
                isMove = Mathf.Lerp(0, dist, rate);
                break;

            //��� ��
            case Constant.LineType.CURVE:
                // ���� �Ÿ��� �켱 �����
                isMove = Mathf.Lerp(0, dist, rate);
                // ���� �����̸� ����Ͽ� � ��η� �̵��ϰԲ� ��
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
        // ���������� ������
        startPos = playerTrans.position;
        // �̵��ؾ� �ϴ� ���� �÷��̾� ������Ʈ�� �ʱ�ȭ
        isMoveMap = false;
    }
    // isUpdate�� ���� �̵���ų �� �������� �ٽ� position���� �����־���� �� true�� ������
    private void UpdatePosition(Vector3 move, bool isUpdate, State dir)
    {


        // �÷��̾� ������Ʈ�� �̵����Ѿ� �� ��
        if (playerTrans.localPosition.y < 48 || dir != State.UP)
        {
            // �÷��̾��� ��ġ�� �����Ѵ�.
            playerTrans.position = startPos + move;
        }
        // �� ������Ʈ�� �̵����Ѿ� �� ��
        else
        {
            if (dir == State.UP)
            {
                playerTrans.position = startPos + new Vector3(move.x, 0f);
                // ���� ��ġ�� �����Ѵ�.
                mapM.UpdateMapPosition(move, isUpdate);
            }
        }
    }

    public void UpdateSpriteCount(int cnt)
    {
        // ��� ���� 2d�ؽ����� ��������Ʈ ������ ������ ���� �ߵ�
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
    // �÷��̾��� ���� ��ġ�� ���Ž�Ŵ
    public void UpdatePlayerMana(Constant.ElementType type, int manaAmount)
    {
        // �ش� �Ӽ��� ���ų� ��(��)�Ӽ��� �ƴϸ� ��ȯ
        if (!playerManaDic.ContainsKey(type) && type != Constant.ElementType.ALL) { return; }

        Constant.ElementType t = Constant.ElementType.NONE;

        // ���Ӽ� Ÿ���� ��Ҵٸ� ������ ���¼Ӽ��� ����
        if (type == Constant.ElementType.ALL) 
        {
            t = playerManaDic.ElementAt(UnityEngine.Random.Range(0, playerManaDic.Count)).Key;
        }
        // �׷��� �ʴٸ� Ÿ�Ͽ� �´� ���¼Ӽ� ã��
        else
        {
            t = type;
            //Debug.Log(t);
        }

        // �ش� �Ӽ��� ���� ��ġ�� 999�� �Ѿ�� 999�� ����
        if (playerManaDic[t] + manaAmount > 999)
        {
            playerManaDic[t] = 999;
        }
        // �׷��� �ʴٸ� ��ġ��ŭ ������
        else
        {
            playerManaDic[t] += manaAmount;
        }

        // ManaUI�� ��ġ�� ������
        manaUIScr.UpdateManaAmountText(t, playerManaDic[t]);
    }
}
