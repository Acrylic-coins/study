using UnityEngine;

// �÷��̾� ��ų Ŭ����
abstract public class PlayerSkill
{
    public Constant.ElementType elementType { get; set; }  // ��ų �Ӽ�. 8���� �� �ϳ��� ������
    public Constant.SkillType skillType { get; set; } // ��ų ���. �̵�, ���� ���� ����

    public string skillName { get; set; } // ��ų �̸�. 
    public string activateKey { get; set; }    // ��ų�� ����ϱ� ���� Ű���� Ű
    public string playerSkillSpriteTrigger { get; set; }    // ��ų ��������Ʈ�� �����ϱ� ���� ���� �̸�

    public int skillMana { get; set; }    // ��ų �Ҹ� ����

    public bool isPenetrate { get; set; } // ��ų�� ������ �ڸ� ���� ���������� �����ϴ��� ����(false�̸� �ƴ�)

    public PlayerSkill()
    {
        isPenetrate = false;
    }
}

// ���� ��ų
public class IceSkill : PlayerSkill
{
    public IceSkill()
    {
        elementType = Constant.ElementType.ICE;
    }
}

// �� ��ų
public class FireSkill : PlayerSkill
{
    public FireSkill()
    {
        elementType = Constant.ElementType.FIRE;
    }
}

// ���� ��ų
public class ThunderSkill : PlayerSkill
{
    public ThunderSkill()
    {
        elementType = Constant.ElementType.THUNDER;
    }
}

// �Ĺ� ��ų
public class PlantSkill : PlayerSkill
{
    public PlantSkill()
    {
        elementType = Constant.ElementType.PLANT;
    }
}

// ��ö ��ų
public class MetalSkill : PlayerSkill
{
    public MetalSkill()
    {
        elementType = Constant.ElementType.METAL;
    }
}

// �� ��ų
public class SoilSkill : PlayerSkill
{
    public SoilSkill()
    {
        elementType = Constant.ElementType.SOIL;
    }
}

// ��� ��ų
public class DarkSkill : PlayerSkill
{
    public DarkSkill()
    {
        elementType = Constant.ElementType.DARK;
    }
}

// �� ��ų
public class LightSkill : PlayerSkill
{
    public LightSkill()
    {
        elementType = Constant.ElementType.LIGHT;
    }
}