using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class CostVisualization : MonoBehaviour
{
    public static CostVisualization instance;
    void Awake() { instance = this; }

    public Transform origin;
    public Transform topRightPoint;
    public GameObject pointPrefab;
    public GameObject connectorPrefab;
    public Transform graphPointsContainer;
    
    public int numPoints;

    public float yDomain;
    public float xRange;

    public TextMeshProUGUI[] axisLabelsX;


    List<Vector2> testData = new List<Vector2>();
    void Start()
    {
        NetworkController.instance.OnNetworkLearn.AddListener(OnCostUpdate);
        NetworkController.instance.OnNetworkReset.AddListener(OnNetworkReset);
        /*testData.Add(new Vector2(1, 3));
        testData.Add(new Vector2(3, 5));
        testData.Add(new Vector2(5, 3));
        testData.Add(new Vector2(7, 7.5f));
        testData.Add(new Vector2(9, 2.3f));*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            PopulateGraph();
    }

    List<float> dataPoints = new List<float>();
    private void OnCostUpdate(float newCost)
    {
        dataPoints.Add(newCost);
        PopulateGraph();
    }

    private void OnNetworkReset()
    {
        dataPoints.Clear();
    }

    void PopulateGraph()
    {
        // Remove old points - keep origin and topRightPoint transforms
        for (int i = 0; i < graphPointsContainer.childCount; i++)
            Destroy(graphPointsContainer.GetChild(i).gameObject);
        
        xRange = dataPoints.Count;

        List<int> iterationsToGraph = new List<int>();
        iterationsToGraph.Add(0);
        for (int i = 1; i < numPoints; i++)
        {
            float percent = (float)i / (numPoints-1);
            iterationsToGraph.Add(Mathf.Max(Mathf.FloorToInt(percent*xRange)-1,0));
        }

        /*string result = "List contents: ";
        foreach (int item in iterationsToGraph)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);*/

        List<Vector2> worldPositions = new List<Vector2>();
        foreach (int index in iterationsToGraph)
        {
            Vector2 graphPos = new Vector2(index, dataPoints[index]);
            GameObject point = Instantiate(pointPrefab, GraphToWorldPos(graphPos), Quaternion.identity);
            point.transform.SetParent(graphPointsContainer);
            point.transform.localScale = new Vector2(0.75f, 0.75f);
            
            worldPositions.Add(point.transform.position);
        }

        for (int i = 1; i < worldPositions.Count; i++)
        {
            Vector2 posA = worldPositions[i];
            Vector2 posB = worldPositions[i - 1];
            Vector2 connectorPos = Vector2.Lerp(posA, posB, 0.5f);
            GameObject newConnector = Instantiate(connectorPrefab, connectorPos, Quaternion.identity);
            newConnector.transform.SetParent(graphPointsContainer);

            newConnector.transform.localScale = new Vector2(Vector2.Distance(posA, posB), newConnector.transform.localScale.y);
            
            newConnector.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(posB.y-posA.y, posB.x-posA.x)*Mathf.Rad2Deg);
        }

        for (int i = 0; i < axisLabelsX.Length; i++)
        {
            axisLabelsX[i].text = $"{iterationsToGraph[i]}";
        }
    }

    Vector2 GraphToWorldPos(Vector2 graphPos)
    {
        return new Vector2(Mathf.Lerp(origin.position.x, topRightPoint.position.x, graphPos.x / xRange), Mathf.Lerp(origin.position.y, topRightPoint.position.y, graphPos.y / yDomain));
    }
}
