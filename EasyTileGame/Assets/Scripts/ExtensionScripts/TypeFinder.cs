using System;   // Type, Activator, AppDomain 등 기본 리플렉션/런타임 API 사용
using System.Linq;  // FirstOrDefault, SelectMany, Where 같은 LINQ 확장 메서드 사용
using System.Collections.Generic;   // Dictionary 등 컬렉션 사용
using System.Reflection;    // Assembly, ReflectionTypeLoadException 등 리플렉션 타입 사용

// 특정 이름을 가진 클래스를 가져오기 위함
// 인스턴스가 필요 없는 정적 유틸리티 클래스
public static class TypeFinder
{
    // 캐시
    // (기저타입, 찾을이름) 튜플을 키로, 실제로 찾은 Type을 값으로 보관
    // 동일한 조건으로 다시 찾을 때 리플렉션 비용 0
    static readonly Dictionary<(Type, string), Type> cache = new();

    /// nameOrFullName: "클래스명" 또는 "네임스페이스.클래스명"
    // 제네릭으로 기저 타입을 지정. 결과는 TBase를 상속(또는 구현)하는 타입만 허용
    // includeAbstract = false 면 추상 타입은 제외(인스턴스화 못 하니까 보통 제외하는 게 안전)
    public static Type FindDerivedType<TBase>(string nameOrFullName, bool includeAbstract = false)
    {
        // 캐시 키 생성 : TBase의 System.Type과 검색 문자열 조합
        var key = (typeof(TBase), nameOrFullName);
        // 캐시에 있으면 즉시 반환(리플렉션 탐색 생략)
        if (cache.TryGetValue(key, out var hit)) return hit;

        // 1) 풀네임 정확 매치 우선
        // 현재 AppDomain에 로드된 모든 어셈블리를 순회
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            // "네임스페이스.클래스명" 풀네임으로 바로 조회. 없으면 null만 받게 throwOnError:false
            var t = asm.GetType(nameOrFullName, throwOnError: false);
            // 조건 1: 타입이 존재
            // 조건 2: TBase <- t (t가 TBase를 상속/구현)
            // 조건 3: 추상 타입 허용 여부 체크
            if (t != null && typeof(TBase).IsAssignableFrom(t) &&
               (includeAbstract || !t.IsAbstract))
                // 찾은 즉시 캐시에 넣고 반환
                return cache[key] = t;
        }

        // 2) 짧은 이름 매치 (동명이인 주의)
        var found = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => SafeGetTypes(a))
            // 각 어셈블리의 모든 타입을 펼침 (예외 안전 버전으로)
            .FirstOrDefault(t =>
            // 조건 1: TBase 파생
            // 조건 2: 추상 허용 여부
            // 조건 3: "짧은 이름" 일치 (네임스페이스 제외된 Name 비교)
                typeof(TBase).IsAssignableFrom(t) &&
               (includeAbstract || !t.IsAbstract) &&
                t.Name == nameOrFullName);

        // 찾으면 캐시에 저장
        if (found != null) cache[key] = found;
        // 못 찾으면 null 반환
        return found;
    }

    static IEnumerable<Type> SafeGetTypes(Assembly a)
    {
        // 어셈블리의 모든 타입 나열. 보통 OK.
        try { return a.GetTypes(); }
        // 일부 타입 로드 실패 시 던지는 예외
        // e.Types에는 실패한 슬롯이 null로 들어갈 수 있으니 null 제거하고 반환
        // 마지막의 !는 c# null-forgiving 연산자(분석기 경고만 억제)
        catch (ReflectionTypeLoadException e) { return e.Types.Where(x => x != null)!; }
    }
}
