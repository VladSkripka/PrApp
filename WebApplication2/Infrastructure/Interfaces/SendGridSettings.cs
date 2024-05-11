namespace WebApplication2.Infrastructure.Interfaces
{
    public interface ISendGridApiService
    {
        string GetSendGridApiKey();
    }
    public class SendGridSettings : ISendGridApiService
    {

        private readonly string _sendGridApiKey;

        public SendGridSettings(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }

        public string GetSendGridApiKey()
        {
            return _sendGridApiKey;
        }

    }
}
