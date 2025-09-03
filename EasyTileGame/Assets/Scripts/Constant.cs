using UnityEngine;
using System;
using System.Collections.Generic;

public static class Constant
{
	public static readonly int TILESIZE = 16;
	public static readonly int TILEXCOUNT = 10;
	public static readonly int TILEYCOUNT = 10;

	public enum ElementType { NONE = -1, FIRE, SOIL, THUNDER, PLANT, ICE, METAL, DARK, LIGHT, NOTHING, ALL };
	// 리소스 주문 방식을 위한 열거형. SELECT면 orderName과 일치한 주소에 가고, ALL이면 orderName을 포함한 모든 주소에 가서 가져옴
	public enum OrderType { SELECT = 0, ALL };
	// 스킬 방식
	public enum SkillType { MOVE = 0, ATTACK};
	// 이동 방식
	// BEELINE = 직선, CURVE = 곡선
	public enum LineType { BEELINE = 0, CURVE = 1};
	// 리소스 자료형 종류
	public enum ResourceType { GAMEOBJECT = 0, SPRITE };
	public struct OrderForm
	{
		public List<string> orderName;  // 주문할 리소스 이름
		public OrderType orderType; // 리소스 주문 방식
		public int orderCnt; // 주문할 리소스 수량
		public ResourceType resourceType; // 주문할 리소스 자료형
		public Transform resourceParent;    // 주문한 리소스의 부모 오브젝트를 찾기 위함(대개 gameObject)
		public int resourceCode;	// 주문한 리소스를 어느 변수에 저장할지 정하기 위함
	}
}
