using JobIntechCHAT.API.Models;

namespace JobIntechCHAT.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser token);
    }
}
