using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

// ���� �ʿ��� ���ҽ��� �ֹ��ϱ� ���� ��ũ��Ʈ
abstract public class ResourceOrder : MonoBehaviour
{

	// �ʿ��� ���ҽ��� addressable �ּҸ� ���� ����Ʈ
	protected List<Constant.OrderForm> resourceOrder = new List<Constant.OrderForm>();

	abstract protected void Awake();
	// ��������Ʈ�� �������� ���� �Լ�
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
