using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Noise : MonoBehaviour
{
    public ComputeShader shader;
    public int resolution = 256;
    public int numPoints;
    public float speed = 1;
    public float decreaseValue = 0.01f;
    public float turnMultiplier;
    public Texture2D image;
    public bool useImage;
    Texture2D texture;
    ComputeBuffer positionsBuffer;
    static readonly int redsID = Shader.PropertyToID("reds");
    RenderTexture tex;
    RenderTexture imgtex;
    Material mat;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        positionsBuffer = new ComputeBuffer(numPoints, sizeof(float)*4);
        Vector4[] positions = new Vector4[numPoints];
        for(int i = 0; i < numPoints; i++)
        {
            positions[i] = new Vector4(Random.Range(5, resolution), Random.Range(5, resolution), Random.Range(-1f,1f), Random.Range(-1f, 1f));
            //positions[i] = new Vector4(resolution/2, resolution / 2, Mathf.Cos(2* Mathf.PI * (float)i/numPoints), Mathf.Sin(2 * Mathf.PI * (float)i / numPoints));
        }
        positionsBuffer.SetData(positions);
        texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
        int kernelHandle = shader.FindKernel("CSMain");
        shader.SetBuffer(kernelHandle, redsID, positionsBuffer);
        tex = new RenderTexture(resolution, resolution, 24);
        imgtex = new RenderTexture(resolution, resolution, 24);
        tex.enableRandomWrite = true;
        imgtex.enableRandomWrite = true;
        tex.Create();
        imgtex.Create();
        shader.SetTexture(shader.FindKernel("SubtractValues"), "Result", tex);

        shader.SetTexture(kernelHandle, "Result", tex);
        shader.SetTexture(kernelHandle, "ImageTexture", image);
        shader.SetFloat("speed", speed);
        shader.SetFloat("turnMultiplier", turnMultiplier);
        shader.SetFloat("decreaseValue", decreaseValue);
        shader.SetBool("useImage", useImage);
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        mat = gameObject.GetComponent<SpriteRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        RunShaders();
    }
    void RunShaders()
    {
        if (Input.GetMouseButtonDown(2))
        {
            useImage = true;
        }
        int kernelHandle = shader.FindKernel("CSMain");

        shader.SetBool("useImage", useImage);
        shader.Dispatch(kernelHandle, numPoints/128, 1, 1);
        shader.Dispatch(shader.FindKernel("SubtractValues"), resolution/32, resolution/32, 1);
        RenderTexture.active = tex;

        texture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        texture.Apply();
        //gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        mat.mainTexture = texture;
        //byte[] bytes = texture.EncodeToPNG();
        //File.WriteAllBytes("C:/Users/thomas/Desktop/img/" +(count++).ToString()+ ".png", bytes);
    }
}