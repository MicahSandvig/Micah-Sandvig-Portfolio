using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for Button class
using UnityEngine.EventSystems; // Add this for ExecuteEvents

public class PlayerStats : MonoBehaviour
{

    public CharacterScriptableObject characterData;

    public Sprite TreeMeleeCharacter;


    //current stats
    public float currentHealth;
    public float currentRecovery;
    public float currentMoveSpeed;
    public float currentMight;
    public float currentProjectileSpeed;
    public float currentMaxHealth;

    public HealthBar healthBar;
    public XPBar xpBar;

    // Reference to your button
    private Button levelUpPineButton;

    public int experience = 0;
    public int level = 1;
    public int experienceCap = 100;
    public int experienceCapIncrease;

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;


    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    public GameObject secondWeaponTest;
    public GameObject firstPassiveItemTest, secondPassiveItemTest;

    void Update()
    {


        LevelUpChecker();


        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();

        healthBar.SetMaxHealth(currentMaxHealth);
        healthBar.SetHealth(currentHealth);

        xpBar.SetMaxXP(experienceCap);
        xpBar.SetXP(experience);
    }

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();


        inventory = GetComponent<InventoryManager>();

        //Assigns stats
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMaxHealth = characterData.MaxHealth;

        //Select appropiate sprite
        if (currentHealth >= 1199)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = TreeMeleeCharacter;
        }
        //Spawn the starter weapon
        SpawnWeapon(characterData.StartingWeapon);


        healthBar.SetMaxHealth(currentMaxHealth);
        healthBar.SetHealth(currentHealth);

        // Try to find the button early
        FindLevelUpPineButton();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
    }


    void LevelUpChecker()
    {

        if (experience >= experienceCap) //Increases level if experience is greater than or equal to XP required to level up
        {

            //Levels up the player and deducts experience
            level++;
            experience -= experienceCap;
            experienceCap += experienceCapIncrease;

            // Check for SpinningWeaponDamage component before trying to level it up
            SpinningWeaponDamage weaponInstance = FindObjectOfType<SpinningWeaponDamage>();
            if (weaponInstance != null)
            {
                weaponInstance.lvlup(); // Only call lvlup if the weapon exists
            }

            // Check for SpinningWeapon component before trying to speed it up
            SpinningWeapon spinSpeed = FindObjectOfType<SpinningWeapon>();
            if (spinSpeed != null)
            {
                spinSpeed.speedup(); // Only call speedup if the weapon exists
            }

            LevelUpPineWeapon();
        }
    }

    public void TakeDamage(float dmg)
    {
        //if player isn't invincible reduce their health and start I-Frames
        if (!isInvincible)
        {
            currentHealth -= dmg;

            healthBar.SetHealth(currentHealth);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                kill();
            }

        }
    }


    public void kill()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.GameOver();
        Debug.Log("PLAYER DEAD");
    }

    public void healOnEnemyDeath(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > characterData.MaxHealth)
        {
            currentHealth = characterData.MaxHealth;
        }
    }

    void Recover()
    {
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;

            //Makes sure player health doesnt go above maximum
            if (currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        try
        {
            // Check if essential components exist
            if (weapon == null)
            {
                Debug.LogWarning("Attempted to spawn null weapon");
                return;
            }

            if (inventory == null)
            {
                Debug.LogWarning("Inventory component is null");
                return;
            }

            // Original implementation - wrapped in try-catch to prevent script disabling
            // checks if slots are full
            if (weaponIndex >= inventory.weaponSlots.Count - 1)
            {
                Debug.LogError("Inventory slots already full!");
                return;
            }

            // Spawn the starting weapon
            GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
            spawnedWeapon.transform.SetParent(transform);

            // Defensive checks before adding to inventory
            WeaponController weaponController = spawnedWeapon.GetComponent<WeaponController>();
            if (weaponController == null)
            {
                Debug.LogWarning("Weapon doesn't have WeaponController component");
                return;
            }

            // Use dummy inventory method if real one might fail
            SafeAddWeapon(weaponIndex, weaponController);

            weaponIndex++;
        }
        catch (System.Exception e)
        {
            // Log but don't let it disable the script
            Debug.LogWarning("Error in SpawnWeapon but continuing execution: " + e.Message);
        }
    }

}