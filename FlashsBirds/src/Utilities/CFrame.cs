using OnixRuntime.Api.Maths;

namespace FlashsBirds.Utilities {
	public class CFrame {
		private readonly float m11 = 1, m12 = 0, m13 = 0, m14 = 0;
		private readonly float m21 = 0, m22 = 1, m23 = 0, m24 = 0;
		private readonly float m31 = 0, m32 = 0, m33 = 1, m34 = 0;
		private const float m41 = 0, m42 = 0, m43 = 0, m44 = 1;

		public readonly float X = 0, Y = 0, Z = 0;
		public readonly Vec3 Position;
		public readonly Vec3 LookVector;
		public readonly Vec3 RightVector;
		public readonly Vec3 UpVector;

		private static readonly Vec3 Right = new(1, 0, 0);
		private static readonly Vec3 Up = new(0, 1, 0);
		private static readonly Vec3 Back = new(0, 0, 1);

		// Constructors //

		public CFrame() {
			m14 = 0;
			m24 = 0;
			m34 = 0;

			X = m14;
			Y = m24;
			Z = m34;

			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}
		
		public CFrame(Vec3 pos) {
			m14 = pos.X;
			m24 = pos.Y;
			m34 = pos.Z;

			X = m14;
			Y = m24;
			Z = m34;

			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}

		public CFrame(Vec3 eye, Vec3 look) {
			Vec3 upCandidate = Up;
			Vec3 zAxis = (eye - look).Normalized;
			Vec3 xAxis = upCandidate.Cross(zAxis);
			
			if (xAxis.Length < 1e-6f) {
				upCandidate = MathF.Abs(zAxis.X) < 0.999f ? new Vec3(1, 0, 0) : new Vec3(0, 1, 0);
				xAxis = upCandidate.Cross(zAxis);
			}

			xAxis = xAxis.Normalized;
			Vec3 yAxis = zAxis.Cross(xAxis).Normalized;

			m11 = xAxis.X;
			m12 = yAxis.X;
			m13 = zAxis.X;
			m14 = eye.X;
			m21 = xAxis.Y;
			m22 = yAxis.Y;
			m23 = zAxis.Y;
			m24 = eye.Y;
			m31 = xAxis.Z;
			m32 = yAxis.Z;
			m33 = zAxis.Z;
			m34 = eye.Z;

			X = m14;
			Y = m24;
			Z = m34;
			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}

		public CFrame(float x = 0, float y = 0, float z = 0) {
			m14 = x;
			m24 = y;
			m34 = z;

			X = m14;
			Y = m24;
			Z = m34;

			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}

		public CFrame(float x, float y, float z, float i, float j, float k, float w) {
			m14 = x;
			m24 = y;
			m34 = z;
			m11 = 1 - 2 * MathF.Pow(j, 2) - 2 * MathF.Pow(k, 2);
			m12 = 2 * (i * j - k * w);
			m13 = 2 * (i * k + j * w);
			m21 = 2 * (i * j + k * w);
			m22 = 1 - 2 * MathF.Pow(i, 2) - 2 * MathF.Pow(k, 2);
			m23 = 2 * (j * k - i * w);
			m31 = 2 * (i * k - j * w);
			m32 = 2 * (j * k + i * w);
			m33 = 1 - 2 * MathF.Pow(i, 2) - 2 * MathF.Pow(j, 2);

			X = m14;
			Y = m24;
			Z = m34;

			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}

		public CFrame(float n14, float n24, float n34, float n11, float n12, float n13, float n21, float n22, float n23, float n31, float n32, float n33) {
			m14 = n14;
			m24 = n24;
			m34 = n34;
			m11 = n11;
			m12 = n12;
			m13 = n13;
			m21 = n21;
			m22 = n22;
			m23 = n23;
			m31 = n31;
			m32 = n32;
			m33 = n33;

			X = m14;
			Y = m24;
			Z = m34;

			Position = new Vec3(m14, m24, m34);
			LookVector = new Vec3(-m13, -m23, -m33);
			RightVector = new Vec3(m11, m21, m31);
			UpVector = new Vec3(m12, m22, m32);
		}

