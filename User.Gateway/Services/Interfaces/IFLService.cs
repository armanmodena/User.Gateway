using Flurl.Http;

namespace User.Gateway.Services.Interfaces
{
    public interface IFLService
    {
        public IFlurlRequest IdentityRequest(string path);
        IFlurlRequest Request(string path);
        public string[] FormErrors(object formErrors);
    }
}