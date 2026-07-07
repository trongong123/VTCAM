using log4net;
using Newtonsoft.Json;

namespace EQX.Core.Common
{
    /// <summary>
    /// Default logger name is IIdentifier.Name
    /// </summary>
    public interface ILogable
    {
        [JsonIgnore]
        ILog Log { get; }
    }
}
