using UnityEngine;

// 플레이어 스킬 클래스
abstract public class PlayerSkill
{
    public Constant.ElementType elementType { get; set; }  // 스킬 속성. 8종류 중 하나로 결정됨
    public Constant.SkillType skillType { get; set; } // 스킬 방식. 이동, 공격 등이 있음

    public string skillName { get; set; } // 스킬 이름. 
    public string activateKey { get; set; }    // 스킬을 사용하기 위한 키보드 키
    public string playerSkillSpriteTrigger { get; set; }    // 스킬 스프라이트를 변경하기 위한 변수 이름

    public int skillMana { get; set; }    // 스킬 소모 마력

    public bool isPenetrate { get; set; } // 스킬로 지나간 자리 전부 밟은것으로 간주하는지 여부(false이면 아님)

    public PlayerSkill()
    {
        isPenetrate = false;
    }
}

// 얼음 스킬
public class IceSkill : PlayerSkill
{
    public IceSkill()
    {
        elementType = Constant.ElementType.ICE;
    }
}

// 불 스킬
public class FireSkill : PlayerSkill
{
    public FireSkill()
    {
        elementType = Constant.ElementType.FIRE;
    }
}

// 번개 스킬
public class ThunderSkill : PlayerSkill
{
    public ThunderSkill()
    {
        elementType = Constant.ElementType.THUNDER;
    }
}

// 식물 스킬
public class PlantSkill : PlayerSkill
{
    public PlantSkill()
    {
        elementType = Constant.ElementType.PLANT;
    }
}

// 강철 스킬
public class MetalSkill : PlayerSkill
{
    public MetalSkill()
    {
        elementType = Constant.ElementType.METAL;
    }
}

// 흙 스킬
public class SoilSkill : PlayerSkill
{
    public SoilSkill()
    {
        elementType = Constant.ElementType.SOIL;
    }
}

// 어둠 스킬
public class DarkSkill : PlayerSkill
{
    public DarkSkill()
    {
        elementType = Constant.ElementType.DARK;
    }
}

// 빛 스킬
public class LightSkill : PlayerSkill
{
    public LightSkill()
    {
        elementType = Constant.ElementType.LIGHT;
    }
}