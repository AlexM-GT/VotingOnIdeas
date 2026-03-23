using VotingOnIdeas.API.Tests.Infrastructure;

namespace VotingOnIdeas.API.Tests;

[CollectionDefinition("Integration")]
public sealed class IntegrationTestsCollection : ICollectionFixture<TestWebApplicationFactory>;
