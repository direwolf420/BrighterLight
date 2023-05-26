using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BrighterLight
{
	class BrighterLight : Mod
	{
		public static LocalizedText AcceptClientChangesText { get; private set; }

		public override void Load()
		{
			string category = $"Configs.Common.";
			AcceptClientChangesText ??= Language.GetOrRegister(this.GetLocalizationKey($"{category}AcceptClientChanges"));
		}
	}

	class BLSystem : ModSystem
	{
		public static float GetBrightness()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (MPClientConfig.Instance.UseServerSettings)
				{
					return Config.Instance.Brightness;
				}

				return MPClientConfig.Instance.Brightness;
			}

			return Config.Instance.Brightness;
		}

		public override void ModifyLightingBrightness(ref float scale)
		{
			float brightness = GetBrightness();
			if (brightness < 1)
			{
				if (scale * brightness > Config.Min - Config.Increment) scale *= brightness;
				else scale = Config.Min + Config.Increment;
			}
			else
			{
				float nightVisionLimit = 1.05625f;
				if (scale * brightness < Config.Max) scale *= brightness;
				else scale = Config.Max - Config.Increment;
				if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.nightVision && scale > nightVisionLimit) scale = nightVisionLimit;
			}
		}
	}
}
