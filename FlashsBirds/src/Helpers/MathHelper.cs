using OnixRuntime.Api.Maths;

namespace FlashsBirds.Helpers;

public static class MathHelper {
	public static float ToRadians(float x) {
		return x * (MathF.PI / 180f);
	}

	public static float ToDegrees(float x) {
		return x * (180f / MathF.PI);
	}

	public static float CatmullRomSpline1D(List<float> points, float time, float alpha, float tension) {
		int i = (int)MathF.Floor(time);

		if (points.Count < i + 4) {
			return points[^1];
		}

		float p1 = points[i];
		float p2 = points[i + 1];
		float p3 = points[i + 2];
		float p4 = points[i + 3];

		float t12 = MathF.Pow(MathF.Abs(p2 - p1), alpha);
		float t23 = MathF.Pow(MathF.Abs(p3 - p2), alpha);
		float t34 = MathF.Pow(MathF.Abs(p4 - p3), alpha);

		const float eps = 1e-4f;
		
		if (t12 < eps) t12 = eps;
		if (t23 < eps) t23 = eps;
		if (t34 < eps) t34 = eps;

		float t = time - MathF.Floor(time);

		float m1 = (1f - tension) * t23 * ((p2 - p1) / t12 - (p3 - p1) / (t12 + t23) + (p3 - p2) / t23);
		float m2 = (1f - tension) * t23 * ((p3 - p2) / t23 - (p4 - p2) / (t23 + t34) + (p4 - p3) / t34);

		float t2 = t * t;
		float t3 = t2 * t;

		float a = 2f * t3 - 3f * t2 + 1f;
		float b = t3 - 2f * t2 + t;
		float c = -2f * t3 + 3f * t2;
		float d = t3 - t2;

		return a * p2 + b * m1 + c * p3 + d * m2;
	}

	public static Vec3 CatmullRomSpline3D(List<Vec3> points, float time, float alpha, float tension) {
		List<float> xPoints = new(points.Count + 1);
		List<float> yPoints = new(points.Count + 1);
		List<float> zPoints = new(points.Count + 1);
		
		// extra point for something (i forgor)
		xPoints.Add(points[0].X);
		yPoints.Add(points[0].Y);
		zPoints.Add(points[0].Z);

		foreach (Vec3 point in points) {
			xPoints.Add(point.X);
			yPoints.Add(point.Y);
			zPoints.Add(point.Z);
		}

		return new Vec3(
			CatmullRomSpline1D(xPoints, time, alpha, tension),
			CatmullRomSpline1D(yPoints, time, alpha, tension),
			CatmullRomSpline1D(zPoints, time, alpha, tension)
		);
	}
}