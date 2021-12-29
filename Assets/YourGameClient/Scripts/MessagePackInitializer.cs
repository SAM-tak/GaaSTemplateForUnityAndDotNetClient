using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;

public static class MessagePackInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Initialize()
    {
        StaticCompositeResolver.Instance.Register(
//            GeneratedResolver.Instance,
            UnityResolver.Instance,
            UnityBlitWithPrimitiveArrayResolver.Instance,
            StandardResolver.Instance
        );
        MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithResolver(StaticCompositeResolver.Instance);
    }
}
