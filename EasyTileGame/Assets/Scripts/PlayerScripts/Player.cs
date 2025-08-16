using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerReadyEffectObj;

    [SerializeField] private SpriteRenderer playerSpr;
    [SerializeField] private BoxCollider2D playerColider;

    [SerializeField] private Texture2D playerTex;

    public bool isPlayerReady { get; private set; } // 플레이어가 움직일 준비가 되었는지 여부(false면 움직일 수 없음)

    private bool isResizeCollidercoEnd = false;
    private Coroutine appearEffectCo;
    private Coroutine resizeColliderCo;
    private Material mar;

    private WaitForSeconds term;
    private void Awake()
    {
        mar = playerSpr.material;

        term = new WaitForSeconds(0.01f);
        playerSpr.material.SetFloat("_MultiSpriteCnt", 21f);
        isPlayerReady = false;
    }

    private void Start()
    {
        appearEffectCo = StartCoroutine(AppearEffect());
    }
    // 등장시 연출을 위한 코루틴
    IEnumerator AppearEffect()
    {
        isResizeCollidercoEnd = false;
        isPlayerReady = false;

        yield return term;
        playerSpr.material.SetFloat("_IsMain", 0f);
        yield return term;
        yield return term;

        int temA = 0;
        int temB = 0;
        int temC = 0;
        float corrK = 0.7f;

        for (int i = 0; i < 35; i++)
        {
            temA = Random.Range(0, 2);
            temB = Random.Range(0, 2);
            temC = Random.Range(0, 2);

            if (temA == 1)
            {
                // 이펙트 스프라이트1 켜기
                playerSpr.material.SetFloat("_IsSub1", (float)temA);
                // 이펙트 스프라이트1 x좌표 갱신
                playerSpr.material.SetFloat("_Sub1X", (float)Random.Range(-110, 110) * 0.01f * corrK);
                // 이펙트 스프라이트1 y좌표 갱신
                playerSpr.material.SetFloat("_Sub1Y", (float)Random.Range(-100, 100) * 0.01f * corrK);
                // 이펙트 스프라이트1 색상 갱신
                playerSpr.material.SetColor("_Sub1Color", new Color((float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f));
            }

            if (temB == 1)
            {
                playerSpr.material.SetFloat("_IsSub2", (float)temB);
                playerSpr.material.SetFloat("_Sub2X", (float)Random.Range(-110, 110) * 0.01f * corrK);
                playerSpr.material.SetFloat("_Sub2Y", (float)Random.Range(-100, 100) * 0.01f * corrK);
                playerSpr.material.SetColor("_Sub2Color", new Color((float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f));
            }

            if (temC == 1)
            {
                playerSpr.material.SetFloat("_IsSub3", (float)temC);
                playerSpr.material.SetFloat("_Sub3X", (float)Random.Range(-110, 110) * 0.01f * corrK);
                playerSpr.material.SetFloat("_Sub3Y", (float)Random.Range(-100, 100) * 0.01f * corrK);
                playerSpr.material.SetColor("_Sub3Color", new Color((float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f, (float)Random.Range(0, 256) / 255f));
            }

            if (i == 10)
            {
                playerSpr.material.SetFloat("_IsMain", 1f);
                playerSpr.material.SetFloat("_IsMainAlpha", 1f);
            }

            if (i == 12)
            {
                playerReadyEffectObj.SetActive(true);

                resizeColliderCo = StartCoroutine(ResizeColider());
            }

            if (i > 9)
            {
                int i2 = i - 9;
                float i3 = (9f - i) * 0.1f;

                corrK = corrK < 0.05f ? 0.05f : corrK - 0.03f;
                playerSpr.material.SetFloat("_IsMainAlpha", i3 <= 1 ? i3 : 1);
                playerSpr.material.SetColor("_MainColorOffset", new Color((float)(i2 * 85f) / 255f, (float)(i2 * 85f) / 255f, (float)(i2 * 85f) / 255f));
            }

            if (i > 15)
            {
                yield return term;
                yield return term;
            }

            yield return term;
            yield return term;
            yield return term;

            playerSpr.material.SetFloat("_IsSub1", 0f);
            playerSpr.material.SetFloat("_IsSub2", 0f);
            playerSpr.material.SetFloat("_IsSub3", 0f);

        }

        playerSpr.material.SetFloat("_IsMainAlpha", 0f);
        playerSpr.material.SetColor("_MainColorOffset", Color.white);

        float rgb = 255f;

        for (int i = 0; i < 60; i++)
        {
            rgb = rgb <= 0 ? 0 : rgb - 4f;
            playerSpr.material.SetColor("_MainColorOffset", new Color(rgb / 255f, rgb / 255f, rgb/ 255f));

            yield return term;

            if (i == 20)
            {
                playerReadyEffectObj.SetActive(false);

                isResizeCollidercoEnd = true;
            }
        }

        playerSpr.material.SetColor("_MainColorOffset", Color.black);

        yield return term;

        isPlayerReady = true;

        StopCoroutine(appearEffectCo);


        yield return null;
    }

    IEnumerator ResizeColider()
    {
        float k = 0;

        while (true)
        {
            k = k < 2f ? k + 0.2f : 2f; 

            for (int i = 0; i < 30 - k; i++)
            {
                playerColider.size = new Vector2((float)(4 + i * (9f + (3f * k))), (float)(4 + i * (9f + (3f * k))));
                yield return term;
            }

            playerColider.size.Set(4f, 4f);

            if (isResizeCollidercoEnd)
            {
                StopCoroutine(resizeColliderCo);
            }
        }

        yield return null;
    }
}
