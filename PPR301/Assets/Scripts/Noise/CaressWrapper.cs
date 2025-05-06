using Caress;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CaressWrapper
{
    private NoiseReducer reducer;

    public void Init(int sampleRate) 
    {
        var config = new NoiseReducerConfig
        {
            SampleRate = sampleRate,
            NumChannels = 1,
            Attenuation = 15,
            Model = RnNoiseModel.Speech
        };
        reducer = new NoiseReducer(config);
    }

    public void ApplyNoiseReduction(float[] buffer)
    {
        reducer?.ReduceNoiseFloat(buffer, 0);
    }

    public void Dispose()
    {
        reducer?.Destroy();
        reducer = null;
    }
}