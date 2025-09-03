using UnityEngine;
using System;
using System.Collections.Generic;

public static class Constant
{
	public static readonly int TILESIZE = 16;
	public static readonly int TILEXCOUNT = 10;
	public static readonly int TILEYCOUNT = 10;

	public enum ElementType { NONE = -1, FIRE, SOIL, THUNDER, PLANT, ICE, METAL, DARK, LIGHT, NOTHING, ALL };
	// ���ҽ� �ֹ� ����� ���� ������. SELECT�� orderName�� ��ġ�� �ּҿ� ����, ALL�̸� orderName�� ������ ��� �ּҿ� ���� ������
	public enum OrderType { SELECT = 0, ALL };
	// ��ų ���
	public enum SkillType { MOVE = 0, ATTACK};
	// �̵� ���
	// BEELINE = ����, CURVE = �
	public enum LineType { BEELINE = 0, CURVE = 1};
	// ���ҽ� �ڷ��� ����
	public enum ResourceType { GAMEOBJECT = 0, SPRITE };
	public struct OrderForm
	{
		public List<string> orderName;  // �ֹ��� ���ҽ� �̸�
		public OrderType orderType; // ���ҽ� �ֹ� ���
		public int orderCnt; // �ֹ��� ���ҽ� ����
		public ResourceType resourceType; // �ֹ��� ���ҽ� �ڷ���
		public Transform resourceParent;    // �ֹ��� ���ҽ��� �θ� ������Ʈ�� ã�� ����(�밳 gameObject)
		public int resourceCode;	// �ֹ��� ���ҽ��� ��� ������ �������� ���ϱ� ����
	}
}
