using System.Collections;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    [SerializeField] GameObject temObj;

    private WaitForSeconds term = new WaitForSeconds(10f);

    private Coroutine resetCo;

    private Transform trans;

    private void Awake()
    {
        resetCo = null;

        trans = this.transform;
    }

    void Start()
    {
        resetCo = StartCoroutine(ResetPo());

    }

    IEnumerator ResetPo()
    {
        while (true)
        {
            if (Mathf.Abs(trans.localPosition.y) > 100f)
            {
                while (this.trans.childCount > 0)
                {
                    this.trans.GetChild(0).parent = temObj.transform;
                }
                this.trans.localPosition = new Vector3(this.trans.localPosition.x, 0f);

                while (temObj.transform.childCount > 0)
                {
                    temObj.transform.GetChild(0).parent = this.trans;
                }
            }

            yield return term;
        }

        yield return null;
    }
}
