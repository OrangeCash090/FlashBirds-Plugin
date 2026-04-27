using FlashsBirds.Utilities;
using FlashsBirds.Helpers;
using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;

namespace FlashsBirds.Classes;

public class Bird {
	private CFrame _orientation = new();
	
	public readonly ModelPart Body;
	public readonly ModelPart LeftWing;
	public readonly ModelPart RightWing;

	public readonly ModelWeld LeftShoulder;
	public readonly ModelWeld RightShoulder;

	public Vec3 Position;
	public Vec3 Velocity;
	public Vec3 Acceleration;

	public readonly float SpawnTime;
	public bool ShouldRemove;

	private const float TimePerKeyframe = 5f;
	private const float KeyframeSpacing = 40f;
	private const float MaxTurnAngle = 90f;
	private const float FlyHeight = 30f;
	private const float DisappearDist = 60f;
	private const float HardRemoveDist = 200f;
	private const float MaxAttempts = 10f;

	public readonly List<Vec3> Path;

	public Bird(Vec3 spawnPosition) {
		Body = new ModelPart {
			MeshPath = "body.obj",
			Texture = TexturePath.Assets("texture.png")
		};
		
		LeftWing = new ModelPart {
			MeshPath = "wingL.obj",
			Texture = TexturePath.Assets("texture.png")
		};
		
		RightWing = new ModelPart {
			MeshPath = "wingR.obj",
			Texture = TexturePath.Assets("texture.png")
		};

		LeftShoulder = new ModelWeld(Body, LeftWing, new CFrame(0.125f / 2f, 0f, 0f), new CFrame());
		RightShoulder = new ModelWeld(Body, RightWing, new CFrame(-0.125f / 2f, 0f, 0f), new CFrame());

		Position = spawnPosition;
		Path = [spawnPosition];
		SpawnTime = GameTimer.ElapsedTime;
	}

	public void Update(RendererWorld gfx, float dt) {
		if (ShouldRemove) return;

		if (!GeneratePath()) {
			ShouldRemove = true;
			return;
		}

		ComputePhysics(dt);

		float speed = Velocity.Length;
		
		if (speed > 0.001f) {
			Vec3 forward = Velocity / speed;
			
			Vec3 localAccel = _orientation.VectorToObjectSpace(Acceleration);
			float roll = Math.Clamp(localAccel.X / 200f, -MathF.PI / 3f, MathF.PI / 3f);
			
			CFrame target = CFrame.LookAlong(Vec3.Zero, forward)
			                * CFrame.Angles(0f, MathF.PI, 0f) // its rotated (i blame you flash)
			                * CFrame.Angles(0f, 0f, roll);
			
			_orientation = _orientation.Lerp(target, Math.Clamp(8f * dt, 0f, 1f));
		}

		Body.Transform = new CFrame(Position) * _orientation;

		float clampedFlap = Math.Clamp(Velocity.Y / 10f + 1f, 0f, 1.5f);
		float currentTime = GameTimer.ElapsedTime;
		float flapAngle = clampedFlap * MathF.Sin(20f * currentTime);
		float flapCos = clampedFlap * 0.5f * MathF.Cos(20f * currentTime);

		LeftShoulder.Transform = CFrame.Angles(flapCos, 0f, flapAngle);
		RightShoulder.Transform = CFrame.Angles(-flapCos, 0f, -flapAngle);

		Body.Update(gfx);
		LeftWing.Update(gfx);
		RightWing.Update(gfx);

		LeftShoulder.Update();
		RightShoulder.Update();
	}

	public bool GeneratePath() {
		Vec3 playerPos = Onix.LocalPlayer!.Position;
		float timeSinceSpawn = GameTimer.ElapsedTime - SpawnTime;
		int currentKeyframe = (int)MathF.Ceiling(timeSinceSpawn / TimePerKeyframe);

		float dx = Position.X - playerPos.X;
		float dz = Position.Z - playerPos.Z;
		float distSq = dx * dx + dz * dz;

		switch (distSq) {
			case >= HardRemoveDist * HardRemoveDist:
			case >= DisappearDist * DisappearDist when !PlayerHelper.IsPositionVisible(Position):
				return false;
		}

		// multithreading??? never heard of it...
		while (Path.Count < currentKeyframe + 3) {
			Vec3? newPos = null;
			int attempts = 0;

			while (newPos == null) {
				if (attempts++ > MaxAttempts) {
					return false;
				}

				newPos = FindNewPosition(Path[^1], Path.Count >= 2 ? Path[^2] : null);
			}

			Path.Add(newPos.Value);
		}

		return true;
	}

	private Vec3? FindNewPosition(Vec3 lastPos, Vec3? beforeLastPos) {
		float yaw;

		if (beforeLastPos == null) {
			yaw = Random.Shared.NextSingle() * MathF.PI * 2f;
		} else {
			Vec3 dir = lastPos - beforeLastPos.Value;
			yaw = MathF.Atan2(dir.X, dir.Z) + MathHelper.ToRadians(MaxTurnAngle) * 2f * (Random.Shared.NextSingle() - 0.5f);
		}

		Vec2 offset = new(MathF.Sin(yaw) * KeyframeSpacing, MathF.Cos(yaw) * KeyframeSpacing);

		int worldX = (int)MathF.Floor(lastPos.X + offset.X);
		int worldZ = (int)MathF.Floor(lastPos.Z + offset.Y);

		int? mapHeight = WorldHelper.GetHeightAt(worldX, worldZ);
		if (mapHeight == null) return null;

		Vec3 newPos = new(lastPos.X + offset.X, mapHeight.Value + FlyHeight, lastPos.Z + offset.Y);
		
		RaycastResult ray = Onix.Region!.Raycast(lastPos, newPos, BlockShapeType.Collision);
		return ray.Type == RaycastResultType.Block ? null : newPos;
	}

	private void ComputePhysics(float dt) {
		float t = (GameTimer.ElapsedTime - SpawnTime) / TimePerKeyframe;

		Vec3 pos0 = MathHelper.CatmullRomSpline3D(Path, t, 0f, 0f);
		Vec3 pos1 = MathHelper.CatmullRomSpline3D(Path, t + dt, 0f, 0f);
		Vec3 pos2 = MathHelper.CatmullRomSpline3D(Path, t + dt * 2, 0f, 0f);

		Position = pos0;
		Velocity = (pos1 - pos0) / dt;
		Acceleration = ((pos2 - pos1) / dt - Velocity) / dt;
	}
}