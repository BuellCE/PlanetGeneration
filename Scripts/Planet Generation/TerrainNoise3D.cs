using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainNoise3D {

    public float noiseSpacing;
    public float heightMultiplier;

    Noise noise = new Noise();

    public TerrainNoise3D(float noiseSpacing, float heightMultiplier) {
        this.noiseSpacing = noiseSpacing;
        this.heightMultiplier = heightMultiplier;
    }

    public float GetHeight(float x, float y, float z) {
        return Perlin3D(x * noiseSpacing, y * noiseSpacing, z * noiseSpacing) * heightMultiplier;
    }

    private float Perlin3D(float x, float y, float z) {
        noise = new Noise();
        return (noise.Evaluate(new Vector3(x, y, z)) + 1) / 2f ;
    }



}
