namespace UserService.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<UserServiceWebApplicationFactory>
{
    public const string Name = "Integration";
}
