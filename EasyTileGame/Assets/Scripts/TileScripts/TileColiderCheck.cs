using UnityEngine;
using System.Collections;
using System;

public class TileColiderCheck : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightTileSpr;

    private WaitForSeconds term = new WaitForSeconds(0.01f);

    private Coroutine oneLightCo;

    private float oneLightCoCnt = 0f;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PLAYER"))
        {
            if (collision.GetComponent<Player>() != null)
            {
                if (!collision.GetComponent<Player>().isPlayerReady)
                {
                    if (oneLightCo != null)
                    {
                        StopCoroutine(oneLightCo);
                    }

                    oneLightCo = StartCoroutine(OneLight());
                }
            }
        }
    }

    IEnumerator OneLight()
    {
        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);

        lightTileSpr.color = new Color(lightTileSpr.color.r + oneLightCoCnt * 0.5f, lightTileSpr.color.g + oneLightCoCnt * 0.5f, lightTileSpr.color.b + oneLightCoCnt * 0.5f, 200f / 255f);

        for (int i = 0; i < oneLightCoCnt; i++) { yield return term; }

        yield return term;
        yield return term;

        lightTileSpr.color = new Color(lightTileSpr.color.r, lightTileSpr.color.g, lightTileSpr.color.b, 0f);

        oneLightCoCnt += 1.5f;

        StopCoroutine(oneLightCo);

        yield return null;
    }
}
