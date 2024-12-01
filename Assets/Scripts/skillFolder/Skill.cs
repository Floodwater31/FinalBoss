using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill", order = 1)]
public class Skill : ScriptableObject
{
    public string skillName;         // Name of the skill
    public string description;       // Description of the skill
    public int maxCooldown;          // Maximum cooldown time
    public int currentCooldown;      // Current cooldown (0 means ready to use)
    public int level = 1;            // Default level

    public GameObject skillUIInstance; // Reference to the SkillUI GameObject

    public void Activate()
    {
        // Activate the skill if cooldown is ready
        if (currentCooldown <= 0)
        {
            for (int i = 0; i < level; i++)
            {
                ActionActivate(); // Call the overridden activation method
            }
            currentCooldown = maxCooldown; // Reset cooldown after activation
        }
        else
        {
            Debug.Log($"{skillName} is on cooldown. Current cooldown: {currentCooldown}");
        }
    }

    // This method will be overridden by specific skills
    public virtual void ActionActivate()
    {
        Debug.LogWarning($"{skillName} has no specific ActionActivate implementation!");
    }

    // Clone method for creating a new instance of the skill
    public Skill Clone()
    {
        Skill clonedSkill = Instantiate(this); // Clone the ScriptableObject
        clonedSkill.skillUIInstance = null;    // Reset UI reference
        clonedSkill.currentCooldown = 0;      // Ensure cooldown starts at 0 for a fresh clone
        return clonedSkill;
    }
}