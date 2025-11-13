using ProtoBuf;

namespace UraniumExpanded.ModSystem
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncClientPacket
    {

        public bool EnableUraniumTools;
        public bool EnableUraniumGlass;

    }
}