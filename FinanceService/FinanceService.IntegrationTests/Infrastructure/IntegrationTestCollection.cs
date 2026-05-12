namespace FinanceService.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<FinanceServiceWebApplicationFactory>
{
    public const string Name = "Integration";
}
