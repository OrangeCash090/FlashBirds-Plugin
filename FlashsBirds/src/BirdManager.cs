using FlashsBirds.Classes;
using FlashsBirds.Helpers;
using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.World;

namespace FlashsBirds;

public class BirdManager(FlashsBirdsConfig config) {
	public readonly FlashsBirdsConfig Config = config;
	public readonly List<Bird> Birds = [];

	private const float ClosestSpawnDist = 10f;
	private const float FurthestSpawnDist = 50f;

	public void Update(RendererWorld gfx, float delta) {
		Birds.RemoveAll(bird => bird.ShouldRemove);
		TrySpawnBird();

		foreach (Bird bird in Birds) {
			bird.Update(gfx, delta);
		}
	}

	private void TrySpawnBird() {
		if (Birds.Count >= Config.MaxBirds) return;

		float time = Onix.Dimension!.NormalizedTime;
		if (MathF.Abs(4f*time - 2f) <= 1f || Onix.Dimension.Id != DimensionType.Overworld) return;

		Vec3 playerPos = Onix.LocalPlayer!.Position;
		float angle = Random.Shared.NextSingle() * MathF.PI * 2f;
		float dist = ClosestSpawnDist + Random.Shared.NextSingle() * (FurthestSpawnDist - ClosestSpawnDist);

		int spawnX = (int)(playerPos.X + MathF.Cos(angle) * dist);
		int spawnZ = (int)(playerPos.Z + MathF.Sin(angle) * dist);

		int? mapHeight = WorldHelper.GetHeightAt(spawnX, spawnZ);
		if (mapHeight == null) return;

		Vec3 spawnPos = new(spawnX, mapHeight.Value + 1.2f, spawnZ);
		
		if (PlayerHelper.IsPositionVisible(spawnPos)) return;
		Birds.Add(new Bird(spawnPos));
	}
}