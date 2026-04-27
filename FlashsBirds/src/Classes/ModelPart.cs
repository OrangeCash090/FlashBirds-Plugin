using FlashsBirds.Utilities;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;

namespace FlashsBirds.Classes;

public class ModelPart {
	public Vec3 Position => Transform.Position;
	public Vec3 Rotation => Transform.ToEulerAnglesYXZ();
	public Vec3 Size = Vec3.One;
	
	public CFrame Transform = new();

	public string? MeshPath;
	public GameMesh? Mesh;
	public TexturePath? Texture;

	public void Update(RendererWorld gfx) {
		if (MeshPath == null) return;
		
		if (Mesh == null) {
			Mesh = AssetHelper.GetMesh(gfx, MeshPath);
		}
		
		using (_ = gfx.PushTransformation(Transform.ToTransformMatrix(Size))) {
			if (Texture != null) {
				gfx.RenderMesh(Mesh, Texture);
			} else {
				gfx.RenderMesh(Mesh, ColorF.White);
			}
		}
	}
}