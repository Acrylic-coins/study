using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
public class TilePrefabOrder : ResourceOrder
{
    Dictionary<string, Sprite> sprDic = new Dictionary<string, Sprite>();

    override protected void Awake()
    {
        Constant.OrderForm form = new Constant.OrderForm() { orderName = new List<string>() };

		List<string> str = new List<string>();

		str.Add("MainTileGameObject_None");		
        resourceOrder.Add(new Constant.OrderForm() 
        {
            orderType = Constant.OrderType.SELECT,
            orderCnt = 250,
            resourceType = Constant.ResourceType.GAMEOBJECT,
            resourceParent = this.transform,
            resourceCode = 0,
            orderName = new List<string>()
        });
		str.MoveList<string>(resourceOrder[0].orderName);

		str.Clear();

        str.Add("MainTileSprite_");
        resourceOrder.Add(new Constant.OrderForm()
        {
            orderType = Constant.OrderType.ALL,
            orderCnt = 1,
            resourceType = Constant.ResourceType.SPRITE,
            resourceParent = null,
            resourceCode = 0,
            orderName = new List<string>()
        });
		str.MoveList<string>(resourceOrder[1].orderName);

        str.Clear();
    }

	override public void SetSpriteResource(Sprite spr, int code)
    {

    }
	override public void SetSpriteResource(List<Sprite> spr, int code)
	{
		switch(code)
        {
            case 0:
                for (int i = 0; i < spr.Count; i++)
                {
                    if (!sprDic.ContainsKey(spr[i].name))
                    {
                        sprDic.Add(spr[i].name, spr[i]);
                    }
                }
                break;
        }
	}

	override public Dictionary<string, Sprite> GetSprite()
    {
        return sprDic;
    }
}
