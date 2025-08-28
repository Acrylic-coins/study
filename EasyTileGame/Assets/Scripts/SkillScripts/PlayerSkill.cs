using UnityEngine;

// 플레이어 스킬 클래스
abstract public class PlayerSkill
{
    public Constant.ElementType skillType { get; protected set; }  // 스킬 타입. 8종류 중 하나로 결정됨

    public string skillName { get; protected set; } // 스킬 이름. 딕셔너리 만들 때 씀 

    public int skillMana { get; protected set; }    // 스킬 소모 마력
}

// 얼음 스킬
public class IceSkill : PlayerSkill
{
    public IceSkill()
    {
        skillType = Constant.ElementType.ICE;
    }
}

// 불 스킬
public class FireSkill : PlayerSkill
{
    public FireSkill()
    {
        skillType = Constant.ElementType.FIRE;
    }
}

// 번개 스킬
public class ThunderSkill : PlayerSkill
{
    public ThunderSkill()
    {
        skillType = Constant.ElementType.THUNDER;
    }
}

// 식물 스킬
public class PlantSkill : PlayerSkill
{
    public PlantSkill()
    {
        skillType = Constant.ElementType.PLANT;
    }
}

// 강철 스킬
public class MetalSkill : PlayerSkill
{
    public MetalSkill()
    {
        skillType = Constant.ElementType.METAL;
    }
}

// 흙 스킬
public class SoilSkill : PlayerSkill
{
    public SoilSkill()
    {
        skillType = Constant.ElementType.SOIL;
    }
}

// 어둠 스킬
public class DarkSkill : PlayerSkill
{
    public DarkSkill()
    {
        skillType = Constant.ElementType.DARK;
    }
}

// 빛 스킬
public class LightSkill : PlayerSkill
{
    public LightSkill()
    {
        skillType = Constant.ElementType.LIGHT;
    }
}