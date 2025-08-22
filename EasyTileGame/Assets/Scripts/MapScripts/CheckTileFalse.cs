using UnityEngine;

public class CheckTileFalse : MonoBehaviour
{
    [SerializeField]private Transform falseTrans;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("TILE"))
        {
            collision.transform.parent = falseTrans;
            collision.gameObject.SetActive(false);
        }
    }
}
