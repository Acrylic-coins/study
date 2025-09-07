using System.Collections;
using UnityEngine;
using TMPro;
// 마력 수치가 바뀔 때마다 텍스트에 효과를 주기 위한 스크립트
public class TextEffectManaUI : MonoBehaviour
{
    private ManaUIScripts manaUIScripts;    // 부모 오브젝트로 돌아기기 위해 필요

    private TextMeshProUGUI tmp;    // 자신 오브젝트의 TextMeshPro 컴포넌트
    private RectTransform rectT;    // 자신 오브젝트의 RectTransform 컴포넌트

    private Coroutine effectCo; // 글자에 효과를 주기위한 코루틴

    private GameObject parentObj;   // 부모 오브젝트

    private Color defaultColor = new Color(1f, 1f, 1f, 1f); // 글자의 기본 색깔(알파값때문에 필요)
    private Color minusColor = new Color(0f, 0f, 0f, 10f / 255f);   // 글자의 투명도 감소치

    private float defaultScaleSize = 1f;    // 글자의 기본 스케일 수치

    private void Awake()
    {
        // 캐싱
        tmp = this.GetComponent<TextMeshProUGUI>();
        rectT = this.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // manaUIScripts가 없으면 실행 불가능. 
        if (manaUIScripts != null)
        {
            // 스케일을 기본수치로 변경
            rectT.localScale = new Vector3(defaultScaleSize, defaultScaleSize, defaultScaleSize);
            // 색상을 기본수치로 변경
            tmp.color = defaultColor;
            // 코루틴 실행
            effectCo = StartCoroutine(EffectCoroutine());
        }
    }
    // 부모 오브젝트를 설정(여러 오브젝트의 자식으로 들어가기 때문에 이후 돌아갈 부모 오브젝트를 찾기 위함)
    public void SettingEffect(ManaUIScripts manaUIScripts, GameObject pare)
    {
        this.manaUIScripts = manaUIScripts;
        parentObj = pare;
    }
    // 이펙트 원본 문자 가져옴 
    public void SettingString(string str)
    {
        tmp.text = str;
    }
    // 이펙트 효과를 주는 코루틴
    IEnumerator EffectCoroutine()
    {
        // 스케일 수치
        float sca = defaultScaleSize;
        // 이펙트가 완전히 투명해지면 루프 종료
        while(tmp.color.a > 0f)
        {
            // 점차 스케일을 키움
            sca += 0.02f;
            // 스케일 적용
            rectT.localScale = new Vector3(sca, sca, sca);
            // 동시에 이펙트의 점차 투명하게 만듦
            tmp.color -= minusColor;
            // 한 프레임 쉼
            yield return null;
        }
        // 효과 종료 후 기존 부모 오브젝트로 돌아감
        this.transform.parent = parentObj.transform;
        // 한 프레임 쉼
        yield return null;
        // 이펙트를 비활성화 시킴
        this.gameObject.SetActive(false);
        // 혹시 모르니 코루틴 종료
        StopCoroutine(effectCo);
    }
}
