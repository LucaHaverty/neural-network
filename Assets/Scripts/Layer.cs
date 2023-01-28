using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int layerID;
    public int neurons;

    public float[,] weights;
    public float[] biases;
    public float[] activations;

    public float[,] costGradientW;
    public float[] costGradientB;

    public int numNodesIn = 0;
    public int numNodesOut = 0;

    public Layer(int neurons, int layerID, Layer[] layers) // Set weights & biases if not given in constructor
    {
        numNodesOut = neurons;
        if (layerID != 0)
            numNodesIn = layers[layerID - 1].neurons;
        else numNodesIn = 0;

        float[,] weights = new float[numNodesOut, numNodesIn];
        costGradientW = new float[numNodesOut, numNodesIn];

        float[] biases = new float[numNodesOut];
        costGradientB = new float[numNodesOut];

        for (int i = 0; i < neurons; i++)
        {
            for (int j = 0; j < numNodesIn; j++)
            {
                float randomValue = Random.Range(-1f, 1f);
                weights[i, j] = randomValue;
            }
            biases[i] = Random.Range(-1f, 1f)/5f;
        }

        this.weights = weights;
        this.biases = biases;


        activations = new float[numNodesOut];

        this.neurons = neurons;
        this.layerID = layerID;
    }
    public void Evaluate(Layer[] layers)
    {
        for (int i = 0; i < neurons; i++)
        {
            float baseValue = 0;
            for (int j = 0; j < layers[layerID - 1].neurons; j++)
            {
                baseValue += layers[layerID - 1].activations[j] * weights[i, j];
            }
            baseValue += biases[i];

            activations[i] = Activation.CalculateActivation(baseValue);
        }
    }

    public void ApplyGradients(float learnRate)
    {
        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            biases[nodeOut] -= costGradientB[nodeOut] * learnRate;
            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {
                weights[nodeOut, nodeIn] -= costGradientW[nodeOut, nodeIn] * learnRate;
            }
        }
    }
}