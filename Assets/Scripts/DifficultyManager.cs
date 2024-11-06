using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class DifficultyManager : MonoBehaviour
{
    // Map variables
    public int enemyAmount = 20;
    public int bombAmount = 30;
    public int shootEnemyAmount = 15;
    public int width = 200;
    public int height = 100;
    public int hilbertReps = 3;

    public float chargingDuration = 1f;
    public float safeRadius = 3f;
    public float chaseSpeed = 4f;
    public float detectionRadius = 5f;
    public float jumpCooldownDuration = 5f;

    public float detonationSpeed = 2f;
    public float radius = 3f;
    public float knockbackConcusionTime = 1f;

    public float projectileSpeed = 10f;
    public float shootingRadius = 10f;
    public float shootingInterval = 2f;

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
        chargingDuration = 1f;
        safeRadius = 3f;
        chaseSpeed = 4f;
        detectionRadius = 5f;
        jumpCooldownDuration = 5f;
        
        // Reset bomb enemies
        detonationSpeed = 2f;
        radius = 3f;
        knockbackConcusionTime = 1f;

        // Reset shooting enemies
        projectileSpeed = 10f;
        shootingRadius = 10f;
        shootingInterval = 2f;

        // Reset map
        enemyAmount = 20;
        bombAmount = 30;
        enemyAmount = 15;
        height = 100;
        width = 200;
        hilbertReps = 3;

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
        if (reduceDifficulty)
        {
            chargingDuration = Mathf.Min(1.75f, chargingDuration + 0.25f);
            safeRadius = Mathf.Max(1.5f, safeRadius - 0.75f);
        }
        else
        {
            chargingDuration = Mathf.Max(0.5f, chargingDuration - 0.25f);
            safeRadius = Mathf.Min(5f, safeRadius + 0.5f);
        }

        // Change detonation speed and explosion radius for bomb enemies
        if (reduceDifficulty)
        {
            detonationSpeed = Mathf.Min(3.5f, detonationSpeed + 0.5f);
            radius = Mathf.Max(2f, radius - 0.5f);
        }
        else
        {
            detonationSpeed = Mathf.Max(0.5f, detonationSpeed - 0.5f);
            radius = Mathf.Min(4f, radius + 0.25f);
        }

        // Change projectile speed and shooting distance for shooting enemies
        if (reduceDifficulty)
        {
            projectileSpeed = Mathf.Max(6f, projectileSpeed - 1f);
            shootingRadius = Mathf.Max(6f, shootingRadius - 1f);
        }
        else
        {
            projectileSpeed = Mathf.Min(13f, projectileSpeed + 1f);
            shootingRadius = Mathf.Min(12f, shootingRadius + 0.5f);
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
        if (reduceDifficulty)
        {
            chaseSpeed = Mathf.Max(2f, chaseSpeed - 1f);
            detectionRadius = Mathf.Max(3f, detectionRadius - 1f);
        }
        else
        {
            chaseSpeed = Mathf.Min(6f, chaseSpeed + 1f);
            detectionRadius = Mathf.Min(8f, detectionRadius + 1f);
        }

        // Change size of map and amount of turns
        if (reduceDifficulty)
        {
            height = Mathf.Max(55, height - 15);
            width = Mathf.Max(110, width - 30);
            hilbertReps = Mathf.Max(2, hilbertReps - 1);
        }
        else
        {
            height = Mathf.Min(145, height + 15);
            width = Mathf.Min(290, width + 30);
            hilbertReps = Mathf.Min(5, hilbertReps + 1);
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
        if (reduceDifficulty)
        {
            jumpCooldownDuration = Mathf.Min(7f, jumpCooldownDuration + 0.5f);
        }
        else
        {
            jumpCooldownDuration = Mathf.Max(2.5f, jumpCooldownDuration - 0.5f);
        }

        // Change attack concussion time for bomb enemies
        if (reduceDifficulty)
        {
            knockbackConcusionTime = Mathf.Max(0.25f, knockbackConcusionTime - 0.25f);
        }
        else
        {
            knockbackConcusionTime = Mathf.Min(2f, knockbackConcusionTime + 0.25f);
        }

        // Change attack cooldown for shooting enemies
        if (reduceDifficulty)
        {
            shootingInterval = Mathf.Min(4f, shootingInterval + 0.5f);
        }
        else
        {
            shootingInterval = Mathf.Max(1f, shootingInterval - 0.25f);
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
        if (reduceDifficulty)
        {
            enemyAmount = Mathf.Max(10, enemyAmount - 2);
            bombAmount = Mathf.Max(20, bombAmount - 2);
            enemyAmount = Mathf.Max(7, enemyAmount - 2);
        }
        else
        {
            enemyAmount = Mathf.Min(30, enemyAmount + 2);
            bombAmount = Mathf.Min(40, bombAmount + 2);
            enemyAmount = Mathf.Min(23, enemyAmount + 2);
        }
    }
}

