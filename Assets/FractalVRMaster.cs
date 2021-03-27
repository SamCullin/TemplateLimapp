using UnityEngine;

public struct FractalCamera {
    public Matrix4x4 toWorld;
    public Matrix4x4 inverseProjection;
}

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FractalVRMaster : MonoBehaviour
{

    public ComputeShader fractalShader;
    public Camera leftEye;
    public Camera rightEye;

    [Range(1, 20)]
    public float fractalPower = 10;
    public float darkness = 70;

    [Header("Colour mixing")]
    [Range(0, 1)] public float blackAndWhite;
    [Range(0, 1)] public float redA;
    [Range(0, 1)] public float greenA;
    [Range(0, 1)] public float blueA = 1;
    [Range(0, 1)] public float redB = 1;
    [Range(0, 1)] public float greenB;
    [Range(0, 1)] public float blueB;

    RenderTexture targetLeft;
    RenderTexture targetRight;
    Light directionalLight;

    private FractalCamera[] eyes;
    private ComputeBuffer eyeComputeBuffer;
    private int fractalKernalId;

    [Header("Animation Settings")]
    public float powerIncreaseSpeed = 0.2f;

    FractalCamera createFractalCamera(Camera cam)
    {
        FractalCamera fCam = new FractalCamera();
        fCam.toWorld = this.leftEye.cameraToWorldMatrix;
        fCam.inverseProjection = this.leftEye.projectionMatrix.inverse;
        return fCam;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        directionalLight = FindObjectOfType<Light>();
        fractalKernalId = this.fractalShader.FindKernel("CSMain");


        FractalCamera leftEye = this.createFractalCamera(this.leftEye);
        FractalCamera rightEye = this.createFractalCamera(this.rightEye);

        this.eyes = new FractalCamera[] { leftEye, rightEye };

    }

    void Init()
    {
        InitRenderTexture(ref this.targetLeft, this.leftEye);
        InitRenderTexture(ref this.targetRight, this.rightEye);
        InitEyeBuffer();
    }

    // Animate properties
    void Update()
    {
        if (Application.isPlaying)
        {
            fractalPower += powerIncreaseSpeed * Time.deltaTime;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Init();
        SetParameters();

        this.fractalShader.GetKernelThreadGroupSizes(fractalKernalId, out uint xGroupSize, out uint yGroupSize, out uint zGroupSize);

        int threadGroupsX = Mathf.CeilToInt(this.leftEye.pixelWidth / xGroupSize);
        int threadGroupsY = Mathf.CeilToInt(this.leftEye.pixelHeight / yGroupSize);
        int threadGroupsZ = Mathf.CeilToInt(this.eyes.Length / zGroupSize);
        fractalShader.Dispatch(0, threadGroupsX, threadGroupsY, threadGroupsZ);

        Graphics.Blit(this.targetLeft, destination);
    }

    void SetParameters()
    {
        fractalShader.SetBuffer(0, "eyes", this.eyeComputeBuffer);
        fractalShader.SetTexture(0, "DestinationLeft", this.targetLeft);
        fractalShader.SetTexture(0, "DestinationRight", this.targetRight);
        fractalShader.SetFloat("power", Mathf.Max(fractalPower, 1.01f));
        fractalShader.SetFloat("darkness", darkness);
        fractalShader.SetFloat("blackAndWhite", blackAndWhite);
        fractalShader.SetVector("colourAMix", new Vector3(redA, greenA, blueA));
        fractalShader.SetVector("colourBMix", new Vector3(redB, greenB, blueB));
        fractalShader.SetVector("_LightDirection", directionalLight.transform.forward);

    }

    void InitRenderTexture(ref RenderTexture texture, Camera cam)
    {
        if (texture == null || texture.width != cam.pixelWidth || texture.height != cam.pixelHeight)
        {
            if (texture != null)
            {
                texture.Release();
            }
            texture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            texture.enableRandomWrite = true;
            texture.Create();
        }
    }

    void InitEyeBuffer()
    {
        if (this.eyeComputeBuffer == null || this.eyeComputeBuffer.count != this.eyes.Length)
        {
            if (this.eyeComputeBuffer != null)
            {
                this.eyeComputeBuffer.Release();
            }
            this.eyeComputeBuffer = new ComputeBuffer(this.eyes.Length, sizeof(float) * 2);
            this.eyeComputeBuffer.SetData(this.eyes);
        }
    }


}