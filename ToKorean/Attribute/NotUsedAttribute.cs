namespace ToKorean.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    internal class NotUsedAttribute : System.Attribute
    {
        public NotUsedAttribute() { }
    }
}
