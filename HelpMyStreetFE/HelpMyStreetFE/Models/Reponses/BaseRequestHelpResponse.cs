namespace HelpMyStreetFE.Models.Reponses
{
    public class BaseRequestHelpResponse<TResponse>
    {
        public bool HasContent { get; set; }
        public bool IsSuccessful { get; set; }
        public TResponse Content { get; set; }
    }
}
