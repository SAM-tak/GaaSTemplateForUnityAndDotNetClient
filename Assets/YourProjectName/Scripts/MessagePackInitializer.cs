using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

public static class MessagePackInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Initialize()
    {
        StaticCompositeResolver.Instance.Register(GeneratedResolver.Instance, StandardResolver.Instance);
        MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
    }
}