		// Operators //

		public static Vec3 operator *(CFrame a, Vec3 b) {
			Vec3 right = new(a.m11, a.m21, a.m31);
			Vec3 up = new(a.m12, a.m22, a.m32);
			Vec3 back = new(a.m13, a.m23, a.m33);
			return new Vec3(a.m14, a.m24, a.m34) + b.X * right + b.Y * up + b.Z * back;
		}
		
		public static CFrame operator -(CFrame a, Vec3 b) {
			return new CFrame(a.m14 - b.X, a.m24 - b.Y, a.m34 - b.Z, a.m11, a.m12, a.m13, a.m21, a.m22, a.m23, a.m31, a.m32, a.m33);
		}
		
		public static CFrame operator +(CFrame a, Vec3 b) {
			return new CFrame(a.m14 + b.X, a.m24 + b.Y, a.m34 + b.Z, a.m11, a.m12, a.m13, a.m21, a.m22, a.m23, a.m31, a.m32, a.m33);
		}

		public static CFrame operator *(CFrame a, CFrame b) {
			float r11 = a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
			float r12 = a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
			float r13 = a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;

			float r21 = a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
			float r22 = a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
			float r23 = a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;

			float r31 = a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
			float r32 = a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
			float r33 = a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;
			
			float tx = a.m11 * b.m14 + a.m12 * b.m24 + a.m13 * b.m34 + a.m14;
			float ty = a.m21 * b.m14 + a.m22 * b.m24 + a.m23 * b.m34 + a.m24;
			float tz = a.m31 * b.m14 + a.m32 * b.m24 + a.m33 * b.m34 + a.m34;

			return new CFrame(tx, ty, tz, r11, r12, r13, r21, r22, r23, r31, r32, r33);
		}

		// Private Static Functions //

		private static Vec3 VectorAxisAngle(Vec3 n, Vec3 v, float t) {
			Vec3 normalized = n.Normalized;
			return v * MathF.Cos(t) + v.Dot(normalized) * normalized * (1 - MathF.Cos(t)) + normalized.Cross(v) * MathF.Sin(t);
		}

		private static (Vec3, float) CFrameAxisAngle(CFrame a) {
			float[] ac = a.GetComponents();
			float mx = ac[0], my = ac[1], mz = ac[2], m11 = ac[3], m12 = ac[4], m13 = ac[5], m21 = ac[6], m22 = ac[7], m23 = ac[8], m31 = ac[9], m32 = ac[10], m33 = ac[11];

			float trace = m11 + m22 + m33;
			float angle = MathF.Acos(Math.Clamp((trace - 1f) / 2f, -1f, 1f));

			if (Math.Abs(angle) < 1e-6f) {
				return (new Vec3(1, 0, 0), 0f);
			}

			float sinTheta = MathF.Sin(angle);

			if (Math.Abs(sinTheta) < 1e-6f) {
				float x = MathF.Sqrt(Math.Max(0, (m11 + 1) / 2));
				float y = MathF.Sqrt(Math.Max(0, (m22 + 1) / 2));
				float z = MathF.Sqrt(Math.Max(0, (m33 + 1) / 2));
				
				if (m12 < 0) y = -y;
				if (m13 < 0) z = -z;

				return (new Vec3(x, y, z), angle);
			}
			
			return ((new Vec3((m32 - m23) / (2 * sinTheta), (m13 - m31) / (2 * sinTheta), (m21 - m12) / (2 * sinTheta))).Normalized, angle);
		}

