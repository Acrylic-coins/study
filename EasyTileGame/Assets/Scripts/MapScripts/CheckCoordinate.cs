using UnityEngine;

public class CheckCoordinate : MonoBehaviour
{
    public int coordX { get; private set; }
    public int coordY { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SettingCoord(int x, int y)
    {
        float vecX = -100;
        float vecY = -100;

        coordX = x;
        coordY = y;
        Debug.Log(this.gameObject.name + " " + coordX + " " + coordY);
        if (coordX == -1)
        {
            vecX = -100;
        }
        else
        {
            vecX = (x - 5) * Constant.TILESIZE + Constant.TILESIZE * 0.5f;
            this.GetComponent<BoxCollider2D>().size = new Vector2(5f, 200f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -100f);
        }

        if (coordY == -1)
        {
            vecY = 100;
        }
        else
        {
            vecY = (y - 5) * Constant.TILESIZE + Constant.TILESIZE * 0.5f;
            this.GetComponent<BoxCollider2D>().size = new Vector2(200f , 5f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(100f, 0f);
        }

        this.transform.localPosition = new Vector3(vecX, vecY);

    }
}
