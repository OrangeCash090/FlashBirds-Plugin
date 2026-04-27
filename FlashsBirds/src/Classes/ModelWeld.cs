using FlashsBirds.Utilities;

namespace FlashsBirds.Classes;

public class ModelWeld(ModelPart part0, ModelPart part1, CFrame c0, CFrame c1) {
	public ModelPart Part0 = part0;
	public ModelPart Part1 = part1;

	public CFrame C0 = c0;
	public CFrame C1 = c1;

	public CFrame Transform = new();

	public void Update() {
		Part1.Transform = Part0.Transform * C0 * Transform * C1.Inverse();
	}
}