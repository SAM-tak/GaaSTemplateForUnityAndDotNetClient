#nullable disable // Server needs this

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    public enum Store
    {
        // Official stores
        AppStore = 1,
        GooglePlay,
        KindleStore,
        MicrosoftStore,
        PSStore,
        NintendoStore,
        Steam,
        EpicStore,
        EAOrigin,
        UPlay,
        DMM,
    }
}
