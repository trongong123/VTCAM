using Newtonsoft.Json;

namespace EQX.Core.Vision.Algorithms
{
    public class VisionToolConnection
    {
        public int OriginToolId { get; }
        public int TargetToolId { get; }
        public int OriginKeyId { get; }
        public int TargetKeyId { get; set; }

        public VisionToolConnection(int originToolId, int targetToolId, int key)
            : this(originToolId, targetToolId, key, key)
        {
        }

        [JsonConstructor]
        public VisionToolConnection(int originToolId, int targetToolId, int originKeyId, int targetKeyId)
        {
            OriginToolId = originToolId;
            TargetToolId = targetToolId;
            OriginKeyId = originKeyId;
            TargetKeyId = targetKeyId;
        }
    }
}