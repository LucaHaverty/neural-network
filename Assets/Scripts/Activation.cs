using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Activation
{
    public static ActivationType activationType = ActivationType.ReLU;
    public enum ActivationType
    {
        None,
        Sigmoid,
        ReLU,
        Step,
        Tanh,
        ELU
    }

    #region Activation Functions
    public static float CalculateActivation(float input)
    {
        switch (activationType)
        {
            case ActivationType.None:
                return input;
            case ActivationType.Sigmoid:
                return Sigmoid(input);
            case ActivationType.ReLU:
                return ReLU(input);
            case ActivationType.Step:
                return Step(input);
            case ActivationType.Tanh:
                return Tanh(input);
            case ActivationType.ELU:
                return ELU(input);
            default: // defaults to Activation.None
                return input;
        }
    }

    public static float Sigmoid(float input)
    {
        return (float)(1.0 / (1.0 + Mathf.Exp(-input)));
    }
    public static float ReLU(float input)
    {
        return Mathf.Max(0, input);
    }
    public static float Step(float input)
    {
        return input >= 0 ? 1 : 0;
    }
    public static float Tanh(float input) // Hyperbolic Tangent
    {
        return (float)System.Math.Tanh(input);
    }
    public static float ELU(float input) // Exponential Linear Unit
    {
        return input >= 0 ? input : Mathf.Exp(input) - 1;
    }
    #endregion
}
