namespace Bridge.Server1
{
    public class Server1MQHandlerBase : MQHandlerBase
    {
        private const string TOKEN_KEY = "token";
        private const string MARKETID_KEY = "marketId";

        public string Token 
        { 
            get 
            {
                if(Context.Request.Headers.TryGetValue(TOKEN_KEY, out string? value))
                {
                    return value;
                }
                return string.Empty;
            } 
        }
        public int MarketId 
        { 
            get 
            {
                if(Context.Request.Headers.TryGetValue(MARKETID_KEY, out string? value) && int.TryParse(value, out int marketId))
                {
                    return marketId;
                }
                return 0;
            } 
        }
    }
}
