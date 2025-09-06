using UnityEngine;
using System.Collections;
using System;

// Ÿ�Ͽ� Ư�� �ݶ��̴��� ������ �� �ߵ��ϴ� �Լ��� ���� Ŭ����
public class TileColiderCheck : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightTileSpr;   // ���� �� �����ϴ� Ÿ���� ��������Ʈ������
    [SerializeField] private SpriteRenderer greyTileSpr;    // ���� ���� �� �����ϴ� Ÿ���� ��������Ʈ������

    private Color defaultColor; // ���� �� �⺻ ����
    private Color defaultGreyColor; // ���� ���� �� �⺻ ����

    private WaitForSeconds term = new WaitForSeconds(0.01f);    // �ڷ�ƾ �� ��� ��(0.01��)

    private Coroutine oneLightCo;   // �÷��̾� ���� �� ������ �����ϱ� ���� �ڷ�ƾ
    private Coroutine repeatLightCo;    // �÷��̾� �̵����� Ÿ���� ���� �� ��½���� �����ϴ� �ڷ�ƾ

    private GameObject player;  // �÷��̾� ���� ������Ʈ

    private float oneLightCoCnt = 0f;   // ���� �󿡼� Ÿ���� ��½�� ���⸦ �����ϱ� ���� ����

    private int maxLightCnt = 5;
    public int lightCnt { get; private set; }   // Ÿ���� ��½�� �� �ִ� Ƚ��.(��½�� ������ �÷��̾�� ������ ������)
    public int tileCoordX { get; set; }
    public int tileCoordY { get; set; }

    private bool isLight = false;
    private bool isRepeatCo = false;
    private bool isCheck = false;

    private void Awake()
    {
        defaultColor = lightTileSpr.color;
        defaultGreyColor = greyTileSpr.color;
    }
    // Ÿ���� Ȱ��ȭ�� �� �ߵ�
    private void OnEnable()
    {
        maxLightCnt = 5;
        lightCnt = 5;    
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PLAYER"))
        {
            if (collision.GetComponent<Player>() != null)
            {
                // �÷��̾ ���� ������ �Ұ����� ������ �� �ߵ�
                if (!collision.GetComponent<Player>().isPlayerReady)
                {
                    if (oneLightCo != null)
                    {
                        StopCoroutine(oneLightCo);
                    }

                    oneLightCo = StartCoroutine(OneLight());
                }
                // �÷��̾ ������ ������ ������ �� �ߵ�
                else
                {
                    player = collision.gameObject;

                    if (lightCnt > 0 && !isRepeatCo)
                    {
                        repeatLightCo = StartCoroutine(RepeatLight());

                    }
                    // ���ǿ� ���� �÷��̾��� ������ ä��
                    // Ÿ���� ����Ƚ���� �����ְ� TileAttribute ������Ʈ�� Ÿ�Ͽ� �پ��־�ߵ�
                    if (lightCnt > 0 && this.GetComponent<TileAttribute>() != null) 
                    {
                        // ȸ�����Ѿ� �ϴ� ��ġ
                        int charge = 2;

                        if (lightCnt < 4) { charge = 1; }

                        collision.GetComponent<PlayerMove>().UpdatePlayerMana(this.GetComponent<TileAttribute>().type, charge);
                    }
                }
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("PLAYER"))
        {
            if (repeatLightCo != null)
            {
                isLight = false;

                if (isRepeatCo && !isCheck)
                {
                    lightCnt += -1;
                }
            }
        }
    }

    // �÷��̾� ù ���� ��, ������ ���� Ÿ�� ��½�� �ڷ�ƾ
    IEnumerator OneLight()
    {
        // �����ϰ� ����
        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);
        // �巯���� ����
        lightTileSpr.color = new Color(lightTileSpr.color.r + oneLightCoCnt * 0.5f, lightTileSpr.color.g + oneLightCoCnt * 0.5f, lightTileSpr.color.b + oneLightCoCnt * 0.5f, 200f / 255f);

        for (int i = 0; i < oneLightCoCnt; i++) { yield return term; }

        yield return term;
        yield return term;
        // �����ϰ� ����
        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);

        oneLightCoCnt += 1.5f;

        StopCoroutine(oneLightCo);
    }
    // �÷��̾� ���� ����, ���� Ÿ���� ��½�̰� �ϱ� ���� �ڷ�ƾ
    IEnumerator RepeatLight()
    {
        isLight = true;
        isRepeatCo = true;
        isCheck = true;

        int cX = 0;
        int cY = 0;

        // �÷��̾ �ش� Ÿ�Ͽ� Ȯ���ϰ� �Դ����� �Ǵ���
        while (true)
        {
            cX = player.GetComponent<PlayerMove>().nowCoordX;
            cY = player.GetComponent<PlayerMove>().nowCoordY;

            if (cX == tileCoordX && cY == tileCoordY)
            {
                break;
            }
            else
            {
                if (!isLight)
                {
                    yield return term;

                    isCheck = false;
                    isRepeatCo = false;
                    
                    StopCoroutine(repeatLightCo);
                }
                
            }
            //Debug.Log(isLight + " " + "tileCoordX : " + tileCoordX + " tileCoordY : " + tileCoordY + " playerX : " + cX + " playerY : " + cY);
            if (cX != -1 && cY != -1 && (cX != tileCoordX || cY != tileCoordY))
            {
                yield return term;

                isCheck = false;
                isRepeatCo = false;

                StopCoroutine(repeatLightCo);
            }

            yield return term;
        }

        isCheck = false;
        lightTileSpr.color = defaultColor;

        float al = 0f;
        float bl = 0f;

        while (true)
        {
            // ���İ��� �÷��� Ÿ�Ͽ� ���� �ö���� ��ó�� ������
            if (isLight && lightCnt > 0)
            {
                if (al < ((float)lightCnt / (float)maxLightCnt) * 255f)
                {
                    al += 5f;
                }
                else
                {
                    al = ((float)lightCnt / (float)maxLightCnt) * 255f;
                }
                // �� �� ��¦�̰� �ϱ� ���ؼ� 255�� �ƴ϶� 200���� ������
                lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, (al / 200f) * 1.5f);

                yield return term;
            }
            // ���İ��� ������ Ÿ���� ��ä������ �Ǵ°�ó�� ������
            else
            {
                if (lightTileSpr.color.a <= 0f) { break; }

                lightTileSpr.color -= new Color(0f, 0f, 0f, 10f);

                if (lightCnt < 1)
                {
                    //greyTileSpr.color -= new Color(3f, 3f, 3f, 0f);
                    bl += -0.05f;
                    greyTileSpr.material.SetFloat("_BloomPlus", bl);
                }

                yield return term;
            }
        }
        yield return term;

        isRepeatCo = false;

        StopCoroutine(repeatLightCo);
    }
}
