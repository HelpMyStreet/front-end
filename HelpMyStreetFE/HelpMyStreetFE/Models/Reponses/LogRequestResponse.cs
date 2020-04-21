namespace HelpMyStreetFE.Models.Reponses
{
    public class LogRequestResponse : BaseResponse
    {
        public int RequestID { get; set; }
        public bool Fulfillable { get; set; }

    }
}
