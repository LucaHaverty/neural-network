using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    public static GraphController instance;
    void Awake() { instance = this; }

    public GameObject graphOverlay;

    Sprite graphSprite;
    Texture2D graphTexture;

    Settings settings;


    void Start()
    {
        settings = Settings.instance;
        InitializeOverlayTexture();
    }

    void Update()
    {

    }

    public void InitializeOverlayTexture()
    {
        graphTexture = new Texture2D(settings.textureResolution, settings.textureResolution);

        graphSprite = Sprite.Create(graphTexture, new Rect(Vector2.zero, Vector2.one * settings.textureResolution), Vector2.zero);
        graphOverlay.GetComponent<SpriteRenderer>().sprite = graphSprite;

        // Position sprite
        float scaleFactor = 100f / settings.textureResolution;
        graphOverlay.transform.localScale = new Vector2(scaleFactor * 25.6f, scaleFactor * 25.6f);
        graphOverlay.transform.localPosition = -Vector2.one * 12.778f;

        for (int x = 0; x < settings.textureResolution; x++)
        {
            for (int y = 0; y < settings.textureResolution; y++)
            {
                graphTexture.SetPixel(x, y, Color.red);
            }
        }
        graphTexture.Apply();
    }

    public void UpdateTexture() // Updates texture using network
    {
        for (int x = 0; x < settings.textureResolution; x++)
        {
            for (int y = 0; y < settings.textureResolution; y++)
            {
                Vector2 graphPos = TextureToGraphPos(new Vector2Int(x, y));

                Classification color = NetworkController.instance.network.Evaluate(new float[] { graphPos.x, graphPos.y });
                if (color == Classification.Red)
                    graphTexture.SetPixel(x, y, Color.red);
                else graphTexture.SetPixel(x, y, Color.blue);
            }
        }
        graphTexture.Apply();
    }

    #region Factor Conversions
    public static Vector2 WorldToGraphPos(Vector2 worldPos)
    {
        float conversion = 0.078125f;
        return worldPos * conversion;
    }
    public static Vector2 GraphToWorldPos(Vector2 graphPos)
    {
        float conversion = 12.8f;
        return graphPos * conversion;
    }
    public static Vector2 TextureToGraphPos(Vector2Int texturePos)
    {
        float conversion = (float) 2f / ( (Settings.instance.textureResolution) );
        return new Vector2((texturePos.x-Settings.instance.textureResolution/2f) * conversion, (texturePos.y-Settings.instance.textureResolution / 2f) * conversion);
    }
    #endregion
}

public enum Classification
{
    Red,
    Blue
}