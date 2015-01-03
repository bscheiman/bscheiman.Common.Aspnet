namespace bscheiman.Common.Aspnet.Interfaces {
    public interface IHasApiKeys {
        string ApiKey { get; set; }
        string ApiSecret { get; set; }
    }
}