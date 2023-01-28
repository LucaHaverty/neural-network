using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOutput : MonoBehaviour
{
    public static InputOutput instance;
    void Awake() { instance = this; }

    public float tileSize;
    public Vector2 originOffset;
    public Vector2 graphBounds;
    public int textureResolution;

    public int numTrainingPoints;
    public GameObject bluePointPrefab;
    public GameObject redPointPrefab;

    public SpriteRenderer graphOverlay;

    Sprite graphSprite;
    Texture2D graphTexture;

    
    DataPoint[] trainingPoints;

    void Start()
    {
        InitializeOverlayTexture();
        InitializeTrainingPoints();
        //Debug.Log(
                //NetworkController.instance.network.AverageCost(trainingPoints)
        //    );
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 1000; i++) { }
                //NetworkController.instance.network.Learn(trainingPoints, 0.0001f);
        }
        //NetworkController.instance.network.Learn(trainingPoints, 0.0001f);
        //Debug.Log(NetworkController.instance.network.AverageCost(trainingPoints));

    }

    void InitializeOverlayTexture()
    {
        graphTexture = new Texture2D(textureResolution, textureResolution);
        
        graphSprite = Sprite.Create(graphTexture, new Rect(Vector2.zero, Vector2.one * textureResolution), Vector2.zero);
        graphOverlay.sprite = graphSprite;
        graphOverlay.transform.localScale = new Vector2(100f / textureResolution * 6.477029f * 2, 100f / textureResolution * 6.477029f * 2);
        graphOverlay.transform.position = new Vector2(-6.477029f, -6.477029f);

        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                Classification color = FindClassification(TextureToGraphPos(new Vector2(x, y)));
                if (color ==  Classification.Red)
                    graphTexture.SetPixel(x, y, Color.red);
                else graphTexture.SetPixel(x, y, Color.blue);
            }
        }
        graphTexture.Apply();
    }

    Vector2 TextureToGraphPos(Vector2 texPos)
    {
        Vector2 globalPos = new Vector2((texPos.x - textureResolution / 2) / textureResolution * 6.477029f * 2, (texPos.y - textureResolution / 2) / textureResolution * 6.477029f * 2);
        return WorldToGraphPos(globalPos);
    }

    void InitializeTrainingPoints()
    {
        trainingPoints = new DataPoint[numTrainingPoints];
        for (int i = 0; i < numTrainingPoints; i++)
        {
            Vector2 pos = GetRandomPositionInBounds();
            Classification classification = FindClassification(pos);
            GameObject gameObject;

            if (classification == Classification.Red) gameObject = Instantiate(redPointPrefab, GraphToWorldPos(pos), Quaternion.identity);
            else gameObject =  Instantiate(bluePointPrefab, GraphToWorldPos(pos), Quaternion.identity);

            gameObject.transform.parent = this.transform;
            trainingPoints[i] = new DataPoint(pos, classification, gameObject);
        }
    }

    public Vector2 WorldToGraphPos(Vector2 worldPos)
    {
        return new Vector2(worldPos.x / tileSize - originOffset.x, worldPos.y / tileSize - originOffset.y);
    }

    public Vector2 GraphToWorldPos(Vector2 graphPos)
    {
        return new Vector2((graphPos.x + originOffset.x) * tileSize , (graphPos.y + originOffset.y) * tileSize);
    }

    public Vector2 GetRandomPositionInBounds()
    {
        return new Vector2(Random.Range(0, graphBounds.x), Random.Range(0, graphBounds.y));
    }

    public Classification GetRandomClassification()
    {
        int n = Random.Range(0, 2);
        if (n == 0) return Classification.Red;
        else return Classification.Blue;
    }

    public Classification FindClassification(Vector2 graphPos)
    {
        /*if (graphPos.x + 2*graphPos.y > 8f) return Classification.Blue;
        else return Classification.Red;*/

        if (graphPos.x + graphPos.y > 9f) return Classification.Blue;
        else return Classification.Red;
    }

    /*public void UpdateTexture()
    {
        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                Classification color = NetworkController.instance.network.Evaluate(new float[] { (float)x/textureResolution*100, (float)y/textureResolution*100 });
                if (color == Classification.Red)
                    graphTexture.SetPixel(x, y, Color.red);
                else graphTexture.SetPixel(x, y, Color.blue);
            }
        }
        graphTexture.Apply();
    }*/
}