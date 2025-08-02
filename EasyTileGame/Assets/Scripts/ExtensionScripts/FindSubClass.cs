using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

// ����Ŭ������ ã�Ƽ� ��ȯ�ϴ� �Լ�
public static class FindSubClass
{
	// where T : MonoBehaviour�� MonoBehaviour�� ����� Ÿ�Ը� ����Ѵٴ� ��
	public static List<Type> FindSubclassOf<T>() where T : MonoBehaviour
	{
		// ���� ��������� T�� ����� ��� Ÿ�� ã��
		var subclassTypes = Assembly.GetAssembly(typeof(T)).	// T�� ���ǵ� ����� ��ü�� ������. ��) T�� ���ǵ� DLL(.csproj ��) ��ü
			GetTypes().	// �ش� ������� ���ǵ� ��� Ÿ��(Ŭ����, ����ü, �������̽� ��)�� ����
			Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract && t.IsClass).	// ���͸�. ����,��������Ŭ�������� Ȯ��, �߻� Ŭ���� ����, Ŭ������ ����
			ToList();	// ����� List<Type>���� ��ȯ

		// ã�� ����Ŭ������ �ϳ��� ���ٸ� ��� �޽����� ����ϰ� ����. ���� ������ ���� �ڵ�
		if (subclassTypes.Count == 0)
		{
			Debug.LogWarning($"No subclasses of {typeof(T).Name} found.");
			return null;
		}

		return subclassTypes;
	}
}
