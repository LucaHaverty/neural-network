using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkOld
{
    /*
    public static Activation activation = Activation.Sigmoid;

    
    Layer[] layers;

    public NeuralNetworkOld(int[] layers)
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

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].Evaluate(layers);
        }

        if (layers[layers.Length-1].activations[0] > layers[layers.Length - 1].activations[1])
            return Classification.Red;
        else return Classification.Blue;
    }

    public void UpdateWeights(float[,] weights_1, float[] biases_1, float[,] weights_2, float[] biases_2)
    {
        layers[1].weights = weights_1;
        layers[1].biases = biases_1;

        layers[2].weights = weights_2;
        layers[2].biases = biases_2;
    }

    public float Cost(DataPoint dataPoint) // Cost for a single datapoint
    {
        Evaluate(new float[] { dataPoint.graphPos.x, dataPoint.graphPos.y });

        // Expected values for output layer based on what color dataPoint is
        float[] expectedOutput;
        if (dataPoint.classification == Classification.Red) expectedOutput = new float[] { 1f, 0f };
        else expectedOutput = new float[] { 0f, 1f };

        // Sum of costs for each activation in output layer
        float cost = NodeCost(layers[layers.Length - 1].activations[0], expectedOutput[0])
            + NodeCost(layers[layers.Length - 1].activations[1], expectedOutput[1]);

        return cost;
    }
    public float AverageCost(DataPoint[] dataPoints) // Average cost for an array of datapoints
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

    public void Learn(DataPoint[] trainingData, float learnRate)
    {
        const float h = 0.0001f;
        float originalCost = AverageCost(trainingData);

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

            for (int nodeOut = 0; nodeOut < layer.numNodesOut; nodeOut++)
            {
                layer.biases[nodeOut] += h;
                float deltaCost = AverageCost(trainingData) - originalCost;
                layer.biases[nodeOut] -= h;
                layer.costGradientB[nodeOut] = deltaCost / h;
            }
            ApplyAllGradients(learnRate);
        }
    }

    public void ApplyAllGradients(float learnRate)
    {
        foreach(Layer layer in layers) {
            layer.ApplyGradients(learnRate);
        }
    }

    public static float ActivationFunction(float input)
    {
        switch(activation)
        {
            case Activation.None:
                return input;
            case Activation.Sigmoid:
                return Sigmoid(input);
            case Activation.ReLU:
                return RelU(input);
            default: // defaults to Activation.None
                return input;
        }
    }
    public static float Sigmoid(float input)
    {
        return (float)(1.0 / (1.0 + Mathf.Pow(2.71828f, -input)));
    }
    public static float RelU(float input)
    {
        return Mathf.Max(0, input);
    }

    public static float NormalizedRandom()
    {
        float u, v, s;
        do
        {
            u = Random.Range(-1f, 1f);
            v = Random.Range(-1f, 1f);
            s = u * u + v * v;
        } while (s > 1 || s == 0);
        s = Mathf.Sqrt(-2f * Mathf.Log(s, Mathf.Exp(1f)) / s);
        return u * s;
    }
    */
}