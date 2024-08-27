namespace IntermediateAPI.Models
{
    public class DfpAccountGenericOutputClaims : DfpResponseEnhancements
    {
        public string CorrelationId { get; set; }

        public string Decision { get; set; }

        public int BotScore { get; set; }

        public int RiskScore { get; set; }
        public string TransactionReferenceId { get; set; }
    }
}
