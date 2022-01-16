namespace System.ComponentModel.DataAnnotations
{
    internal sealed class DisplayAttribute : Attribute
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
}
