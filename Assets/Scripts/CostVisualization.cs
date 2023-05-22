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
    public TextMeshProUGUI[] costLabels;

    public TextMeshProUGUI iterationText;
    public TextMeshProUGUI costText;

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
        if (Input.GetKeyDown(KeyCode.G))
        {
            PopulateGraph();
        }
    }

    List<float> dataPoints = new List<float>();
    private void OnCostUpdate(float newCost)
    {
        dataPoints.Add(newCost);
    }

    private void OnNetworkReset()
    {
        dataPoints.Clear();
    }   

    public void ResetTextUnderGraph() 
    {
        iterationText.text = $"Iteration: 0";
        costText.text = $"Average Cost: N/A";
    }
    
    public void PopulateGraph()
    {
        // Text under graph
        iterationText.text = $"Iteration: {dataPoints.Count}";
        costText.text = $"Average Cost: {Mathf.RoundToInt(dataPoints[^1]*1000)/1000f}";
        
        // Remove old points
        for (int i = 0; i < graphPointsContainer.childCount; i++)
            Destroy(graphPointsContainer.GetChild(i).gameObject);
        
        xRange = dataPoints.Count;

        List<float> positionsToGraph = new List<float>();
        positionsToGraph.Add(0);
        for (int i = 1; i < numPoints; i++)
        {
            float percent = (float)i / (numPoints-1);
            positionsToGraph.Add(Mathf.Max(percent*xRange-1,0));
        }
        
        List<Vector2> worldPositions = new List<Vector2>();
        foreach (float index in positionsToGraph)
        {
            Vector2 graphPos = new Vector2(index, dataPoints[Mathf.FloorToInt(index)]);
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
            axisLabelsX[i].text = $"{Mathf.FloorToInt(positionsToGraph[i*5])+1}";

            costLabels[i].text = (Mathf.Round(dataPoints[Mathf.FloorToInt(positionsToGraph[i*5])]*1000)/1000f).ToString();
            costLabels[i].transform.position = graphPointsContainer.GetChild(i*5).position + new Vector3(0, 1.2f);
        }
    }

    Vector2 GraphToWorldPos(Vector2 graphPos)
    {
        return new Vector2(Mathf.Lerp(origin.position.x, topRightPoint.position.x, graphPos.x / xRange), Mathf.Lerp(origin.position.y, topRightPoint.position.y, graphPos.y / yDomain));
    }
}
