using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

// 확장메서드
public static class ExtensionMethod
{
	// 어느 리스트로 값을 전달할 때 참조형식으로 주지 않도록 하기 위함
	public static void MoveList<T>(this List<T> list, List<T> home)
	{
		home.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			home.Add(list[i]);
		}
		
	}
}
