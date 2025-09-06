using UnityEngine;
using System.Collections;
using System;

// 타일에 특정 콜라이더가 감지될 때 발동하는 함수를 담은 클래스
public class TileColiderCheck : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightTileSpr;   // 밟을 때 등장하는 타일의 스프라이트렌더러
    [SerializeField] private SpriteRenderer greyTileSpr;    // 밟지 않을 때 등장하는 타일의 스프라이트렌더러

    private Color defaultColor; // 밟을 때 기본 색상
    private Color defaultGreyColor; // 밟지 않을 때 기본 색상

    private WaitForSeconds term = new WaitForSeconds(0.01f);    // 코루틴 내 대기 텀(0.01초)

    private Coroutine oneLightCo;   // 플레이어 등장 시 연출을 조성하기 위한 코루틴
    private Coroutine repeatLightCo;    // 플레이어 이동으로 타일을 밟을 때 번쩍임을 연출하는 코루틴

    private GameObject player;  // 플레이어 게임 오브젝트

    private float oneLightCoCnt = 0f;   // 연출 상에서 타일의 번쩍임 세기를 조절하기 위한 변수

    private int maxLightCnt = 5;
    public int lightCnt { get; private set; }   // 타일이 번쩍일 수 있는 횟수.(번쩍일 때마다 플레이어는 마력을 충전함)
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
    // 타일이 활성화될 시 발동
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
                // 플레이어가 아직 조작이 불가능한 상태일 때 발동
                if (!collision.GetComponent<Player>().isPlayerReady)
                {
                    if (oneLightCo != null)
                    {
                        StopCoroutine(oneLightCo);
                    }

                    oneLightCo = StartCoroutine(OneLight());
                }
                // 플레이어가 조작이 가능한 상태일 때 발동
                else
                {
                    player = collision.gameObject;

                    if (lightCnt > 0 && !isRepeatCo)
                    {
                        repeatLightCo = StartCoroutine(RepeatLight());

                    }
                    // 조건에 따라 플레이어의 마력을 채움
                    // 타일의 충전횟수가 남아있고 TileAttribute 컴포넌트가 타일에 붙어있어야됨
                    if (lightCnt > 0 && this.GetComponent<TileAttribute>() != null) 
                    {
                        // 회복시켜야 하는 수치
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

    // 플레이어 첫 등장 시, 연출을 위한 타일 번쩍임 코루틴
    IEnumerator OneLight()
    {
        // 투명하게 만듦
        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);
        // 드러나게 만듦
        lightTileSpr.color = new Color(lightTileSpr.color.r + oneLightCoCnt * 0.5f, lightTileSpr.color.g + oneLightCoCnt * 0.5f, lightTileSpr.color.b + oneLightCoCnt * 0.5f, 200f / 255f);

        for (int i = 0; i < oneLightCoCnt; i++) { yield return term; }

        yield return term;
        yield return term;
        // 투명하게 만듦
        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);

        oneLightCoCnt += 1.5f;

        StopCoroutine(oneLightCo);
    }
    // 플레이어 등장 이후, 밟은 타일을 번쩍이게 하기 위한 코루틴
    IEnumerator RepeatLight()
    {
        isLight = true;
        isRepeatCo = true;
        isCheck = true;

        int cX = 0;
        int cY = 0;

        // 플레이어가 해당 타일에 확실하게 왔는지를 판단함
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
            // 알파값을 올려서 타일에 색이 올라오는 것처럼 연출함
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
                // 좀 더 반짝이게 하기 위해서 255가 아니라 200으로 나눴음
                lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, (al / 200f) * 1.5f);

                yield return term;
            }
            // 알파값을 내려서 타일이 무채색으로 되는것처럼 연출함
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
