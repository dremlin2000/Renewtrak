using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Glossary.Infrastructure.EntityConfigurations
{
    class GlossaryEntityTypeConfiguration : IEntityTypeConfiguration<Core.Models.Glossary>
    {
        public void Configure(EntityTypeBuilder<Core.Models.Glossary> builder)
        {
            builder.ToTable(nameof(Core.Models.Glossary));
            builder.HasKey(b => b.Id);
        }
    }
}