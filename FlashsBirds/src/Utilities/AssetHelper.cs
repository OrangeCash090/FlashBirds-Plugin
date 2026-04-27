using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace FlashsBirds.Utilities;

public static class AssetHelper {
	public static string AssetPath = "";
	private static readonly Dictionary<string, GameMesh> _meshCache = [];

	public static GameMesh GetMesh(RendererWorld gfx, string path) {
		if (_meshCache.TryGetValue(path, out GameMesh? mesh)) {
			return mesh;
		}

		using GameMeshBuilder.GameMeshBuilderSession mb = gfx.NewMeshBuilderSession(ColorF.White, MeshBuilderPrimitiveType.Triangle);
		mb.Builder.PostTransformOffset = Vec3.Zero;

		mb.Builder.VertexBatch(OnixWavefront.GetVertices(AssetPath + path));
	
		GameMesh builtMesh = mb.Builder.Build();
		_meshCache.Add(path, builtMesh);
		
		return builtMesh;
	}

	public static void ClearCache() {
		foreach ((string name, GameMesh mesh) in _meshCache) {
			mesh.Dispose();
		}
	}
}