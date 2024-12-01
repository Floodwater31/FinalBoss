 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public enum GameMode{
    idle,
    shop,
    battle,
    endRound
}
public class FinalBoss : MonoBehaviour
{
    static public FinalBoss S;

    public GameObject uiPrefab; // Prefab of the UI element

    //enemy stuff
    public Transform enemyCanvas;
    private GameObject baseEnemy;
    public List<BaseEntity> enemyList = new List<BaseEntity>();

    // skill stuff
    public SkillDatabase skillDatabase; // Assign the SkillDatabase in the Inspector
    public Transform skillContent; // Assign the content holder for skills in the Inspector
    public GameObject skillPrefab;     // Skill UI prefab for display
    public List<Skill> activeSkills = new List<Skill>();

    //shop stuff
    public List<Button> shopButtons; // Assign shop buttons in the Inspector
    public List<TextMeshProUGUI> shopButtonTexts; // Assign corresponding text components for buttons
    public List<TextMeshProUGUI> shopButtonNames;
    public List<TextMeshProUGUI> shopButtonCDs;

    public GameObject startRoundButton; // Assign in the Inspector

    
    [Header("Inscribed")]
    public TextMeshProUGUI  GoldUI;
    public TextMeshProUGUI  HealthUI;
    public TextMeshProUGUI  RoundUI;

    [Header("Dynamic")]
    public int              gold;
    public int              maxBossHealth;
    public int              currentBossHealth;
    public int              round;
    public GameMode         mode = GameMode.idle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        S = this;

        round = 0;
        gold = 0;
        maxBossHealth = 100;

        activeSkills.Clear();
        // Start the game with 1 random skill

        NewRound();
    }

void AddOrLevelUpSkill(string skillName)
{
    // Find the skill in the database
    Skill skillFromDatabase = skillDatabase.skills.FirstOrDefault(skill => skill.skillName == skillName);

    if (skillFromDatabase == null)
    {
        Debug.LogError($"Skill '{skillName}' not found in the SkillDatabase!");
        return;
    }

    // Check if the skill is already in the activeSkills list
    Skill existingSkill = activeSkills.FirstOrDefault(skill => skill.skillName == skillName);

    if (existingSkill != null)
    {
        // Increase the skill level
        existingSkill.level++;
        Debug.Log($"Skill '{skillName}' leveled up to {existingSkill.level}!");

        // Update SkillUI to reflect the new level
        if (existingSkill.skillUIInstance != null)
        {
            SkillUI skillUI = existingSkill.skillUIInstance.GetComponent<SkillUI>();
            if (skillUI != null)
            {
                skillUI.UpdateLevel(existingSkill.level);
            }
        }
    }
    else
    {
        // Clone the skill and add it to activeSkills
        Skill clonedSkill = skillFromDatabase.Clone();
        activeSkills.Add(clonedSkill);

        // Display the new skill in the UI
        DisplaySkill(clonedSkill);

        Debug.Log($"Skill '{skillName}' added to active skills.");
    }
}

    void AddRandomSkillToPlayer()
{
    if (skillDatabase != null && skillDatabase.skills.Count > 0)
    {
        // Select a random skill from the database
        int randomIndex = Random.Range(0, skillDatabase.skills.Count);
        Skill randomSkill = skillDatabase.skills[randomIndex];

        if (randomSkill != null)
        {
            AddOrLevelUpSkill(randomSkill.skillName);
            Debug.Log($"Random skill added or leveled up: {randomSkill.skillName}");
        }
    }
    else
    {
        Debug.LogError("SkillDatabase is empty or not assigned!");
    }
}

    void DisplaySkill(Skill skill) {
        // Instantiate the SkillUI prefab
        GameObject skillUIObject = Instantiate(skillPrefab, skillContent);

        // Link the SkillUI instance to the skill
        skill.skillUIInstance = skillUIObject;

        // Set up the UI
        SkillUI skillUIComponent = skillUIObject.GetComponent<SkillUI>();

        if (skillUIComponent != null) {
            skillUIComponent.Setup(skill);
        }
    }

    void NewRound()
{
    round += 1;
    gold += 50;
    currentBossHealth = maxBossHealth;

    instantiateEnemies(round);

    foreach (Skill skill in activeSkills)
    {
        skill.currentCooldown = skill.maxCooldown; // Reset cooldown after use
    }

    UpdateGUI();
    AssignSkillsToShopButtons(); // Assign new skills to shop buttons
    StartShop();
}

    void instantiateEnemies(int r) {
        for(int i = 0; i < r; i++) {
            createEnemy(r);
        }
    }

    public void createEnemy(int r) {
        // Instantiate the enemy GameObject
        GameObject newEnemyObject = Instantiate(uiPrefab, enemyCanvas);
        // Get the BaseEntity component
        BaseEntity newEntity = newEnemyObject.GetComponent<BaseEntity>();

        // Ensure the enemy has a BaseEntity component
        if (newEntity != null) {
            newEntity.Setup(r); // Set up enemy stats

            enemyList.Add(newEntity); // Add to the list for management
        } else {
            Debug.LogError("Prefab does not have a BaseEntity component!");
        }
    }

    void UpdateGUI() {
        // Show the data in the GUITexts
        GoldUI.text = "Gold: " + gold;
        HealthUI.text = "Health: " + currentBossHealth + "/" + maxBossHealth;
        RoundUI.text = "Round: " + round;
    }

    void StartShop() {
        mode = GameMode.shop;
        startRoundButton.SetActive(true); // Show the button
        //refresh shop

    }

    public void onStartRoundClick() {
        if (mode == GameMode.shop) {
            mode = GameMode.battle;
            startRoundButton.SetActive(false); // Hide the button
            StartBattle();
        }
    }

    void StartBattle() {
        StartCoroutine(BattleRoutine());
    }

    IEnumerator BattleRoutine() {
        Debug.Log("Battle started!");

        while (currentBossHealth > 0 && enemyList.Count > 0) {
            // Step 1: Activate Skills
            ActivateSkills();

            // Step 2: Enemy Attacks
            foreach (BaseEntity enemy in enemyList.ToList()) {
                if (enemy == null) {
                    // Skip destroyed enemies
                    continue;
                }

                // Perform the attack
                yield return EnemyAttack(enemy);

                // Check if the boss's health is depleted
                if (currentBossHealth <= 0) {
                    Debug.Log("Boss defeated!");
                    break;
                }
            }

            // Remove destroyed enemies
            enemyList.RemoveAll(e => e == null);
        }

        if (currentBossHealth <= 0) {
            Debug.Log("Boss defeated!");
            yield return new WaitForSeconds(2); // Wait for the specified delay
            SceneManager.LoadScene(0); // Replace with your game over scene
        } else {
            Debug.Log("All enemies defeated!");
        }

        // Start a new round
        yield return new WaitForSeconds(1); // Small delay before starting a new round
        NewRound();
    }

    void ActivateSkills() {
        foreach (Skill skill in activeSkills) {
            if (skill.currentCooldown > 0) {
                skill.currentCooldown -= 1; // Reduce cooldown each turn
            }

            // Update SkillUI cooldown
            if (skill.skillUIInstance != null) {
                SkillUI skillUI = skill.skillUIInstance.GetComponent<SkillUI>();
                if (skillUI != null) {
                    skillUI.UpdateCooldown(skill); // Update UI with the current cooldown
                }
            }

            // Activate the skill if cooldown is 0
            if (skill.currentCooldown <= 0) {
                skill.Activate();
                skill.currentCooldown = skill.maxCooldown; // Reset cooldown after use

                // Update SkillUI cooldown again after reset
                if (skill.skillUIInstance != null) {
                    SkillUI skillUI = skill.skillUIInstance.GetComponent<SkillUI>();
                    if (skillUI != null) {
                        skillUI.UpdateCooldown(skill); // Refresh UI for reset cooldown
                    }
                }
            }
        }
    }

    IEnumerator EnemyAttack(BaseEntity enemy) {
        Debug.Log($"{enemy.name} attacks for {enemy.basedamage} damage!");

        // Reduce the boss's health
        currentBossHealth -= enemy.basedamage;

        // Update the GUI
        UpdateGUI();

        // Add a small delay for the attack animation/effect
        yield return new WaitForSeconds(0.5f);
    }


