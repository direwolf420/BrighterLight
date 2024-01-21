using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace BrighterLight
{
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static Config Instance;

		public const float Min = 0.5f;
		public const float Max = 1.1f;
		public const float Increment = 0.00625f;
		public const float Default = 1.025f;

		[Slider]
		[Range(Min, Max)]
		[Increment(Increment)]
		[DefaultValue(Default)]
		public float Brightness;

		public static void Clamp(ref float value, float min, float max)
		{
			value = value < min ? min : (value > max ? max : value);
		}

		public static bool IsPlayerLocalServerOwner(int whoAmI)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return Netplay.Connection.Socket.GetRemoteAddress().IsLocalHost();
			}

			return NetMessage.DoesPlayerSlotCountAsAHost(whoAmI);
		}

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return true;
			else if (!IsPlayerLocalServerOwner(whoAmI))
			{
				message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost");
				return false;
			}
			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			Clamp(ref Brightness, Min, Max);
		}
	}

	public class MPClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static MPClientConfig Instance;

		[Header("MultiplayerOnly")]

		[Slider]
		[Range(Config.Min, Config.Max)]
		[Increment(Config.Increment)]
		[DefaultValue(Config.Default)]
		public float Brightness;

		[DefaultValue(true)]
		public bool UseServerSettings;

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			Config.Clamp(ref Brightness, Config.Min, Config.Max);
		}
	}
}
