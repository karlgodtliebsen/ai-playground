using System.Security.Claims;

namespace AI.Test.Support.Fixtures;

/// <summary>
/// https://gunnarpeipman.com/aspnet-core-integration-tests-users-roles/
/// </summary>
public class TestClaimsProvider
{
    public IList<Claim> Claims { get; }

    public TestClaimsProvider(IList<Claim> claims)
    {
        Claims = claims;
    }

    public TestClaimsProvider()
    {
        Claims = new List<Claim>();
    }

    public static TestClaimsProvider WithAdministratorClaims()
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Ulid.NewUlid().ToString()));
        provider.Claims.Add(new Claim(ClaimTypes.Name, "Administrator"));
        provider.Claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
        return provider;
    }
}
