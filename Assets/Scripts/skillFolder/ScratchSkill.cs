using UnityEngine;

[CreateAssetMenu(fileName = "ScratchSkill", menuName = "Skills/Scratch", order = 3)]
public class ScratchSkill : Skill
{
    public int damage = 5;

    public override void ActionActivate() {
        if (FinalBoss.S.enemyList.Count > 0)
        {
            var target = FinalBoss.S.enemyList[Random.Range(0, FinalBoss.S.enemyList.Count)];
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"{skillName} activated! Dealt {damage} damage to {target.name}.");
            }
        }
        else
        {
            Debug.LogWarning("No enemies available to attack!");
        }

    }
}
