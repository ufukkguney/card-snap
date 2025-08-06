using UnityEngine;

public enum SkillType
{
    HealthBoost,           // 1. Increases player's health by A
    AttackBoost,           // 2. Increases attack of played card by B
    DefenseBoost,          // 3. Increases defense of played card by C
    OpponentAttackDebuff,  // 4. Decreases opponent's attack by D
    OpponentDefenseDebuff, // 5. Decreases opponent's defense by E
    Shield,                // 6. Absorbs F damage, but increases opponent's attack by G next turn
    ShieldPenalty          // Next turn attack bonus from opponent's shield
}

[System.Serializable]
public struct SkillConfig
{
    public SkillType skillType;
    public int primaryValue;   // A, B, C, D, E, F values
    public int secondaryValue; // G value (for shield penalty)
    public string description;

    public SkillConfig(SkillType type, int primary, int secondary = 0, string desc = "")
    {
        skillType = type;
        primaryValue = primary;
        secondaryValue = secondary;
        description = desc;
    }
}
