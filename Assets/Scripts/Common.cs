namespace MyGame
{
    public static class Common
    {
        public static bool ContainLayer(int layerMask, int layerIndex)
        {
            return (layerMask & (1 << layerIndex)) != 0;
        }
    }
}
