
using Domain.Enitities;

namespace Application.InterfacesServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IList<string> roles);

    }
}
