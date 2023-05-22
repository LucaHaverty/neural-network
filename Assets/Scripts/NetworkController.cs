using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;
    void Awake() { instance = this;  }

    public UnityEvent<float> OnNetworkLearn = new UnityEvent<float>();
    public UnityEvent OnNetworkReset = new UnityEvent();

    //int[] layers = { 2, 3, 2 };
    public NeuralNetwork network;

    public float[] weights_1_0;

    public float[] weights_1_1;

    public float[] weights_1_2;

    public float[] biases_1;

    public float[] weights_2_0;

    public float[] weights_2_1;

    public float[] biases_2;

    List<DataPoint> trainingData;
    //int currentNumPoints;

    bool started = false;
    bool autoEval;

    void Start()
    {
        trainingData = new List<DataPoint>();

        /*float min = -1f / 3f;
        float max = 1f / 3f;
        
        for (int i = 0; i < weights_1_0.Length; i++) weights_1_0[i] = Random.Range(min, max);
        for (int i = 0; i < weights_1_1.Length; i++) weights_1_1[i] = Random.Range(min, max);
        for (int i = 0; i < weights_1_2.Length; i++) weights_1_2[i] = Random.Range(min, max);
        for (int i = 0; i < biases_1.Length; i++) biases_1[i] = Random.Range(min, max);
        for (int i = 0; i < weights_2_0.Length; i++) weights_2_0[i] = Random.Range(min, max);
        for (int i = 0; i < weights_2_1.Length; i++) weights_2_1[i] = Random.Range(min, max);
        for (int i = 0; i < biases_2.Length; i++) biases_2[i] = Random.Range(min, max);*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            StartNetwork();

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(TestRoutine());
        }

        if (started)
        {
            GraphController.instance.UpdateTexture();

            if (Input.GetKey(KeyCode.Space))
                LearnForIterations(1);

            if (Input.GetKeyDown(KeyCode.L))
            {
                ScreenshotController.instance.TakeNetworkPicture("");
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                LearnForIterations(10);
            if (Input.GetKeyDown(KeyCode.Alpha2))   
                LearnForIterations(40);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                StartCoroutine(LearnForIterationsBatched(50));
            if (Input.GetKeyDown(KeyCode.Alpha4))
                StartCoroutine(LearnForIterationsBatched(400));
            if (Input.GetKeyDown(KeyCode.Alpha5))
                StartCoroutine(LearnForIterationsBatched(1600));
            if (Input.GetKeyDown(KeyCode.Alpha6))
                StartCoroutine(LearnForIterationsBatched(2000));
            if (Input.GetKeyDown(KeyCode.Alpha7))
                StartCoroutine(LearnForIterationsBatched(10000));
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                network = new NeuralNetwork(Settings.instance.layers);
                OnNetworkReset.Invoke();
            }

            network.GetWeights(GetWeightsCallback);

            if (Input.GetKeyDown(KeyCode.A))
                autoEval = !autoEval;

            if (autoEval)
                network.Learn(trainingData.ToArray(), Settings.instance.learnRate);
        }

        if (Input.GetMouseButtonDown(0))
        {
            AddPoint(Classification.Blue, GraphController.WorldToGraphPos(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }

        if (Input.GetMouseButtonDown(1))
        {
            AddPoint(Classification.Red, GraphController.WorldToGraphPos(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }

    public IEnumerator TestRoutine()
    {
        CostVisualization.instance.ResetTextUnderGraph();
        GraphController.instance.ToggleOverlay();
        ScreenshotController.instance.TakeNetworkPicture("0");
        GraphController.instance.ToggleOverlay();

        LearnForIterations(10);
        ScreenshotController.instance.TakeNetworkPicture("10");
        yield return null;
        LearnForIterations(90);
        ScreenshotController.instance.TakeNetworkPicture("100");
        yield return null;
        yield return StartCoroutine(LearnForIterationsBatched(400));
        ScreenshotController.instance.TakeNetworkPicture("500");
        yield return StartCoroutine(LearnForIterationsBatched(500));
        ScreenshotController.instance.TakeNetworkPicture("1000");
        yield return StartCoroutine(LearnForIterationsBatched(1000));
        ScreenshotController.instance.TakeNetworkPicture("2000");
        ScreenshotController.instance.TakeCostPicture("cost");
    }

    private IEnumerator LearnForIterationsBatched(int iterations)
    {
        iterations /= 50;
        for (int i = 0; i < iterations; i++)
        {
            for (int iteration = 0; iteration < 50; iteration++)
            {
                network.Learn(trainingData.ToArray(), Settings.instance.learnRate);
            }
            CostVisualization.instance.PopulateGraph();
            yield return null;
        }
    }

    private void LearnForIterations(int iterations)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            network.Learn(trainingData.ToArray(), Settings.instance.learnRate);
        }
        CostVisualization.instance.PopulateGraph();
    }

    void StartNetwork()
    {
        started = true;

        network = new NeuralNetwork(Settings.instance.layers);
        if (Settings.instance.autoInitPoints) AutoInitializePoints();
    }

    void UpdateNetworkWeights()
    {
        // Update based on inspector weights and biases in real time
        float[,] weights_1 = new float[,] { { weights_1_0[0], weights_1_0[1] }, { weights_1_1[0], weights_1_1[1] }, { weights_1_2[0], weights_1_2[1] } };
        float[,] weights_2 = new float[,] { { weights_2_0[0], weights_2_0[1], weights_2_0[2] }, { weights_2_1[0], weights_2_1[1], weights_2_1[2] } };

        // float[,] weights_1 = new float[,] { { weights_1_0[0], weights_1_0[1] }, { weights_1_1[0], weights_1_1[1] } };

        network.UpdateWeights(weights_1, biases_1, weights_2, biases_2);
    }

    void AutoInitializePoints()
    {
        for (int i = 0; i < Settings.instance.numTrainingPoints/2; i++)
        {
            Vector2 position = Vector2.zero;
            Classification type = Classification.Blue;
            while (type != Classification.Red)
            {
                position = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                type = position.y > 0 ? Classification.Blue : Classification.Red;
                if (type == Classification.Red)
                    continue;
            }
            GameObject prefab = Instantiate(type == Classification.Red ? Settings.instance.redPrefab : Settings.instance.bluePrefab);
            prefab.transform.position = GraphController.GraphToWorldPos(position);
            prefab.transform.SetParent(GraphController.instance.transform);

            trainingData.Add(new DataPoint(position, type, prefab));
        }
        for (int i = 0; i < Settings.instance.numTrainingPoints / 2; i++)
        {

            Vector2 position = Vector2.zero;
            Classification type = Classification.Red;
            while (type != Classification.Blue)
            {
                position = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                type = position.y > 0 ? Classification.Blue : Classification.Red;
                if (type == Classification.Red)
                    continue;
            }
            GameObject prefab = Instantiate(type == Classification.Red ? Settings.instance.redPrefab : Settings.instance.bluePrefab);
            prefab.transform.position = GraphController.GraphToWorldPos(position);
            prefab.transform.SetParent(GraphController.instance.transform);

            trainingData.Add(new DataPoint(position, type, prefab));
        }
    }

    void AddPoint(Classification classification, Vector2 position)
    {
        GameObject prefab = Instantiate(classification == Classification.Red ? Settings.instance.redPrefab : Settings.instance.bluePrefab);
        prefab.transform.position = GraphController.GraphToWorldPos(position);
        prefab.transform.SetParent(GraphController.instance.transform);

        trainingData.Add(new DataPoint(position, classification, prefab));
    }

    void GetWeightsCallback(float[,] weights_1, float[] biases_1, float[,] weights_2, float[] biases_2)
    {
        this.biases_1 = biases_1;
        this.biases_2 = biases_2;

        weights_1_0[0] = weights_1[0, 0];
        weights_1_0[1] = weights_1[0, 1];

        weights_1_1[0] = weights_1[1, 0];
        weights_1_1[1] = weights_1[1, 1];

        weights_1_2[0] = weights_1[2, 0];
        weights_1_2[1] = weights_1[2, 1];

        weights_2_0[0] = weights_2[0, 0];
        weights_2_0[1] = weights_2[0, 1];

        weights_2_1[0] = weights_2[1, 0];
        weights_2_1[1] = weights_2[1, 1];

        this.biases_1 = biases_1;
        this.biases_2 = biases_2;
    }
}
