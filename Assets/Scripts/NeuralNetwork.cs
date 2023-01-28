using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NeuralNetwork
{
    Layer[] layers;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new Layer[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = new Layer(layers[i], i, this.layers);
        }
    }

    public Classification Evaluate(float[] inputs)
    {
        layers[0].activations = inputs;

        // Evaluate every layer except input layer
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].Evaluate(layers);
        }

        if (layers[layers.Length - 1].activations[0] > layers[layers.Length - 1].activations[1])
            return Classification.Red;
        else return Classification.Blue;
    }

    public void Learn(DataPoint[] trainingData, float learnRate)
    {
        const float h = 0.01f;
        float originalCost = AverageCost(trainingData);
        
        NetworkController.instance.OnNetworkLearn.Invoke(originalCost);

        foreach (Layer layer in layers)
        {
            // Calculate cost gradient for weights
            for (int nodeIn = 0; nodeIn < layer.numNodesIn; nodeIn++)
            {
                for (int nodeOut = 0; nodeOut < layer.numNodesOut; nodeOut++)
                {
                    layer.weights[nodeOut, nodeIn] += h;
                    float deltaCost = AverageCost(trainingData) - originalCost;
                    layer.weights[nodeOut, nodeIn] -= h;
                    layer.costGradientW[nodeOut, nodeIn] = deltaCost / h;
                }
            }

            // Calculate cost gradients for biases
            for (int nodeOut = 0; nodeOut < layer.numNodesOut; nodeOut++)
            {
                layer.biases[nodeOut] += h;
                float deltaCost = AverageCost(trainingData) - originalCost;
                layer.biases[nodeOut] -= h;
                layer.costGradientB[nodeOut] = deltaCost / h;
            }
            ApplyAllGradients(learnRate);
        }
        //Debug.Log();

    }

    public void ApplyAllGradients(float learnRate)
    {
        foreach (Layer layer in layers)
        {
            layer.ApplyGradients(learnRate);
        }
    }

    public void UpdateWeights(float[,] weights_1, float[] biases_1, float[,] weights_2, float[] biases_2)
    {
        layers[1].weights = weights_1;
        layers[1].biases = biases_1;

        layers[2].weights = weights_2;
        layers[2].biases = biases_2;
    }

    public void GetWeights(System.Action<float[,], float[], float[,], float[]> callback)
    {
        callback(layers[1].weights, layers[1].biases, layers[2].weights, layers[2].biases);
    } 

    public void UpdateWeights(float[,] weights_1, float[] biases_1)
    {
        layers[1].weights = weights_1;
        layers[1].biases = biases_1;
    }

    #region Cost Functions
    public float Cost(DataPoint dataPoint) // Cost for a single datapoint
    {
        //Debug.Log(dataPoint.graphPos.x);
        Evaluate(new float[] { dataPoint.graphPos.x, dataPoint.graphPos.y });

        // Expected values for output layer based on what color dataPoint is
        float[] expectedOutput;
        if (dataPoint.classification == Classification.Red) expectedOutput = new float[] { 1f, 0f };
        else expectedOutput = new float[] { 0f, 1f };

        // Sum of costs for each activation in output layer
        float cost = NodeCost(layers[layers.Length - 1].activations[0], expectedOutput[0])
            + NodeCost(layers[layers.Length - 1].activations[1], expectedOutput[1]);

        //Debug.Log(expectedOutput[0] + " " + expectedOutput[1] + " " + layers[layers.Length - 1].activations[0] + " " + layers[layers.Length - 1].activations[1]);
        //Debug.Log(NodeCost(layers[layers.Length - 1].activations[0], expectedOutput[0]) + " " + NodeCost(layers[layers.Length - 1].activations[1], expectedOutput[1]));

        return cost;
    }
    public float AverageCost(DataPoint[] dataPoints) // Average cost for an array of dataPoints
    {
        float averageCost = 0;
        foreach (DataPoint dataPoint in dataPoints)
        {
            averageCost += Cost(dataPoint);
        }
        return averageCost / dataPoints.Length;
    }

    public float NodeCost(float actualValue, float expectedValue) // Cost of a single node output
    {
        float error = actualValue - expectedValue;
        return error * error; // Squares error to emphasize correcting large differences
    }
    #endregion
}


public class DataPoint
{
    public Vector2 graphPos;
    public Classification classification;
    public GameObject gameObject;

    public DataPoint(Vector2 graphPos, Classification classification, GameObject gameObject)
    {
        this.graphPos = graphPos;
        this.classification = classification;
        this.gameObject = gameObject;
    }
}
