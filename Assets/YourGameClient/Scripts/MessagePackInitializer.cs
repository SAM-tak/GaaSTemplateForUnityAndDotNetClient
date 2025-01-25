using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;

namespace YourGameClient
{
    public static class MessagePackInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            StaticCompositeResolver.Instance.Register(
                UnityResolver.InstanceWithStandardResolver,
                MessagePack.CustomResolver.Instance
            );
            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithResolver(StaticCompositeResolver.Instance);
        }
    }
}
