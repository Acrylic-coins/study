using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

// 서브클래스를 찾아서 반환하는 함수
public static class FindSubClass
{
	// where T : MonoBehaviour는 MonoBehaviour를 상속한 타입만 허용한다는 뜻
	public static List<Type> FindSubclassOf<T>() where T : MonoBehaviour
	{
		// 현재 어셈블리에서 T를 상속한 모든 타입 찾기
		var subclassTypes = Assembly.GetAssembly(typeof(T)).	// T가 정의된 어셈블리 전체를 가져옴. 예) T가 정의된 DLL(.csproj 등) 전체
			GetTypes().	// 해당 어셈블리에 정의된 모든 타입(클래스, 구조체, 인터페이스 등)을 나열
			Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract && t.IsClass).	// 필터링. 직접,간접서브클래스인지 확인, 추상 클래스 제외, 클래스만 포함
			ToList();	// 결과를 List<Type>으로 변환

		// 찾은 서브클래스가 하나도 없다면 경고 메시지를 출력하고 종료. 예외 방지용 안전 코드
		if (subclassTypes.Count == 0)
		{
			Debug.LogWarning($"No subclasses of {typeof(T).Name} found.");
			return null;
		}

		return subclassTypes;
	}
}