		private static float GetDeterminant(CFrame a) {
			float[] ac = a.GetComponents();
			float a14 = ac[0], a24 = ac[1], a34 = ac[2], a11 = ac[3], a12 = ac[4], a13 = ac[5], a21 = ac[6], a22 = ac[7], a23 = ac[8], a31 = ac[9], a32 = ac[10], a33 = ac[11];
			float det = (a11 * a22 * a33 * m44 + a11 * a23 * a34 * m42 + a11 * a24 * a32 * m43
			             + a12 * a21 * a34 * m43 + a12 * a23 * a31 * m44 + a12 * a24 * a33 * m41
			             + a13 * a21 * a32 * m44 + a13 * a22 * a34 * m41 + a13 * a24 * a31 * m42
			             + a14 * a21 * a33 * m42 + a14 * a22 * a31 * m43 + a14 * a23 * a32 * m41
			             - a11 * a22 * a34 * m43 - a11 * a23 * a32 * m44 - a11 * a24 * a33 * m42
			             - a12 * a21 * a33 * m44 - a12 * a23 * a34 * m41 - a12 * a24 * a31 * m43
			             - a13 * a21 * a34 * m42 - a13 * a22 * a31 * m44 - a13 * a24 * a32 * m41
			             - a14 * a21 * a32 * m43 - a14 * a22 * a33 * m41 - a14 * a23 * a31 * m42);
			return det;
		}

		private static CFrame Invert4x4(CFrame a) {
			float a14 = a.m14, a24 = a.m24, a34 = a.m34;
			float a11 = a.m11, a12 = a.m12, a13 = a.m13;
			float a21 = a.m21, a22 = a.m22, a23 = a.m23;
			float a31 = a.m31, a32 = a.m32, a33 = a.m33;
			
			float r11 = a11, r12 = a21, r13 = a31;
			float r21 = a12, r22 = a22, r23 = a32;
			float r31 = a13, r32 = a23, r33 = a33;
			
			float x = -(r11 * a14 + r12 * a24 + r13 * a34);
			float y = -(r21 * a14 + r22 * a24 + r23 * a34);
			float z = -(r31 * a14 + r32 * a24 + r33 * a34);

			return new CFrame(x, y, z, r11, r12, r13, r21, r22, r23, r31, r32, r33);
		}

		private static float[] QuaternionFromCFrame(CFrame a) {
			float[] ac = a.GetComponents();
			float mx = ac[0], my = ac[1], mz = ac[2], m11 = ac[3], m12 = ac[4], m13 = ac[5], m21 = ac[6], m22 = ac[7], m23 = ac[8], m31 = ac[9], m32 = ac[10], m33 = ac[11];
			float trace = m11 + m22 + m33;
			float w = 1, i = 0, j = 0, k = 0;

			if (trace > 0) {
				float s = MathF.Sqrt(1 + trace);
				float r = 0.5f / s;
				w = s * 0.5f;
				i = (m32 - m23) * r;
				j = (m13 - m31) * r;
				k = (m21 - m12) * r;
			} else {
				float big = Math.Max(Math.Max(m11, m22), m33);
				if (big == m11) {
					float s = MathF.Sqrt(1 + m11 - m22 - m33);
					float r = 0.5f / s;
					w = (m32 - m23) * r;
					i = 0.5f * s;
					j = (m21 + m12) * r;
					k = (m13 + m31) * r;
				} else if (big == m22) {
					float s = MathF.Sqrt(1 - m11 + m22 - m33);
					float r = 0.5f / s;
					w = (m13 - m31) * r;
					i = (m21 + m12) * r;
					j = 0.5f * s;
					k = (m32 + m23) * r;
				} else if (big == m33) {
					float s = MathF.Sqrt(1 - m11 - m22 + m33);
					float r = 0.5f / s;
					w = (m21 - m12) * r;
					i = (m13 + m31) * r;
					j = (m32 + m23) * r;
					k = 0.5f * s;
				}
			}

			return [w, i, j, k];
		}

		private static CFrame LerpInternal(CFrame a, CFrame b, float t) {
			Quaternion qa = a.ToQuaternion();
			Quaternion qb = b.ToQuaternion();
			Quaternion qm = Quaternion.Slerp(qa, qb, t);

			Vec3 pm = a.Position.Lerp(b.Position, t);
			return new CFrame(pm.X, pm.Y, pm.Z, qm.X, qm.Y, qm.Z, qm.W);
		}

