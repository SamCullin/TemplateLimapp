using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FractalMaster : MonoBehaviour {

    public ComputeShader fractalShader;

    [Range (1, 20)]
    // public float fractalPower = 10;
    // so that it starts from 
    public float fractalPower = 1;
    public float darkness = 90;

    [Header ("Colour mixing")]
    [Range (0, 1)] public float blackAndWhite;
    [Range (0, 1)] public float redA;
    [Range (0, 1)] public float greenA = 0f;
    [Range (0, 1)] public float blueA = 1f;
    [Range (0, 1)] public float redB = 1f;
    [Range (0, 1)] public float greenB;
    [Range (0, 1)] public float blueB;

    RenderTexture target;
    Camera cam;
    Light directionalLight;

    [Header ("Animation Settings")]
    public float powerIncreaseSpeed = 0.02f;

    void Start() {
        Application.targetFrameRate = 60;
    }
    
    void Init () {
        cam = Camera.current;
        directionalLight = FindObjectOfType<Light> ();
    }

    // Animate properties
    void Update () {
        if (Application.isPlaying) {
            fractalPower += powerIncreaseSpeed * Time.deltaTime;

            // mimics the sunrise, greenish vibe when application starts
            if (greenA <= 1)
                this.greenA += 0.001f;

            if (greenA >= 1 && greenB <= 1)
                this.greenB += 0.001f;

            //mimics the afternoon, incorporating yellowish vibe
            if (greenB >= 1 && blueA >= 0.5)
                this.blueA -= 0.0005f;

            //mimics the late afternoon (sunset)
            if (blueA <= 0.5 && redA <= 1)
                this.redA += 0.001f;
        }
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Init ();
        InitRenderTexture ();
        SetParameters ();

        int threadGroupsX = Mathf.CeilToInt (cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt (cam.pixelHeight / 8.0f);
        fractalShader.Dispatch (0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit (target, destination);
    }

    void SetParameters () {
        fractalShader.SetTexture (0, "Destination", target);
        fractalShader.SetFloat ("power", Mathf.Max (fractalPower, 1.01f));
        fractalShader.SetFloat ("darkness", darkness);
        fractalShader.SetFloat ("blackAndWhite", blackAndWhite);
        fractalShader.SetVector ("colourAMix", new Vector3 (redA, greenA, blueA));
        fractalShader.SetVector ("colourBMix", new Vector3 (redB, greenB, blueB));

        fractalShader.SetMatrix ("_CameraToWorld", cam.cameraToWorldMatrix);
        fractalShader.SetMatrix ("_CameraInverseProjection", cam.projectionMatrix.inverse);
        fractalShader.SetVector ("_LightDirection", directionalLight.transform.forward);

    }

    void InitRenderTexture () {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight) {
            if (target != null) {
                target.Release ();
            }
            target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }
    }

}