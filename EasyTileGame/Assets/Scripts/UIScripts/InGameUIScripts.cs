using System.Collections;
using UnityEngine;

// ������ �� �� ������ UI�� �����ϱ� ���� ��ũ��Ʈ
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
