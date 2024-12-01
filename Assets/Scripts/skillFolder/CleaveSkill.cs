using UnityEngine;

[CreateAssetMenu(fileName = "CleaveSkill", menuName = "Skills/Cleave", order = 2)]
public class CleaveSkill : Skill
{
    public int damage = 5;

    public override void ActionActivate()
    {
        if (FinalBoss.S.enemyList.Count > 0)
        {
            foreach (var enemy in FinalBoss.S.enemyList)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log($"{skillName} activated! Dealt {damage} damage to {enemy.name}.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"{skillName} attempted to activate, but no enemies were available.");
        }
    }
}