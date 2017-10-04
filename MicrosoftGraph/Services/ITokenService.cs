using System.Threading.Tasks;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    public interface ITokenService
    {
        Task<string> GetAccessToken(TokenProvider tokenProvider, AutoAuthConfiguration autoAuthConfiguration);
    }
}
