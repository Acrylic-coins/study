using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSpr;

    private Coroutine appearEffectCo;
    private WaitForSeconds term;
    private void Awake()
    {
        term = new WaitForSeconds(0.01f);
        playerSpr.material.SetFloat("_MultiSpriteCnt", 21f);
    }

    private void Start()
    {
        appearEffectCo = StartCoroutine(AppearEffect());
    }
    // 등장시 연출을 위한 코루틴
    IEnumerator AppearEffect()
    {
        yield return term;
        playerSpr.material.SetFloat("_IsMain", 0f);
        yield return term;
        yield return term;

        int temA = 0;
        int temB = 0;
        int temC = 0;
        float corrK = 1f;

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
        for (int i = 0; i < 40; i++)
        {
            rgb = rgb <= 0 ? 0 : rgb - 4f;
            playerSpr.material.SetColor("_MainColorOffset", new Color(rgb / 255f, rgb / 255f, rgb/ 255f));

            yield return term;
        }

        playerSpr.material.SetColor("_MainColorOffset", Color.black);


        StopCoroutine(appearEffectCo);

        yield return null;
    }

}
