using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

// 각자 필요한 리소스를 주문하기 위한 스크립트
abstract public class ResourceOrder : MonoBehaviour
{

	// 필요한 리소스의 addressable 주소를 담은 리스트
	protected List<Constant.OrderForm> resourceOrder = new List<Constant.OrderForm>();

	abstract protected void Awake();
	// 스프라이트를 가져오기 위한 함수
	abstract public void SetSpriteResource(Sprite spr, int code);
	abstract public void SetSpriteResource(List<Sprite> spr, int code);

	public List<Constant.OrderForm> GetOrderForm()
	{
		return resourceOrder;
	}
	virtual public Dictionary<string,Sprite> GetSprite()
	{
		return null;
	}
	
}
