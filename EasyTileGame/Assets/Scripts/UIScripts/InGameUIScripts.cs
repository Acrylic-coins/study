using System.Collections;
using UnityEngine;

// 게임을 할 때 나오는 UI를 관리하기 위한 스크립트
public class InGameUIScripts : MonoBehaviour
{
    private CanvasGroup canGroup;

    private Coroutine appearUICo;

    public void Awake()
    {
        canGroup = this.GetComponent<CanvasGroup>();
    }

    public void OnEnable()
    {
        appearUICo = StartCoroutine(AppearUICoroutine());
    }

    IEnumerator AppearUICoroutine()
    {
        while(canGroup.alpha < 1f)
        {
            yield return null;

            canGroup.alpha += 0.03f;

            if (canGroup.alpha >= 1f) { canGroup.alpha = 1f; }
        }
        yield return null;

        StopCoroutine(appearUICo);
    }
}
