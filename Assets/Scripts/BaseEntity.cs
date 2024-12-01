using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int basedamage = 1;
    public int currentHealth = 5;
    public TextMeshProUGUI Stats;


    public void Setup(int r) {
        basedamage = r;
        currentHealth = 5*r;
        updateStats();
    }
    public void TakeDamage(int amount) {
        currentHealth -= amount;
        if(currentHealth <= 0){
            Destroy(this.gameObject);
        }
        updateStats();
    }

    public void updateStats() {
        Stats.text = "Health: " + currentHealth + "\nAttack: " + basedamage;
    }

}
