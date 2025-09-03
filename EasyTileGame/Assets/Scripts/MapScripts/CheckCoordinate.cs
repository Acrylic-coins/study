using UnityEngine;

public class CheckCoordinate : MonoBehaviour
{
    public int coordX { get; private set; }
    public int coordY { get; private set; }

    public void SettingCoord(int x, int y)
    {
        float vecX = -100;
        float vecY = -100;

        coordX = x;
        coordY = y;

        if (coordX == -1)
        {
            vecX = -100;
        }
        else
        {
            vecX = (x - 5) * Constant.TILESIZE + Constant.TILESIZE * 0.5f;
            this.GetComponent<BoxCollider2D>().size = new Vector2(15.92f, 200f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -100f);
        }

        if (coordY == -1)
        {
            vecY = 100;
        }
        else
        {
            vecY = (y - 5) * Constant.TILESIZE + Constant.TILESIZE * 0.5f;
            this.GetComponent<BoxCollider2D>().size = new Vector2(200f , 15.92f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(100f, 0f);
        }

        this.transform.localPosition = new Vector3(vecX, vecY);

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("TILE"))
        {
            if (coordX == -1)
            {
                collision.GetComponent<TileColiderCheck>().tileCoordY = coordY;
            }
            else if (coordY == -1)
            {
                collision.GetComponent<TileColiderCheck>().tileCoordX = coordX;
            }
        }
    }
}
