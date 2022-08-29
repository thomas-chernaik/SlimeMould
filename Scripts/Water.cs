using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public ComputeShader shader;
    public Vector2 centre;
    public float scale;
    public int numPixels;
    float numCollisions;
    ComputeBuffer collisionsBuffer;
    Texture2D texture;
    RenderTexture tex;


    // Start is called before the first frame update
    void Start()
    {
        shader.SetFloat(Shader.PropertyToID("scale"), scale);
        shader.SetVector(Shader.PropertyToID("centre"), centre);
        shader.SetInt(Shader.PropertyToID("numPixels"), numPixels);

        
    }
    private void OnCollisionStay(Collision collision)
    {
        numCollisions = collision.contacts.Length;
        List<Vector3> collisions = new List<Vector3>();
        for(int i = 0; i<numCollisions; i++)
        {
            collisions.Add(collision.contacts[i].point);
        }
        collisionsBuffer.SetData(collisions.ToArray());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void RunShader()
    {
        texture = new Texture2D(numPixels, numPixels, TextureFormat.RGB24, false);
        int kernelHandle = shader.FindKernel("CSMain");
        tex = new RenderTexture(256, 256, 24);
        tex.enableRandomWrite = true;
        tex.Create();
        shader.SetTexture(kernelHandle, "Result", tex);
    }
}
