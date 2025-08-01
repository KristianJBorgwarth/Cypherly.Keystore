﻿using Keystore.Infrastructure.Persistence.Context;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Keystore.Test.Integration.Setup.Fixtures;

[CollectionDefinition("KeystoreApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, KeystoreDbContext>>
{

}