using OnixRuntime.Api;
using OnixRuntime.Api.Maths;

namespace FlashsBirds.Helpers;

public static class WorldHelper {
	public static int? GetHeightAt(int x, int z) {
		ChunkPos chunk = new(x >> 4, z >> 4);
		int? mapHeight = Onix.Region!.GetChunk(chunk)?.GetHeightAt(x & 15, z & 15);
		return mapHeight;
	}
}