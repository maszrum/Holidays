using NUnit.Framework;

namespace Holidays.MongoDb.Tests.TestFixtures;

[TestFixture]
public class OffersMongoRepositoryTests
{
    [Test]
    public void test_one()
    {
        var repository = Create.Repository();
    }
}
