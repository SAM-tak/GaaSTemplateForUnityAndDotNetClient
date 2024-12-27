using UnityEngine;
using MessagePack;
using MessagePack.Unity;
using MessagePack.Unity.Extension;

namespace YourGameClient
{
    public static class MessagePackInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(UnityResolver.InstanceWithStandardResolver);
        }
    }
}
