namespace HelpMyStreetFE.Models.Reponses
{
    public class LogRequestResponseContent
    {
        public int RequestID { get; set; }
        public bool Fulfillable { get; set; }
    }

    public class LogRequestResponse
    {
        public bool HasContent { get; set; }
        public bool IsSuccessful { get; set; }
        public LogRequestResponseContent content { get; set; }
    }
}
