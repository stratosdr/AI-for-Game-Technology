using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PlayerAnalytics
{
    public string playerId;
    public List<SessionData> sessions;

    public PlayerAnalytics()
    {
        playerId = System.Guid.NewGuid().ToString(); // Generate a new player ID
        sessions = new List<SessionData>();
    }

    public void SaveToJson(string filePath)
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(filePath, json);
    }
}

[System.Serializable]
public class SessionData
{
    public float levelTime;  // time taken to complete level
    public bool levelComplete; // whether level was completed or player quit
    public int timesRestarted;  // times restarted
    public int timesDied;  // times died
    public float timeSprinting;  // time spent sprinting
    public float timeWalking;  // time spent walking
    public int damageTakenNormal;  // total  damage taken
    public int damageTakenBomb;  // total  damage taken
    public int damageTakenBullet;  // total  damage taken
    public int enemyJumpsMissed;
    public int enemyBulletsMissed;
    public int enemyBombsMissed;
    public int bombsIgnited;
    public int timesPaused;  // times paused
    public float detectionTime;  // time spent in enemy detection zone
    public int collisions;  // amount of collisions

    public SessionData()
    {   
        levelTime = 0f;
        levelComplete = false;
        timesRestarted = 0;
        timesDied = 0;
        timeSprinting = 0f;
        timeWalking = 0f;
        damageTakenNormal = 0;
        damageTakenBomb = 0;
        damageTakenBullet = 0;
        enemyJumpsMissed = 0;
        enemyBulletsMissed = 0;
        enemyBombsMissed = 0;
        bombsIgnited = 0;
        timesPaused = 0;
        detectionTime = 0f;
        collisions = 0;
    }
}

public class AnalyticsManager : MonoBehaviour
{
    private PlayerAnalytics playerAnalytics;
    private SessionData currentSessionData;
    private float levelStartTime;
    private float lastTimeChangedSpeed;
    private bool isSprinting;
    private string analyticsFilePath;
    private PlayerModel playerModel;


    void Awake()
    {
        if (FindObjectsOfType<AnalyticsManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Create new player analytics data
        playerAnalytics = new PlayerAnalytics();

        // Set the analytics file path based on the new player ID
        analyticsFilePath = Path.Combine(Application.dataPath, $"analytics_{playerAnalytics.playerId}.json");
        
        DontDestroyOnLoad(gameObject); // Keep this manager between scenes
    }

    void Start()
    {
        playerModel = FindObjectOfType<PlayerModel>();
        if (playerModel == null)
        {
            Debug.LogError("PlayerModel not found in the scene!");
        }
    }

    public void StartNewSession()
    {
        currentSessionData = new SessionData();
        lastTimeChangedSpeed = Time.time;
        isSprinting = false;
        levelStartTime = Time.time;
    }

    public void EndSession(bool levelComplete, bool newSession)
    {
        currentSessionData.levelTime = Time.time - levelStartTime;
        currentSessionData.levelComplete = levelComplete;
        if (isSprinting) RecordSprinting();
        else RecordWalking();
        playerAnalytics.sessions.Add(currentSessionData);

        // Make prediction after the session ends
        float[] currentPrediction = playerModel.MakePrediction(currentSessionData);

        if (newSession) StartNewSession(); // Prepare for a new session
    }

    public void RecordRestart()
    {
        currentSessionData.timesRestarted++;
    }

    public void RecordDeath()
    {
        currentSessionData.timesDied++;
    }

    public void RecordSprinting()
    {
        currentSessionData.timeSprinting += Time.time - lastTimeChangedSpeed;
        lastTimeChangedSpeed = Time.time;
        isSprinting = false;
    }

    public void RecordWalking()
    {
        currentSessionData.timeWalking += Time.time - lastTimeChangedSpeed;
        lastTimeChangedSpeed = Time.time;
        isSprinting = true;
    }

    public void RecordNormalDamage(int damage)
    {
        currentSessionData.damageTakenNormal += damage;
    }

    public void RecordBombDamage(int damage){
        currentSessionData.damageTakenBomb += damage;
    }

    public void RecordBulletDamage(int damage){
        currentSessionData.damageTakenBullet += damage;
    }

    public void RecordBombIgnition(){
        currentSessionData.bombsIgnited += 1;
    }

    public void RecordBombMissed(){
        currentSessionData.enemyBombsMissed += 1;
    }

    public void RecordJumpMissed(){
        currentSessionData.enemyJumpsMissed += 1;
    }
    
    public void RecordBulletMissed(){
        currentSessionData.enemyBulletsMissed += 1;
    }


    public void RecordPause()
    {
        currentSessionData.timesPaused++;
    }

    public void RecordDetectionTime(float detectionTime)
    {
        currentSessionData.detectionTime += detectionTime;
    }

    public void RecordCollision()
    {
        currentSessionData.collisions++;
    }

    public void SaveAnalytics()
    {
        playerAnalytics.SaveToJson(analyticsFilePath);
        Debug.Log("Analytics saved to " + analyticsFilePath);
        
        playerAnalytics = new PlayerAnalytics();
        // Update the file path for the new player ID
        analyticsFilePath = Path.Combine(Application.dataPath, $"analytics_{playerAnalytics.playerId}.json");
        StartNewSession(); // Start a new session
    }
}
