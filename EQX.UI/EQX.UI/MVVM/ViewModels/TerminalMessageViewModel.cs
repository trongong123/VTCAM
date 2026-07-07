using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.UI.MVVM
{
    public class TerminalMessageViewModel : CIMHostMessageViewModelBase
    {
        private readonly IConfiguration _configuration;

        protected override string CIMHostMessageLogFolder => _configuration["Folders:TerminalLogFolder"]!;

        public override string Header => "Terminal";

        public TerminalMessageViewModel(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
    }
}
