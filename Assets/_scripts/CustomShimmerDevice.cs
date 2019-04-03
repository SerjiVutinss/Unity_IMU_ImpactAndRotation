using System;

namespace Assets._scripts
{
    [Serializable]
    public class CustomShimmerDevice
    {
        // com port should be unique
        public int ComPort;
        // shimmer ID should be unique
        public string ShimmerID;

        public string DisplayName { get => string.Format("Shimmer {0} on COM{1}", ShimmerID, ComPort); }

        public CustomShimmerDevice(int comPort, string shimmerID)
        {
            ComPort = comPort;
            ShimmerID = shimmerID;
        }
    }
}
