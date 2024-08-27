using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpLoginAccountOutputClaims : DfpAccountGenericOutputClaims
    {
        public string LoginId { get; set; }
    }
}
