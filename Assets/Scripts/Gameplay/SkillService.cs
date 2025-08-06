using UnityEngine;
using System.Collections.Generic;

public class SkillService : ISkillService
{
    private readonly Dictionary<SkillType, int> playerEffects = new();
    private readonly Dictionary<SkillType, int> aiEffects = new();
    private readonly Dictionary<SkillType, int> nextTurnPlayerEffects = new();
    private readonly Dictionary<SkillType, int> nextTurnAiEffects = new();
    private readonly SkillConfig[] availableSkills;
    private SkillConfig? currentPlayerSkill;
    private SkillConfig? currentAiSkill;

    public SkillService()
    {
        availableSkills = new SkillConfig[]
        {
            new SkillConfig(SkillType.HealthBoost, 30, 0, "Heal +30 HP"),
            new SkillConfig(SkillType.AttackBoost, 20, 0, "Attack +20"),
            new SkillConfig(SkillType.DefenseBoost, 15, 0, "Defense +15"),
            new SkillConfig(SkillType.OpponentAttackDebuff, 15, 0, "Enemy Attack -15"),
            new SkillConfig(SkillType.OpponentDefenseDebuff, 10, 0, "Enemy Defense -10"),
            new SkillConfig(SkillType.Shield, 25, 10, "Shield 25 DMG, Enemy +10 ATK next turn")
        };
    }

    public SkillConfig GetRandomSkill()
    {
        return availableSkills[Random.Range(0, availableSkills.Length)];
    }

    public void ActivateSkill(SkillConfig skill, bool isPlayer)
    {
        var effects = isPlayer ? playerEffects : aiEffects;
        
        effects.Clear();
        
        effects[skill.skillType] = skill.primaryValue;
        
        if (skill.skillType == SkillType.Shield)
        {
            var opponentNextTurnEffects = isPlayer ? nextTurnAiEffects : nextTurnPlayerEffects;
            opponentNextTurnEffects[SkillType.ShieldPenalty] = skill.secondaryValue;
            Debug.Log($"Shield penalty set: {(isPlayer ? "AI" : "Player")} will get +{skill.secondaryValue} attack next turn");
        }
        
        if (isPlayer)
            currentPlayerSkill = skill;
        else
            currentAiSkill = skill;
            
        Debug.Log($"{(isPlayer ? "Player" : "AI")} activated skill: {skill.description}");
    }

    public int GetModifiedAttack(int baseAttack, bool isPlayer)
    {
        var effects = isPlayer ? playerEffects : aiEffects;
        var opponentEffects = isPlayer ? aiEffects : playerEffects;
        
        int modified = baseAttack;
        
        if (effects.ContainsKey(SkillType.AttackBoost))
            modified += effects[SkillType.AttackBoost];
            
        if (opponentEffects.ContainsKey(SkillType.OpponentAttackDebuff))
            modified -= opponentEffects[SkillType.OpponentAttackDebuff];
            
        if (effects.ContainsKey(SkillType.ShieldPenalty))
        {
            modified += effects[SkillType.ShieldPenalty];
            Debug.Log($"{(isPlayer ? "Player" : "AI")} got shield penalty bonus: +{effects[SkillType.ShieldPenalty]} attack");
        }
            
        return Mathf.Max(0, modified);
    }

    public int GetModifiedDefense(int baseDefense, bool isPlayer)
    {
        var effects = isPlayer ? playerEffects : aiEffects;
        var opponentEffects = isPlayer ? aiEffects : playerEffects;
        
        int modified = baseDefense;
        
        if (effects.ContainsKey(SkillType.DefenseBoost))
            modified += effects[SkillType.DefenseBoost];
            
        if (opponentEffects.ContainsKey(SkillType.OpponentDefenseDebuff))
            modified -= opponentEffects[SkillType.OpponentDefenseDebuff];
            
        return Mathf.Max(0, modified);
    }

    public void ProcessHealthEffects(ref int health, ref int damage, bool isPlayer)
    {
        var effects = isPlayer ? playerEffects : aiEffects;
        
        if (effects.ContainsKey(SkillType.HealthBoost))
        {
            health += effects[SkillType.HealthBoost];
            Debug.Log($"{(isPlayer ? "Player" : "AI")} gained {effects[SkillType.HealthBoost]} health");
        }
        
        if (effects.ContainsKey(SkillType.Shield))
        {
            int shieldValue = effects[SkillType.Shield];
            int absoredDamage = Mathf.Min(damage, shieldValue);
            damage -= absoredDamage;
            
            if (absoredDamage > 0)
                Debug.Log($"{(isPlayer ? "Player" : "AI")} shield absorbed {absoredDamage} damage");
        }
    }

    public void ResetTurnEffects()
    {
        playerEffects.Clear();
        aiEffects.Clear();
        
        foreach(var effect in nextTurnPlayerEffects)
        {
            playerEffects[effect.Key] = effect.Value;
            Debug.Log($"Applied pending effect to Player: {effect.Key} = {effect.Value}");
        }
        
        foreach(var effect in nextTurnAiEffects)
        {
            aiEffects[effect.Key] = effect.Value;
            Debug.Log($"Applied pending effect to AI: {effect.Key} = {effect.Value}");
        }
        
        nextTurnPlayerEffects.Clear();
        nextTurnAiEffects.Clear();
        
        currentPlayerSkill = null;
        currentAiSkill = null;
        
        Debug.Log("Turn effects reset and pending effects applied");
    }

    public SkillConfig? GetCurrentSkill(bool isPlayer)
    {
        return isPlayer ? currentPlayerSkill : currentAiSkill;
    }

}
