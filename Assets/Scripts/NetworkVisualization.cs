using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkVisualization : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject connectorPrefab;
    public Transform parent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateVisualization();
        }
    }

    void GenerateVisualization()
    {
        for (int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);

        Dictionary<Vector2Int, Vector2> nodePositions = new Dictionary<Vector2Int, Vector2>();

        int[] layers = Settings.instance.layers;
        for (int layer = 0; layer < layers.Length; layer++)
        {
            for (int node = 0; node < layers[layer]; node++)
            {
                Vector2 pos = new Vector2(
                    layer * Settings.instance.layerSpacing - (Settings.instance.layerSpacing * (layers.Length - 1) / 2f),
                    (node * Settings.instance.nodeSpacing) - (Settings.instance.nodeSpacing * (layers[layer] - 1) / 2f));

                nodePositions.Add(new Vector2Int(layer, node), pos);

                Instantiate(nodePrefab, pos, Quaternion.identity)
                    .transform.SetParent(parent);

                if (layer == 0)
                    continue;

                for (int connector = 0; connector < layers[layer-1]; connector++)
                {
                    Vector2 prevNodePos = nodePositions[new Vector2Int(layer - 1, connector)];

                    Transform prefab = Instantiate(connectorPrefab, Vector2.Lerp(pos, prevNodePos, 0.5f), Quaternion.identity).transform;
                    prefab.SetParent(parent);
                    prefab.localScale = new Vector2(prefab.transform.localScale.y, Vector2.Distance(pos, nodePositions[new Vector2Int(layer - 1, connector)]));
                    prefab.rotation = Quaternion.LookRotation(Vector3.forward, prevNodePos-pos);
                }
            }
        }
    }
}
