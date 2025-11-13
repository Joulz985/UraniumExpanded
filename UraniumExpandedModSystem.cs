namespace UraniumExpanded.ModSystem
{
    using HarmonyLib;
    using System.Linq;
    using System.Net.Sockets;
    using UraniumExpanded.ModConfiguration;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Server;
    using Vintagestory.API.Util;
    public class UraniumExpandedModSystem : ModSystem
    {
        private const string ConfigFileName = "uraniumexpanded.json";
        private ICoreClientAPI capi;
        private ICoreAPI api;
        private IServerNetworkChannel serverChannel;

        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            try
            {
                ModConfig fromDisk;
                if ((fromDisk = api.LoadModConfig<ModConfig>(ConfigFileName)) == null)
                { api.StoreModConfig(ModConfig.Loaded, ConfigFileName); }
                else
                { ModConfig.Loaded = fromDisk; }
            }
            catch
            { api.StoreModConfig(ModConfig.Loaded, ConfigFileName); }

            // Set a property that JSON patches can check
            api.World.Config.SetBool("EnableUraniumTools", ModConfig.Loaded.EnableUraniumTools);
            api.World.Config.SetBool("EnableUraniumGlass", ModConfig.Loaded.EnableUraniumGlass);
            if (api.Side == EnumAppSide.Server)
            {
                this.Mod.Logger.Event($"EnableUraniumTools set to {ModConfig.Loaded.EnableUraniumTools} on server");
                this.Mod.Logger.Event($"EnableUraniumGlass set to {ModConfig.Loaded.EnableUraniumGlass} on server");
            }

                bool emLoaded = api.ModLoader.GetMod("em") != null;
                // We want our patch to run only when EM is NOT present:
                api.World.Config.SetBool("EMisPresent", emLoaded);

                api.Logger.Notification($"[UraniumExpanded] em loaded = {emLoaded}. EMisPresent = {emLoaded}");

        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            this.capi = api;

            capi.Network.RegisterChannel("uraniumexpanded")
                .RegisterMessageType<SyncClientPacket>()
                .SetMessageHandler<SyncClientPacket>(packet =>
                {
                    ModConfig.Loaded.EnableUraniumTools = packet.EnableUraniumTools;
                    this.Mod.Logger.Event($"Received EnableUraniumTools of {packet.EnableUraniumTools} from server");
                    ModConfig.Loaded.EnableUraniumGlass = packet.EnableUraniumGlass;
                    this.Mod.Logger.Event($"Received EnableUraniumGlass of {packet.EnableUraniumGlass} from server");
                });
        }
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            // send connecting players the config settings
            sapi.Event.PlayerJoin += this.OnPlayerJoin; // add method so we can remove it in dispose to prevent memory leaks
            // register network channel to send data to clients
            this.serverChannel = sapi.Network.RegisterChannel("uraniumexpanded")
                .RegisterMessageType<SyncClientPacket>()
                .SetMessageHandler<SyncClientPacket>((player, packet) => { /* do nothing. idk why this handler is even needed, but it is */ });
        }

        private void OnPlayerJoin(IServerPlayer player)
        {
            // send the connecting player the settings it needs to be synced
            this.serverChannel.SendPacket(new SyncClientPacket
            {
                EnableUraniumTools = ModConfig.Loaded.EnableUraniumTools,
                EnableUraniumGlass = ModConfig.Loaded.EnableUraniumGlass,
            }, player);
        }
        public override void Dispose()
        {
            // remove our player join listener so we dont create memory leaks
            if (this.api is ICoreServerAPI sapi)
            {
                sapi.Event.PlayerJoin -= this.OnPlayerJoin;
            }

        }

    }
}
