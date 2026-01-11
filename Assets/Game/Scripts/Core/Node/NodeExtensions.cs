namespace Eclipse
{
    public static class NodeExtensions
    {
        public static Node Not(this Node node)
        {
            return new InverterDecorator(node);
        }

        // 나중에 다른 것도 추가 가능
    }
}
