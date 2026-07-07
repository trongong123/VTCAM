using EQX.Core.Common;
using Microsoft.Extensions.Configuration;

namespace EQX.UI.MVVM
{
    public class InterlockMessageViewModel : CIMHostMessageViewModelBase
    {
        private readonly IConfiguration _configuration;

        protected override string CIMHostMessageLogFolder => _configuration["Folders:InterlockLogFolder"]!;

        public override string Header => "Interlock";
        public InterlockMessageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
