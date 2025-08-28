using UnityEngine;

// �÷��̾� ��ų Ŭ����
abstract public class PlayerSkill
{
    public Constant.ElementType skillType { get; protected set; }  // ��ų Ÿ��. 8���� �� �ϳ��� ������

    public string skillName { get; protected set; } // ��ų �̸�. ��ųʸ� ���� �� �� 

    public int skillMana { get; protected set; }    // ��ų �Ҹ� ����
}

// ���� ��ų
public class IceSkill : PlayerSkill
{
    public IceSkill()
    {
        skillType = Constant.ElementType.ICE;
    }
}

// �� ��ų
public class FireSkill : PlayerSkill
{
    public FireSkill()
    {
        skillType = Constant.ElementType.FIRE;
    }
}

// ���� ��ų
public class ThunderSkill : PlayerSkill
{
    public ThunderSkill()
    {
        skillType = Constant.ElementType.THUNDER;
    }
}

// �Ĺ� ��ų
public class PlantSkill : PlayerSkill
{
    public PlantSkill()
    {
        skillType = Constant.ElementType.PLANT;
    }
}

// ��ö ��ų
public class MetalSkill : PlayerSkill
{
    public MetalSkill()
    {
        skillType = Constant.ElementType.METAL;
    }
}

// �� ��ų
public class SoilSkill : PlayerSkill
{
    public SoilSkill()
    {
        skillType = Constant.ElementType.SOIL;
    }
}

// ��� ��ų
public class DarkSkill : PlayerSkill
{
    public DarkSkill()
    {
        skillType = Constant.ElementType.DARK;
    }
}

// �� ��ų
public class LightSkill : PlayerSkill
{
    public LightSkill()
    {
        skillType = Constant.ElementType.LIGHT;
    }
}