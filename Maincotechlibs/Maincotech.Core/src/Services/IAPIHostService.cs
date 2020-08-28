namespace Maincotech.Services
{
    public interface IAPIHostService : IHostService
    {
        string GetAPIMetadataUrl();
    }
}