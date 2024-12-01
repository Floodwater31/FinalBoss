using UnityEngine;

[CreateAssetMenu(fileName = "RampageSkill", menuName = "Skills/Rampage", order = 4)]
public class RampageSkill : Skill
{
    public int damage = 20;

    public override void ActionActivate() {
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
            Debug.LogWarning("No enemies available to attack!");
        }
    }
}
