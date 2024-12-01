using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Skills/Skill Database", order = 0)]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> skills; // List of all skills in the database

    // Get a skill by name
    public Skill GetSkillByName(string name)
    {
        foreach (Skill skill in skills)
        {
            if (skill.skillName == name)
            {
                return skill;
            }
        }
        Debug.LogWarning($"Skill with name {name} not found in database!");
        return null;
    }
}
