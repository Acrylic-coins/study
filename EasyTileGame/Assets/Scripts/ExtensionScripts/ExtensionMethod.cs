using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

// Ȯ��޼���
public static class ExtensionMethod
{
	// ��� ����Ʈ�� ���� ������ �� ������������ ���� �ʵ��� �ϱ� ����
	public static void MoveList<T>(this List<T> list, List<T> home)
	{
		home.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			home.Add(list[i]);
		}
		
	}
}
