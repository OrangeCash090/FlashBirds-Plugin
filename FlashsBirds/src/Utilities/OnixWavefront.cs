using OnixRuntime.Api.Rendering;

namespace FlashsBirds.Utilities;

public static class OnixWavefront {
	public static List<MeshBuilderVertexUvNormal> GetVertices(string path) {
		List<float[]> vertices = [];
		List<float[]> uvs = [];
		List<float[]> normals = [];
		List<MeshBuilderVertexUvNormal> finalVertices = [];

		string data = File.ReadAllText(path);
		string[] lines = data.Split("\n");

		foreach (string line in lines) {
			if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
				continue;

			string[] parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

			switch (parts[0]) {
				case "v":
					vertices.Add(
						[
							float.Parse(parts[1]),
							float.Parse(parts[2]),
							float.Parse(parts[3])
						]
					);
					break;

				case "vt":
					uvs.Add(
						[
							float.Parse(parts[1]),
							1f - float.Parse(parts[2])
						]
					);
					break;

				case "vn":
					normals.Add(
						[
							float.Parse(parts[1]),
							float.Parse(parts[2]),
							float.Parse(parts[3])
						]
					);
					break;

				case "f":
					if (parts.Length < 4)
						throw new Exception("Face has fewer than 3 vertices.");

					for (int i = 2; i < parts.Length - 1; i++) {
						TokenizeVertex(parts[1], vertices, uvs, normals, finalVertices);
						TokenizeVertex(parts[i], vertices, uvs, normals, finalVertices);
						TokenizeVertex(parts[i + 1], vertices, uvs, normals, finalVertices);
					}

					break;
			}
		}
		
		return finalVertices;
	}

	private static void TokenizeVertex(string token, List<float[]> vertices, List<float[]> uvs, List<float[]> normals, List<MeshBuilderVertexUvNormal> outVerts) {
		string[] indices = token.Split('/');

		int vIndex = int.Parse(indices[0]) - 1;
		float[] v = vertices[vIndex];

		float[] uv = uvs.Count > 0 && indices.Length > 1 && indices[1] != ""
			? uvs[int.Parse(indices[1]) - 1]
			: [ 0f, 0f ];

		float[] n = normals.Count > 0 && indices.Length > 2 && indices[2] != ""
			? normals[int.Parse(indices[2]) - 1]
			: [ 0f, 0f, 0f ];

		outVerts.Add(new MeshBuilderVertexUvNormal(
			v[0], v[1], v[2],
			uv[0], uv[1],
			n[0], n[1], n[2]
		));
	}
}