		// Public Static Functions //

		public static CFrame Identity() {
			return new CFrame(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0
			);
		}

		public static CFrame FromAxisAngle(Vec3 axis, float theta) {
			Vec3 r = VectorAxisAngle(axis, Right, theta);
			Vec3 u = VectorAxisAngle(axis, Up, theta);
			Vec3 b = VectorAxisAngle(axis, Back, theta);
			return new CFrame(0, 0, 0, r.X, u.X, b.X, r.Y, u.Y, b.Y, r.Z, u.Z, b.Z);
		}

		public static CFrame Angles(float x, float y, float z) {
			CFrame cfx = FromAxisAngle(Right, x);
			CFrame cfy = FromAxisAngle(Up, y);
			CFrame cfz = FromAxisAngle(Back, z);
			return cfx * cfy * cfz;
		}

		public static CFrame FromEulerAnglesXYZ(float x, float y, float z) {
			return Angles(x, y, z);
		}

		public static CFrame FromEulerAnglesYXZ(float x, float y, float z) {
			CFrame cfx = FromAxisAngle(Right, x);
			CFrame cfy = FromAxisAngle(Up, y);
			CFrame cfz = FromAxisAngle(Back, z);
			return cfy * cfx * cfz;
		}

		public static CFrame FromEulerAnglesZYX(float x, float y, float z) {
			CFrame cfx = FromAxisAngle(Right, x);
			CFrame cfy = FromAxisAngle(Up, y);
			CFrame cfz = FromAxisAngle(Back, z);
			
			return cfz * cfy * cfx;
		}

		public static CFrame FromRotationBetweenVectors(Vec3 from, Vec3 to) {
			Vec3 f = from.Normalized;
			Vec3 t = to.Normalized;
			float cos = f.Dot(t);
			
			if (cos > 0.9999f) {
				return new CFrame();
			}

			if (cos < -0.9999f) {
				Vec3 ortho = new Vec3(1, 0, 0).Cross(f);
				if (ortho.Length < 1e-6f) ortho = new Vec3(0, 1, 0).Cross(f);
				ortho = ortho.Normalized;
				return FromAxisAngle(ortho, MathF.PI);
			}

			Vec3 axis = f.Cross(t);
			float s = MathF.Sqrt((1 + cos) * 2.0f);
			float invs = 1.0f / s;
			
			float qx = axis.X * invs;
			float qy = axis.Y * invs;
			float qz = axis.Z * invs;
			float qw = 0.5f * s;
			return new CFrame(0, 0, 0, qx, qy, qz, qw);
		}

		public static CFrame LookAt(Vec3 at, Vec3 look) {
			return new CFrame(at, look);
		}

		public static CFrame LookAlong(Vec3 at, Vec3 direction) {
			return LookAt(at, at + direction);
		}

		// Methods //

		public override string ToString() {
			return string.Join(", ", GetComponents());
		}

		public CFrame Inverse() {
			return Invert4x4(this);
		}

		public CFrame Lerp(CFrame cf2, float t) {
			return LerpInternal(this, cf2, t);
		}

		public CFrame ToWorldSpace(CFrame cf2) {
			return this * cf2;
		}

		public CFrame ToObjectSpace(CFrame cf2) {
			return this.Inverse() * cf2;
		}

		public Vec3 PointToWorldSpace(Vec3 v) {
			return this * v;
		}

		public Vec3 PointToObjectSpace(Vec3 v) {
			return this.Inverse() * v;
		}

		public Vec3 VectorToWorldSpace(Vec3 v) {
			return (this - this.Position) * v;
		}

		public Vec3 VectorToObjectSpace(Vec3 v) {
			return (this - this.Position).Inverse() * v;
		}

