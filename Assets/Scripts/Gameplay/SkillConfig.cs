[System.Serializable]
public struct SkillConfig
{
    public SkillType SkillType;
    public int PrimaryValue;
    public int SecondaryValue;
    public string Description;

    public SkillConfig(SkillType type, int primary, int secondary = 0, string desc = "")
    {
        SkillType = type;
        PrimaryValue = primary;
        SecondaryValue = secondary;
        Description = desc;
    }
}

public enum SkillType
{
    HealthBoost,      
    AttackBoost,   
    DefenseBoost,         
    OpponentAttackDebuff, 
    OpponentDefenseDebuff, 
    Shield,               
    ShieldPenalty       
}
