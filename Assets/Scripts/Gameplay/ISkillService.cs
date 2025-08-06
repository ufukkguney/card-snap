using UnityEngine;

public interface ISkillService
{
    SkillConfig GetRandomSkill();
    
    void ActivateSkill(SkillConfig skill, bool isPlayer);
    
    int GetModifiedAttack(int baseAttack, bool isPlayer);
    
    int GetModifiedDefense(int baseDefense, bool isPlayer);
    
    void ProcessHealthEffects(ref int health, ref int damage, bool isPlayer);
    
    void ResetTurnEffects();
    
    SkillConfig? GetCurrentSkill(bool isPlayer);
    
}
