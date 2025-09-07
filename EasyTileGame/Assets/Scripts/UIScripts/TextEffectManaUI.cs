using System.Collections;
using UnityEngine;
using TMPro;
// ���� ��ġ�� �ٲ� ������ �ؽ�Ʈ�� ȿ���� �ֱ� ���� ��ũ��Ʈ
public class TextEffectManaUI : MonoBehaviour
{
    private ManaUIScripts manaUIScripts;    // �θ� ������Ʈ�� ���Ʊ�� ���� �ʿ�

    private TextMeshProUGUI tmp;    // �ڽ� ������Ʈ�� TextMeshPro ������Ʈ
    private RectTransform rectT;    // �ڽ� ������Ʈ�� RectTransform ������Ʈ

    private Coroutine effectCo; // ���ڿ� ȿ���� �ֱ����� �ڷ�ƾ

    private GameObject parentObj;   // �θ� ������Ʈ

    private Color defaultColor = new Color(1f, 1f, 1f, 1f); // ������ �⺻ ����(���İ������� �ʿ�)
    private Color minusColor = new Color(0f, 0f, 0f, 10f / 255f);   // ������ ���� ����ġ

    private float defaultScaleSize = 1f;    // ������ �⺻ ������ ��ġ

    private void Awake()
    {
        // ĳ��
        tmp = this.GetComponent<TextMeshProUGUI>();
        rectT = this.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // manaUIScripts�� ������ ���� �Ұ���. 
        if (manaUIScripts != null)
        {
            // �������� �⺻��ġ�� ����
            rectT.localScale = new Vector3(defaultScaleSize, defaultScaleSize, defaultScaleSize);
            // ������ �⺻��ġ�� ����
            tmp.color = defaultColor;
            // �ڷ�ƾ ����
            effectCo = StartCoroutine(EffectCoroutine());
        }
    }
    // �θ� ������Ʈ�� ����(���� ������Ʈ�� �ڽ����� ���� ������ ���� ���ư� �θ� ������Ʈ�� ã�� ����)
    public void SettingEffect(ManaUIScripts manaUIScripts, GameObject pare)
    {
        this.manaUIScripts = manaUIScripts;
        parentObj = pare;
    }
    // ����Ʈ ���� ���� ������ 
    public void SettingString(string str)
    {
        tmp.text = str;
    }
    // ����Ʈ ȿ���� �ִ� �ڷ�ƾ
    IEnumerator EffectCoroutine()
    {
        // ������ ��ġ
        float sca = defaultScaleSize;
        // ����Ʈ�� ������ ���������� ���� ����
        while(tmp.color.a > 0f)
        {
            // ���� �������� Ű��
            sca += 0.02f;
            // ������ ����
            rectT.localScale = new Vector3(sca, sca, sca);
            // ���ÿ� ����Ʈ�� ���� �����ϰ� ����
            tmp.color -= minusColor;
            // �� ������ ��
            yield return null;
        }
        // ȿ�� ���� �� ���� �θ� ������Ʈ�� ���ư�
        this.transform.parent = parentObj.transform;
        // �� ������ ��
        yield return null;
        // ����Ʈ�� ��Ȱ��ȭ ��Ŵ
        this.gameObject.SetActive(false);
        // Ȥ�� �𸣴� �ڷ�ƾ ����
        StopCoroutine(effectCo);
    }
}
