using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.UI.MVVM
{
    public class OPCallMessageViewModel : CIMHostMessageViewModelBase
    {
        private readonly IConfiguration _configuration;

        protected override string CIMHostMessageLogFolder => _configuration["Folders:OPCallLogFolder"]!;

        public override string Header => "Operator Call Message";

        public OPCallMessageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
