using Glossary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glossary.Api.IntegrationTests.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(GlossaryDbContext db)
        {
            db.Set<Core.Models.Glossary>().AddRange(GetSeedTestData());
            db.SaveChanges();
        }

        public static List<Core.Models.Glossary> GetSeedTestData()
        {
            return new List<Core.Models.Glossary>()
            {
                new Core.Models.Glossary
                {
                    Id = Guid.Parse("f8ecf490-22f6-4550-8fb8-427e57250404"),
                    Term = "TestTerm2",
                    Definition = "TestDefinition2"
                },
                new Core.Models.Glossary
                {
                    Id = Guid.Parse("ddb7b5b9-67a0-476d-902e-56b55565c57f"),
                    Term = "TestTerm1",
                    Definition = "TestDefinition1"
                }
            };
        }
    }
}
