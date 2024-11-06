using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class DifficultyManager : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<DifficultyManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject); // Keep this manager between scenes
    }


    public void UpdateDifficulty(float[] prediction)
    {   
        // Rename predictions for readability
        float competence = prediction[0];
        float flow = prediction[1];
        float tension = prediction[2];
        float challenge = prediction[3];
        float negativeAffect = prediction[4];
        float positiveAffect = prediction[5];

        UpdateCompetenceCharacteristics(competence, positiveAffect);
        UpdateFlowCharacteristics(flow, negativeAffect);
        UpdateTensionCharacteristics(tension, positiveAffect);
        UpdateChallengeCharacteristics(challenge, competence, negativeAffect);

        Debug.Log("Difficulty adjusted");
    }

    public void ResetDifficulty()
    {   
        // Reset enemies
        EnemyBehavior[] enemies = FindObjectsOfType<EnemyBehavior>();
        foreach (var enemy in enemies)
        {
            enemy.chargingDuration = 1f;
            enemy.safeRadius = 3f;
            enemy.chaseSpeed = 4f;
            enemy.detectionRadius = 5f;
            enemy.jumpCooldownDuration = 5f;    
        }
        
        // Reset bomb enemies
        BombBehaviour[] bombEnemies = FindObjectsOfType<BombBehaviour>();
        foreach (var bombEnemy in bombEnemies)
        {
            bombEnemy.detonationSpeed = 2f;
            bombEnemy.radius = 3f;
            bombEnemy.knockbackConcusionTime = 1f;
        }

        // Reset shooting enemies
        ShootingEnemyBehavior[] shootingEnemies = FindObjectsOfType<ShootingEnemyBehavior>();
        foreach (var shootingEnemy in shootingEnemies)
        {
            shootingEnemy.projectileSpeed = 10f;
            shootingEnemy.shootingRadius = 10f;
            shootingEnemy.shootingInterval = 2f;
        }

        // Reset map
        CellularLevelGenerator mapGenerator = FindObjectOfType<CellularLevelGenerator>();
        mapGenerator.enemyAmount = 20;
        mapGenerator.bombAmount = 30;
        mapGenerator.enemyAmount = 15;
        mapGenerator.height = 100;
        mapGenerator.width = 200;
        mapGenerator.hilbertReps = 3;

        Debug.Log("Difficulty reset");
    }

    public void UpdateCompetenceCharacteristics(float competence, float positiveAffect)
    {   
        bool reduceDifficulty = false;
        switch (competence)
        {
            case 3:
                if (positiveAffect >= 3)
                {
                    Debug.Log("Difficulty adjustment: Competence not affecting difficulty");
                    return;
                }
                // Player skillful but not content
                else
                {
                   Debug.Log("Difficulty adjustment: Competence increasing difficulty"); 
                }
                break;
            case <= 2:
                Debug.Log("Difficulty adjustment: Competence reducing difficulty");
                reduceDifficulty = true;
                break;
            default:
                Debug.Log("Difficulty adjustment: Competence increasing difficulty");
                break;
        }
        
        // Change charging duration and attack radius for normal enemies
        EnemyBehavior[] enemies = FindObjectsOfType<EnemyBehavior>();
        foreach (var enemy in enemies)
        {
            if (reduceDifficulty) {
                enemy.chargingDuration = Mathf.Min(1.75f, enemy.chargingDuration + 0.25f);
                enemy.safeRadius = Mathf.Max(1.5f, enemy.safeRadius - 0.75f);
            }
            else {
                enemy.chargingDuration = Mathf.Max(0.5f, enemy.chargingDuration - 0.25f);
                enemy.safeRadius = Mathf.Min(5f, enemy.safeRadius + 0.5f);
            }
        }

        // Change detonation speed and explosion radius for bomb enemies
        BombBehaviour[] bombEnemies = FindObjectsOfType<BombBehaviour>();
        foreach (var bombEnemy in bombEnemies)
        {
            if (reduceDifficulty) {
                bombEnemy.detonationSpeed = Mathf.Min(3.5f, bombEnemy.detonationSpeed + 0.5f);
                bombEnemy.radius = Mathf.Max(2f, bombEnemy.radius - 0.5f);
            }
            else {
                bombEnemy.detonationSpeed = Mathf.Max(0.5f, bombEnemy.detonationSpeed - 0.5f);
                bombEnemy.radius = Mathf.Min(4f, bombEnemy.radius + 0.25f);
            }
        }

        // Change projectile speed and shooting distance for shooting enemies
        ShootingEnemyBehavior[] shootingEnemies = FindObjectsOfType<ShootingEnemyBehavior>();
        foreach (var shootingEnemy in shootingEnemies)
        {
            if (reduceDifficulty) {
                shootingEnemy.projectileSpeed = Mathf.Max(6f, shootingEnemy.projectileSpeed - 1f);
                shootingEnemy.shootingRadius = Mathf.Max(6f, shootingEnemy.shootingRadius - 1f);
            }
            else {
                shootingEnemy.projectileSpeed = Mathf.Min(13f, shootingEnemy.projectileSpeed + 1f);
                shootingEnemy.shootingRadius = Mathf.Min(12f, shootingEnemy.shootingRadius + 0.5f);
            }
        }
    }

    public void UpdateFlowCharacteristics(float flow, float negativeAffect)
    {   
        bool reduceDifficulty = false;
        switch (flow)
        {
            case 3:
                // Player in flow but somewhat bored
                if (negativeAffect >= 3)
                {
                    Debug.Log("Difficulty adjustment: Flow increasing difficulty");
                }
                else
                {
                    Debug.Log("Difficulty adjustment: Flow not affecting difficulty");
                    return;
                }
                break;
            case <= 2:
                Debug.Log("Difficulty adjustment: Flow reducing difficulty");
                reduceDifficulty = true;
                break;
            default:
                Debug.Log("Difficulty adjustment: Flow increasing difficulty");
                break;
        }
        
        // Change chase speed and detection radius for normal enemies
        EnemyBehavior[] enemies = FindObjectsOfType<EnemyBehavior>();
        foreach (var enemy in enemies)
        {
            if (reduceDifficulty) {
                enemy.chaseSpeed = Mathf.Max(2f, enemy.chaseSpeed - 1f);
                enemy.detectionRadius = Mathf.Max(3f, enemy.detectionRadius - 1f);
            }
            else {
                enemy.chaseSpeed = Mathf.Min(6f, enemy.chaseSpeed + 1f);
                enemy.detectionRadius = Mathf.Min(8f, enemy.detectionRadius + 1f);
            }
        }

        // Change size of map and amount of turns
        CellularLevelGenerator mapGenerator = FindObjectOfType<CellularLevelGenerator>();
        if (reduceDifficulty)
        {
            mapGenerator.height = Mathf.Max(55, mapGenerator.height - 15);
            mapGenerator.width = Mathf.Max(110, mapGenerator.width - 30);
            mapGenerator.hilbertReps = Mathf.Max(2, mapGenerator.hilbertScale - 1);
        }
        else
        {
            mapGenerator.height = Mathf.Min(145, mapGenerator.height + 15);
            mapGenerator.width = Mathf.Min(290, mapGenerator.width + 30);
            mapGenerator.hilbertReps = Mathf.Min(5, mapGenerator.hilbertReps + 1);
        }
    }

    
    public void UpdateTensionCharacteristics(float tension, float positiveAffect)
    {   
        bool reduceDifficulty = false;
        switch (tension)
        {
            case 3:
                // Moderate tension but player is not feeling good, reduce tension
                if (positiveAffect <= 2)
                {
                    Debug.Log("Difficulty adjustment: Tension reducing difficulty");
                    reduceDifficulty = true;
                }
                else
                {
                    Debug.Log("Difficulty adjustment: Tension not affecting difficulty");
                    return; 
                }
                break;
            case >= 4:
                Debug.Log("Difficulty adjustment: Tension reducing difficulty");
                reduceDifficulty = true;
                break;
            default:
                // Low tension but player is not feeling good, increase tension
                if (positiveAffect <= 2)
                {
                    Debug.Log("Difficulty adjustment: Tension increasing difficulty");
                }
                else
                {
                    Debug.Log("Difficulty adjustment: Tension not affecting difficulty");
                    return;
                }
                break;
        }

        // Change attack cooldown for normal enemies
        EnemyBehavior[] enemies = FindObjectsOfType<EnemyBehavior>();
        foreach (var enemy in enemies)
        {
            if (reduceDifficulty)
            {
                enemy.jumpCooldownDuration = Mathf.Min(7f, enemy.jumpCooldownDuration + 0.5f);
            }
            else
            {
                enemy.jumpCooldownDuration = Mathf.Max(2.5f, enemy.jumpCooldownDuration - 0.5f);
            }
        }

        // Change attack concussion time for bomb enemies
        BombBehaviour[] bombEnemies = FindObjectsOfType<BombBehaviour>();
        foreach (var bombEnemy in bombEnemies)
        {
            if (reduceDifficulty)
            {
                bombEnemy.knockbackConcusionTime = Mathf.Max(0.25f, bombEnemy.knockbackConcusionTime - 0.25f);
            }
            else
            {
                bombEnemy.knockbackConcusionTime = Mathf.Min(2f, bombEnemy.knockbackConcusionTime + 0.25f);
            }
        }

        // Change attack cooldown for shooting enemies
        ShootingEnemyBehavior[] shootingEnemies = FindObjectsOfType<ShootingEnemyBehavior>();
        foreach (var shootingEnemy in shootingEnemies)
        {
            if (reduceDifficulty) {
                shootingEnemy.shootingInterval = Mathf.Min(4f, shootingEnemy.shootingInterval + 0.5f);
            }
            else {
                shootingEnemy.shootingInterval = Mathf.Max(1f, shootingEnemy.shootingInterval - 0.25f);
            }
        }
    }

    public void UpdateChallengeCharacteristics(float challenge, float competence, float negativeAffect)
    {   
        bool reduceDifficulty = false;
        switch (challenge)
        {
            case 3:
                Debug.Log("Difficulty adjustment: Challenge not affecting difficulty");
                return;
            case <= 2:
                // Player good enough at playing and challenge low, increase challenge
                if (competence >= 3 || negativeAffect >= 3)
                {
                    Debug.Log("Difficulty adjustment: Challenge increasing difficulty");
                }
                else
                {
                    Debug.Log("Difficulty adjustment: Challenge reducing difficulty");
                    reduceDifficulty = true;
                }
                break;
            default:
                // Player not good enough at playing but challenge high, reduce challenge
                if (competence <= 2 || negativeAffect >= 3)
                {
                    Debug.Log("Difficulty adjustment: Challenge reducing difficulty");
                    reduceDifficulty = true;
                }
                else
                {  
                    Debug.Log("Difficulty adjustment: Challenge not affecting difficulty");
                    return;
                }
                break;
        }
        
        // Change count of enemies
        CellularLevelGenerator mapGenerator = FindObjectOfType<CellularLevelGenerator>();
        if (reduceDifficulty)
        {
            mapGenerator.enemyAmount = Mathf.Max(10, mapGenerator.enemyAmount - 2);
            mapGenerator.bombAmount = Mathf.Max(20, mapGenerator.bombAmount - 2);
            mapGenerator.enemyAmount = Mathf.Max(7, mapGenerator.enemyAmount - 2);
        }
        else
        {
            mapGenerator.enemyAmount = Mathf.Min(30, mapGenerator.enemyAmount + 2);
            mapGenerator.bombAmount = Mathf.Min(40, mapGenerator.bombAmount + 2);
            mapGenerator.enemyAmount = Mathf.Min(23, mapGenerator.enemyAmount + 2);
        }
    }
}

