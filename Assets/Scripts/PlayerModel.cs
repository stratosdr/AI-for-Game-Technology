using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class PlayerModel : MonoBehaviour
{
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;

    // Means and Scales from Python's StandardScaler
    private readonly float[] means = 
    { 
        39.7287895f, 1.49180328f, 0.512568306f, 27.2957648f,
        12.4330243f, 1.35027322f, 0.403825137f, 2.20464481f,
        5.81748634f, 1.53251366f, 2.96010929f, 3.39125683f,
        0.0333333333f, 33.2922187f, 6.92540984f, 8.48746378f
    };

    private readonly float[] scales = 
    { 
        14.8812342f, 1.27553204f, 0.37670868f, 9.99241378f,
        5.91071603f, 0.75941865f, 0.56369676f, 1.03115022f,
        3.77709088f, 1.70013775f, 2.20201778f, 2.0637089f,
        0.14074354f, 25.3835224f, 2.95719093f, 0.4086001f
    };

    void Awake()
    {
        if (FindObjectsOfType<PlayerModel>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); // Keep this model between scenes
    }

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);
    }

    public float[] MakePrediction(SessionData sessionData)
    {
        // Preprocess SessionData
        float[] inputParameters = PreprocessSessionData(sessionData);
        
        // Create input tensor from the float array
        Tensor inputTensor = new Tensor(1, inputParameters.Length, inputParameters);
        
        // Execute the model
        worker.Execute(inputTensor);

        // Get the output tensor
        Tensor outputTensor = worker.PeekOutput();
        float[] predictions = outputTensor.ToReadOnlyArray();
        // Post-process the predictions: round and clip between 1 and 5
        for (int i = 0; i < predictions.Length; i++)
        {
            predictions[i] = Mathf.Clamp(Mathf.Round(predictions[i]), 1, 5);
        }

        inputTensor.Dispose();
        outputTensor.Dispose();

        Debug.Log("Made player modeling predictions: " + string.Join(", ", predictions));

        return predictions;
    }

    private float[] PreprocessSessionData(SessionData sessionData)
    {
        // Prepare a list to store scaled values
        List<float> preprocessedData = new List<float>();

        // Normalization factor
        float normalizationFactor = sessionData.timesRestarted + 1;

        // Scale and add each relevant feature in order, normalizing where necessary
        preprocessedData.Add((sessionData.levelTime - means[0]) / scales[0]);
        preprocessedData.Add(sessionData.levelComplete ? 1f : 0f);
        preprocessedData.Add((sessionData.timesRestarted - means[1]) / scales[1]);
        preprocessedData.Add((sessionData.timesDied - means[2]) / scales[2]);
        preprocessedData.Add((sessionData.timeSprinting - means[3]) / scales[3]);
        preprocessedData.Add((sessionData.timeWalking - means[4]) / scales[4]);
        preprocessedData.Add((sessionData.damageTakenNormal - means[5]) / scales[5]);
        preprocessedData.Add((sessionData.damageTakenBomb - means[6]) / scales[6]);
        preprocessedData.Add((sessionData.damageTakenBullet - means[7]) / scales[7]);
        preprocessedData.Add((sessionData.enemyJumpsMissed - means[8]) / scales[8]);
        preprocessedData.Add((sessionData.enemyBulletsMissed - means[9]) / scales[9]);
        preprocessedData.Add((sessionData.enemyBombsMissed - means[10]) / scales[10]);
        preprocessedData.Add((sessionData.bombsIgnited - means[11]) / scales[11]);
        preprocessedData.Add((sessionData.timesPaused - means[12]) / scales[12]);
        preprocessedData.Add((sessionData.detectionTime - means[13]) / scales[13]);
        preprocessedData.Add((sessionData.collisions - means[14]) / scales[14]);

        // Calculate and add average speed
        float walkSpeed = 5f;
        float sprintSpeed = 10f;
        float totalDistance = (sessionData.timeSprinting * sprintSpeed) + (sessionData.timeWalking * walkSpeed);
        float averageSpeed = totalDistance / (sessionData.levelTime > 0 ? sessionData.levelTime : 1);
        preprocessedData.Add((averageSpeed - means[15]) / scales[15]);

        return preprocessedData.ToArray();
    }

    void OnDestroy()
    {
        if (worker != null)
        {
            worker.Dispose();
            worker = null;
        }
    }
}
