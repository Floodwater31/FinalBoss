using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public TextMeshProUGUI skillNameText;     // Assign in the Inspector
    public TextMeshProUGUI cooldownText;     // Assign in the Inspector
    public TextMeshProUGUI descriptionText;  // Assign in the Inspector
    public TextMeshProUGUI levelText;        // Assign in the Inspector

    // Method to initialize the UI elements with skill data
    public void Setup(Skill skill)
    {
        if (skill != null)
        {
            skillNameText.text = skill.skillName;
            cooldownText.text = $"CD: {skill.currentCooldown}/{skill.maxCooldown}";
            descriptionText.text = skill.description;
            levelText.text = $"Lvl: {skill.level}";
        }
        else
        {
            Debug.LogError("No skill provided for SkillUI setup!");
        }
    }

    // Dynamically update cooldown during gameplay
    public void UpdateCooldown(Skill skill)
    {
        if (skill != null)
        {
            cooldownText.text = $"CD: {skill.currentCooldown}/{skill.maxCooldown}";
        }
    }

    // Dynamically update the level during gameplay
    public void UpdateLevel(int level)
    {
        levelText.text = $"Lvl: {level}";
    }
}