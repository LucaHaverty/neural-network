using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;
    void Awake() { instance = this;  }

    [Header("Graph Config")]
    public int textureResolution;

    [Header("Network Config")]
    public int[] layers;
    public int numTrainingPoints;
    public float learnRate;
    public bool autoInitPoints;

    [Header("Network Visualization")]
    public float layerSpacing;
    public float nodeSpacing;

    public GameObject redPrefab;
    public GameObject bluePrefab;
}
