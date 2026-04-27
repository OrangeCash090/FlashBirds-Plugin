using FlashsBirds.Utilities;
using OnixRuntime.Api;
using OnixRuntime.Plugin;
using OnixRuntime.Api.Rendering;

namespace FlashsBirds {
	public class FlashsBirds : OnixPluginBase {
		public static FlashsBirds Instance { get; private set; } = null!;
		public static FlashsBirdsConfig Config { get; private set; } = null!;

		private static BirdManager? Manager;

		public FlashsBirds(OnixPluginInitInfo initInfo) : base(initInfo) {
			Instance = this;
			base.DisablingShouldUnloadPlugin = false;
			
			#if DEBUG
			// base.WaitForDebuggerToBeAttached();
			#endif
		}

		protected override void OnLoaded() {
			Console.WriteLine($"Plugin {CurrentPluginManifest.Name} loaded!");
			
			Config = new FlashsBirdsConfig(PluginDisplayModule, true);
			AssetHelper.AssetPath = PluginAssetsPath + "\\";
			Onix.Events.Common.WorldRender += OnWorldRender;

			Manager = new BirdManager(Config);
		}

		protected override void OnEnabled() { }

		protected override void OnDisabled() { }

		protected override void OnUnloaded() {
			Console.WriteLine($"Plugin {CurrentPluginManifest.Name} unloaded!");
			Onix.Events.Common.WorldRender -= OnWorldRender;
			
			AssetHelper.ClearCache();
			Manager = null;
		}

		private void OnWorldRender(RendererWorld gfx, float delta) {
			if (Onix.LocalPlayer == null) return;
			
			gfx.SetMaterialParameters(new GameMaterialParameters {
				Light = true
			});
			
			Manager?.Update(gfx, delta);
			GameTimer.Update(delta);
		}
	}
}