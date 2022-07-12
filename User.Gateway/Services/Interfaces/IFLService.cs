using Flurl.Http;

namespace User.Gateway.Services.Interfaces
{
    public interface IFLService
    {
        IFlurlRequest Request(string path);
        public string[] FormErrors(object formErrors);
    }
}