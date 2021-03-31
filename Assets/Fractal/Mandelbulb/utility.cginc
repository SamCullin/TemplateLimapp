



/********* math stuff ******************/
inline float2
complex_mult(float2 c1, float2 c2)
{
	return float2(c1.x * c2.x - c1.y * c2.y, c1.x * c2.y + c1.y * c2.x);
}

float2
recurse_complex_mult(float2 cin, int n)
{
	int ii;
	float2 cout = cin;

	for (ii = 0; ii < n; ii++)
	{
		cout = complex_mult(cout, cin);
	}

	return cout;
}


// theta -- angle vector makes with the XY plane
// psi -- angle the projected vector on the XY plane makes with the X axis
// note: this might not be the usual convention
inline void
cartesian_to_polar(float3 p, out float r, out float theta, out float psi)
{
	r = length(p);
	float r1 = p.x * p.x + p.y * p.y;
	theta = atan(p.z / r1); // angle vector makes with the XY plane
	psi = atan(p.y / p.x); // angle of xy-projected vector with X axis
}


// theta -- angle vector makes with the XY plane
// psi -- angle the projected vector on the XY plane makes with the X axis
// note: this might not be the usual convention	
inline void
polar_to_cartesian(float r, float theta, float psi, out float3 p)
{
	p.x = r * cos(theta) * cos(psi);
	p.y = r * cos(theta) * sin(psi);
	p.z = r * sin(theta);
}


// [-1,1] to [0,1]
float
norm_to_unorm(float i)
{
	return (i + 1) * 0.5;
}



/*************************************** distance estimators ******************************/
// distance to the surface of a sphere
float
de_sphere_surface(float3 p, float3 c, float r)
{
	return abs(length(p - c) - r);
}


// instancing can be done really cheap. the function below creates infinite spheres on the XY plane
float
de_sphere_instances(float3 p)
{
	p.xy = fmod((p.xy), 1.0) - 0.5;
	return length(p) - 0.2;
}