using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;

namespace YourGameClient
{
    public static class MessagePackInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            StaticCompositeResolver.Instance.Register(
                BuiltinResolver.Instance,
                UnityResolver.Instance,
                UnityBlitWithPrimitiveArrayResolver.Instance,
                GeneratedResolver.Instance,
                MessagePack.CustomResolver.Instance
            );
            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithResolver(StaticCompositeResolver.Instance);
        }
    }
}