		public Vec3 ToEulerAnglesXYZ() {
			float x, y, z;

			if (m13 < 1f) {
				if (m13 > -1f) {
					x = MathF.Atan2(-m23, m33);
					y = MathF.Asin(m13);
					z = MathF.Atan2(-m12, m11);
				} else {
					x = (float)-Math.Atan2(m21, m22);
					y = -(MathF.PI / 2);
					z = 0f;
				}
			} else {
				x = MathF.Atan2(m21, m22);
				y = MathF.PI / 2;
				z = 0f;
			}

			return new Vec3(x, y, z);
		}

		public Vec3 ToEulerAnglesYXZ() {
			float x, y, z;

			if (m23 < 1f) {
				if (m23 > -1f) {
					y = MathF.Atan2(m13, m33);
					x = MathF.Asin(-m23);
					z = MathF.Atan2(m21, m22);
				} else {
					y = MathF.Atan2(m12, m11);
					x = MathF.PI / 2;
					z = 0f;
				}
			} else {
				y = MathF.Atan2(-m12, m11);
				x = -(MathF.PI / 2);
				z = 0f;
			}

			return new Vec3(x, y, z);
		}

		public (Vec3, float) ToAxisAngle() {
			return CFrameAxisAngle(this);
		}

		public bool FuzzyEq(CFrame b, float epsilon = 1e-5f) {
			float[] bc = b.GetComponents();
			float b14 = bc[0], b24 = bc[1], b34 = bc[2], b11 = bc[3], b12 = bc[4], b13 = bc[5], b21 = bc[6], b22 = bc[7], b23 = bc[8], b31 = bc[9], b32 = bc[10], b33 = bc[11];

			return Math.Abs(m11 - b11) < epsilon &&
			       Math.Abs(m12 - b12) < epsilon &&
			       Math.Abs(m13 - b13) < epsilon &&
			       Math.Abs(m14 - b14) < epsilon &&
			       Math.Abs(m21 - b21) < epsilon &&
			       Math.Abs(m22 - b22) < epsilon &&
			       Math.Abs(m23 - b23) < epsilon &&
			       Math.Abs(m24 - b24) < epsilon &&
			       Math.Abs(m31 - b31) < epsilon &&
			       Math.Abs(m32 - b32) < epsilon &&
			       Math.Abs(m33 - b33) < epsilon &&
			       Math.Abs(m34 - b34) < epsilon;
		}

		public CFrame Orthonormalize() {
			Vec3 forward = LookVector.Normalized;
			Vec3 up = (UpVector - UpVector.Dot(forward) * forward).Normalized;
			Vec3 right = up.Cross(forward);

			return new CFrame(
				right.X, right.Y, right.Z, m14,
				up.X, up.Y, up.Z, m24,
				forward.X, forward.Y, forward.Z, m34
			);
		}

		public float[] GetComponents() {
			return [m14, m24, m34, m11, m12, m13, m21, m22, m23, m31, m32, m33];
		}

		public Quaternion ToQuaternion() {
			float[] quat = QuaternionFromCFrame(this);
			
			float w = quat[0], i = quat[1], j = quat[2], k = quat[3];
			float magnitude = MathF.Sqrt(w * w + i * i + j * j + k * k);

			if (magnitude > 0.0001f) {
				w /= magnitude;
				i /= magnitude;
				j /= magnitude;
				k /= magnitude;
			}

			return new Quaternion(i, j, k, w);
		}

		public TransformationMatrix ToTransformMatrix(Vec3 size) {
			return TransformationMatrix.Scale(size) * ToQuaternion() * TransformationMatrix.TranslateWorldPosition(Position);
		}

		public CFrame Clone() {
			float[] ac = GetComponents();
			float mx = ac[0], my = ac[1], mz = ac[2], m11 = ac[3], m12 = ac[4], m13 = ac[5], m21 = ac[6], m22 = ac[7], m23 = ac[8], m31 = ac[9], m32 = ac[10], m33 = ac[11];
			return new CFrame(mx, my, mz, m11, m12, m13, m21, m22, m23, m31, m32, m33);
		}
	}
}