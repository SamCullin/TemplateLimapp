using UnityEngine;

public class MultibufferParticles : MonoBehaviour
{
	private struct Particle
	{
		public Vector3 position;
		public Vector4 color;
	}

	private const int SIZE_PARTICLE = 28;

	[SerializeField]
	private int particleCount = 10000;
	private int lastParticleCount = 0;

	[SerializeField]
	private float power = 1;

	[SerializeField]
	private bool enableFog = false;
	
	[SerializeField]
	private bool enableShadows = false;
	
	[SerializeField]
	private bool enableColor = false;
	
	[SerializeField]
	private bool enableCutOff = false;



	[SerializeField]
	public Material material;

	[SerializeField]
	public ComputeShader computeShader;

	private int mComputeShaderKernelID;
	private const int WARP_SIZE = 128;
	private int mWarpCount;

	[SerializeField]
	private Transform camPos;

	[SerializeField]
	private float _FOV = 1.0f;


	[SerializeField]
	private float _StepRatio = 1f;
	public float StepRatio { get{return _StepRatio;} set{_StepRatio = value;} }

	[SerializeField]
	private float _Threshold = 1f;
	public float Threshold { get{return _Threshold;} set{_Threshold = value;} }

	[SerializeField]
	private float _jitter = 1f;
	
	[SerializeField]
	private float _fogAmount = 1f;


 	[ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] 
	[SerializeField]
	private Color _Color;

	[SerializeField]
	private int numBuffers = 8;

	[SerializeField]
	private int activeBuffer = 0;


	private Vector4 _RenderParam
	{
		get { return new Vector4(AO1, AO2, AO3, AO4); }
	}

	[SerializeField]
	public float AO1 = 1;

	[SerializeField]
	public float AO2 = 1;

	[SerializeField]
	public float AO3 = 1;


	[SerializeField]
	public float AO4 = 1;



	private ComputeBuffer[] particleBuffers;

	void Start()
	{
		InitBuffers();

	}

	void OnDestroy()
	{
		for(int i = 0; i < particleBuffers.Length; i ++)
		{
			if ( particleBuffers[i] != null)
				particleBuffers[i].Release();
		}
	}
	
	void Update()
	{
		InitBuffers();
		mComputeShaderKernelID = computeShader.FindKernel("CSMain");
		computeShader.GetKernelThreadGroupSizes(mComputeShaderKernelID, out uint xGroupSize, out uint yGroupSize, out uint zGroupSize);
		mWarpCount = Mathf.CeilToInt((float)particleCount / xGroupSize);


		material.SetMatrix("modelToWorld", transform.localToWorldMatrix);
		material.SetColor("_Color", _Color);

		activeBuffer = (activeBuffer + 1) % numBuffers;

		// Send datas to the compute shader
		computeShader.SetFloat("Time", Time.time / 40f);
		computeShader.SetFloat("FOV", _FOV);
		computeShader.SetFloat("pointDim", Mathf.Sqrt(particleCount));
		computeShader.SetFloat("stepRatio", _StepRatio);

		computeShader.SetBool("enableFog", this.enableFog);
		computeShader.SetBool("enableShadows", this.enableShadows);
		computeShader.SetBool("enableColor", this.enableColor);
		computeShader.SetBool("enableCutOff", this.enableCutOff);
		

		computeShader.SetVector("camPos",
		 transform.transform.InverseTransformPoint(camPos.position));

		Quaternion q =  Quaternion.Inverse(transform.rotation) * camPos.rotation;

		computeShader.SetVector("camQRot", new Vector4(q.x, q.y, q.z, q.w));
		computeShader.SetFloat("scale", transform.localToWorldMatrix.GetScale().x );

		computeShader.SetFloat("_Threshold", _Threshold);
		computeShader.SetFloat("_StepRatio", _StepRatio);
		computeShader.SetFloat("_jitter", _jitter);
		computeShader.SetVector("_RenderParam", _RenderParam);
		computeShader.SetFloat("_fogAmount", _fogAmount);
		computeShader.SetFloat("_power", this.power);
		// Update the Particles, only for the ative buffer
 
		computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffers[activeBuffer]);
		computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
	}

	void OnRenderObject()
	{
		// draw ALL the buffers
		for ( int i = 0; i < particleBuffers.Length; i++)
		{
			material.SetPass(0);
			material.SetBuffer("particleBuffer", particleBuffers[i]);
			Graphics.DrawProceduralNow(MeshTopology.Points, 1, particleCount);
		}
	}
	

	void InitBuffers()
    {
		if(particleBuffers == null || particleBuffers.Length != numBuffers || particleCount != lastParticleCount)
        {
			particleBuffers = new ComputeBuffer[numBuffers];
			for (int i = 0; i < numBuffers; i++)
			{
				Particle[] particleArray = new Particle[particleCount];
				particleBuffers[i] = new ComputeBuffer(particleCount, SIZE_PARTICLE);
				particleBuffers[i].SetData(particleArray);
			}
			lastParticleCount = particleCount;
		}
    }
}
