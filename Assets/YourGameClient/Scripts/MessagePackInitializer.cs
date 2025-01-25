using UnityEngine;
using MessagePack;
using MessagePack.Unity;

namespace YourGameClient
{
    public static class MessagePackInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(UnityResolver.InstanceWithStandardResolver);
        }
    }
}
