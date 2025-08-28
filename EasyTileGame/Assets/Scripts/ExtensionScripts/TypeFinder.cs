using System;   // Type, Activator, AppDomain �� �⺻ ���÷���/��Ÿ�� API ���
using System.Linq;  // FirstOrDefault, SelectMany, Where ���� LINQ Ȯ�� �޼��� ���
using System.Collections.Generic;   // Dictionary �� �÷��� ���
using System.Reflection;    // Assembly, ReflectionTypeLoadException �� ���÷��� Ÿ�� ���

// Ư�� �̸��� ���� Ŭ������ �������� ����
// �ν��Ͻ��� �ʿ� ���� ���� ��ƿ��Ƽ Ŭ����
public static class TypeFinder
{
    // ĳ��
    // (����Ÿ��, ã���̸�) Ʃ���� Ű��, ������ ã�� Type�� ������ ����
    // ������ �������� �ٽ� ã�� �� ���÷��� ��� 0
    static readonly Dictionary<(Type, string), Type> cache = new();

    /// nameOrFullName: "Ŭ������" �Ǵ� "���ӽ����̽�.Ŭ������"
    // ���׸����� ���� Ÿ���� ����. ����� TBase�� ���(�Ǵ� ����)�ϴ� Ÿ�Ը� ���
    // includeAbstract = false �� �߻� Ÿ���� ����(�ν��Ͻ�ȭ �� �ϴϱ� ���� �����ϴ� �� ����)
    public static Type FindDerivedType<TBase>(string nameOrFullName, bool includeAbstract = false)
    {
        // ĳ�� Ű ���� : TBase�� System.Type�� �˻� ���ڿ� ����
        var key = (typeof(TBase), nameOrFullName);
        // ĳ�ÿ� ������ ��� ��ȯ(���÷��� Ž�� ����)
        if (cache.TryGetValue(key, out var hit)) return hit;

        // 1) Ǯ���� ��Ȯ ��ġ �켱
        // ���� AppDomain�� �ε�� ��� ������� ��ȸ
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            // "���ӽ����̽�.Ŭ������" Ǯ�������� �ٷ� ��ȸ. ������ null�� �ް� throwOnError:false
            var t = asm.GetType(nameOrFullName, throwOnError: false);
            // ���� 1: Ÿ���� ����
            // ���� 2: TBase <- t (t�� TBase�� ���/����)
            // ���� 3: �߻� Ÿ�� ��� ���� üũ
            if (t != null && typeof(TBase).IsAssignableFrom(t) &&
               (includeAbstract || !t.IsAbstract))
                // ã�� ��� ĳ�ÿ� �ְ� ��ȯ
                return cache[key] = t;
        }

        // 2) ª�� �̸� ��ġ (�������� ����)
        var found = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => SafeGetTypes(a))
            // �� ������� ��� Ÿ���� ��ħ (���� ���� ��������)
            .FirstOrDefault(t =>
            // ���� 1: TBase �Ļ�
            // ���� 2: �߻� ��� ����
            // ���� 3: "ª�� �̸�" ��ġ (���ӽ����̽� ���ܵ� Name ��)
                typeof(TBase).IsAssignableFrom(t) &&
               (includeAbstract || !t.IsAbstract) &&
                t.Name == nameOrFullName);

        // ã���� ĳ�ÿ� ����
        if (found != null) cache[key] = found;
        // �� ã���� null ��ȯ
        return found;
    }

    static IEnumerable<Type> SafeGetTypes(Assembly a)
    {
        // ������� ��� Ÿ�� ����. ���� OK.
        try { return a.GetTypes(); }
        // �Ϻ� Ÿ�� �ε� ���� �� ������ ����
        // e.Types���� ������ ������ null�� �� �� ������ null �����ϰ� ��ȯ
        // �������� !�� c# null-forgiving ������(�м��� ��� ����)
        catch (ReflectionTypeLoadException e) { return e.Types.Where(x => x != null)!; }
    }
}
