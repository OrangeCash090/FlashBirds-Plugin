using OnixRuntime.Api;
using OnixRuntime.Api.Maths;

namespace FlashsBirds.Helpers;

public static class PlayerHelper {
	public static bool IsPositionVisible(Vec3 point, float radius = 1f) {
		Vec2? screen = Onix.Render.WorldToScreen(point);
		if (screen == null) return false;

		Vec3 camPos = Onix.LocalPlayer!.Position + new Vec3(0, 1.62f, 0);
		Angles rotation = Onix.LocalPlayer.Rotation;
		
		float yaw = MathHelper.ToRadians(rotation.Yaw);
		float pitch = MathHelper.ToRadians(-rotation.Pitch);
		
		Vec3 relative = point - camPos;
		Vec3 rotated = relative.Rotate(yaw, pitch);
		
		if (rotated.Z <= 0f) return false;

		Vec2 screenSize = Onix.Gui.ScreenSize;
		float fovV = Onix.Render.RawFov.Y;
		float screenRadius = radius * screenSize.Y / (rotated.Z * MathF.Tan(MathHelper.ToRadians(fovV / 2f)));

		return screen.Value.X + screenRadius >= 0
		       && screen.Value.X - screenRadius < screenSize.X
		       && screen.Value.Y + screenRadius >= 0
		       && screen.Value.Y - screenRadius < screenSize.Y;
	}
}