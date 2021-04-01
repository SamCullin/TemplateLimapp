using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FractalMaster : MonoBehaviour {

    public ComputeShader fractalShader;

    [Range (1, 20)]
    public float fractalPower = 1;
    public float darkness = 90;
    //public float secondsCounter;

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
        //this.secondsCounter = Time.timeSinceLevelLoad;
    }

    
    void Update () {
        if (Application.isPlaying) {
            fractalPower += powerIncreaseSpeed * Time.deltaTime;

            // Animate properties
            PlayDayNight();
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

    void PlayDayNight()
    {
        float seconds = GameManager.instance.GetSeconds();
        Debug.Log("current second is " + seconds);

        if (seconds <= 10)
        {
            Debug.Log("sunrise starts");
            // mimics the sunrise, greenish vibe when application starts
            if (greenA <= 1)
                this.greenA += 0.005f;

            if (greenA >= 1 && greenB <= 1)
                this.greenB += 0.005f;

        }

        if (seconds > 10 && seconds <= 20)
        {
            Debug.Log("afternoon starts");
            //mimics the afternoon, incorporating yellowish vibe
            if (greenB >= 1 && blueA >= 0.5)
                this.blueA -= 0.005f;
        }

        if (seconds > 20 && seconds <= 30)
        {
            Debug.Log("sunset starts");
            //mimics the late afternoon (sunset)
            if (blueA <= 0.5 && redA <= 1)
                this.redA += 0.005f;
        }


        if (seconds > 30 && seconds <= 40)
        {
            Debug.Log("evening starts");
            // mimics the evening
            if (blueB <= 1)
                this.blueB += 0.005f;

            if (blueB >= 1 && redA >= 0.5)
                this.redA -= 0.005f;
        }

        if (seconds > 40 && seconds <= 50)
        {
            Debug.Log("late evening starts");
            // mimics the late night
            if (redA >= 0.15)
                this.redA -= 0.005f;

            if (redA <= 0.15 && blueA <= 1)
                this.blueA += 0.005f;

        }

        if (seconds > 50 && seconds <= 60)
        {
            Debug.Log("dawn starts");
            // mimics the dawn;
            if (redA <= 1)
                this.redA += 0.005f;

            if (redA >= 1 && greenA >= 0)
                this.greenA -= 0.005f;

        }
    }
}