void AssignSkillsToShopButtons()
{
    if (skillDatabase == null || skillDatabase.skills.Count == 0)
    {
        Debug.LogError("SkillDatabase is empty or not assigned!");
        return;
    }

    int buttonCount = shopButtons.Count;

    // Ensure there are enough buttons to assign skills
    if (buttonCount == 0)
    {
        Debug.LogError("No buttons assigned to shopButtons list!");
        return;
    }

    // Assign random skills to buttons
    for (int i = 0; i < buttonCount; i++) // Use the exact count of shop buttons
    {
        if (i >= skillDatabase.skills.Count) 
        {
            Debug.LogWarning("Not enough skills in the database to assign to buttons.");
            break; // Avoid accessing out-of-bound indexes if fewer skills than buttons
        }

        // Reactivate the button if not already disabled
        shopButtons[i].interactable = true;

        // Get a random skill from the skill database
        int randomIndex = Random.Range(0, skillDatabase.skills.Count);
        Skill randomSkill = skillDatabase.skills[randomIndex];

        // Debug log to ensure random skill is picked
        Debug.Log($"Assigning skill '{randomSkill.skillName}' to button {i}");

        // Assign skill to button
        shopButtons[i].onClick.RemoveAllListeners(); // Clear existing listeners
        shopButtons[i].onClick.AddListener(() =>
        {
            // Check if player has enough gold (30 gold required)
            if (gold >= 30)
            {
                // Deduct 30 gold for the purchase
                gold -= 30;
                // Add or level up the skill
                AddOrLevelUpSkill(randomSkill.skillName);
                //shopButtons[i].interactable = false; // Disable button after purchase

                // Debug to confirm button was disabled
                Debug.Log($"Button {i} clicked, skill '{randomSkill.skillName}' bought, 30 gold deducted.");

                // Update the UI with the new gold amount
                UpdateGUI();
            }
            else
            {
                Debug.Log("Not enough gold to buy this skill.");
            }

            // Force canvas update to reflect changes immediately
            Canvas.ForceUpdateCanvases();
        });

        // Update button text with skill info
        if (shopButtonTexts.Count > i && shopButtonTexts[i] != null) // Ensure safe index access
        {
            shopButtonTexts[i].text = $"{randomSkill.description}";
            shopButtonNames[i].text = $"{randomSkill.skillName}";
            shopButtonCDs[i].text = $"CD: {randomSkill.maxCooldown}";
        }
    }
}
}