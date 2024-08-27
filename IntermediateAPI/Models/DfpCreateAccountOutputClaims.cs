using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpCreateAccountOutputClaims : DfpAccountGenericOutputClaims
    {
        public string SignUpId { get; set; }
    }
}
