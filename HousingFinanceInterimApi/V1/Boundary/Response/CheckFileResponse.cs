using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class CheckFileResponse
    {
        public bool ExistFile { get; set; }
        public DateTime NextStepTime { get; set; }
    }
}
