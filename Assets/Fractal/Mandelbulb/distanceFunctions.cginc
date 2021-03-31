
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles


// distance estimator for the mandelbulb	
float de_mandelbulb(float3 c)
{
	// i believe that similar to the mandelbrot, the mandelbulb is enclosed in a sphere of radius 2 (hence the delta) 
	const float delta = 2;

	bool converges = true; // unused
	float divergenceIter = 0; // unused
	float3 p = c;
	float dr = 2.0, r = 1.0;

	int ii;
	for (ii = 0; ii < _NumIterations; ii++)
	{
		// equation used: f(p) = p^_Exponent + c starting with p = 0			

		// get polar coordinates of p
		float theta, psi;
		cartesian_to_polar(p, r, theta, psi);

		// rate of change of points in the set
		dr = _Exponent * pow(r, _Exponent - 1) * dr + 1.0;

		// find p ^ _Exponent
		r = pow(r, _Exponent);
		theta *= _Exponent;
		psi *= _Exponent;

		// convert to cartesian coordinates
		polar_to_cartesian(r, theta, psi, p);

		// add c
		p += c;

		// check for divergence
		if (length(p) > delta) {
			divergenceIter = ii;
			converges = false;
			break;
		}
	}
	float dis = log(r) * r / dr;
	// TODO: Return more info about the point 
	// float3x2 = float3x2(float3())

	return dis; // Greens formula
}

float DE(float3 d)
{
	return de_mandelbulb(d);
}