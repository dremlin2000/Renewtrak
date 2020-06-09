using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Glossary.Infrastructure
{
    public class GlossaryDbContext : DbContext
    {
        public GlossaryDbContext(DbContextOptions<GlossaryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            //Apply all existing EntityTypeConfigurations in the executing assembly
            var configTypes =
                Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition &&
                t.GetTypeInfo().ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach (var configType in configTypes)
            {
                dynamic config = Activator.CreateInstance(configType);
                modelBuilder.ApplyConfiguration(config);
            }

            modelBuilder.Entity<Core.Models.Glossary>()
                .HasData(
                    new Core.Models.Glossary
                    {
                        Id = Guid.Parse("705b3587-eec0-434e-90ae-7e0a3d3326e3"),
                        Term = "abyssal plain",
                        Definition = "The ocean floor offshore from the continental margin, usually very flat with a slight slope."
                    },
                    new Core.Models.Glossary
                    {
                        Id = Guid.Parse("ebffa4ff-727c-4736-a316-0ae006b1c8c7"),
                        Term = "accrete",
                        Definition = "To add terranes (small land masses or pieces of crust) to another, usually larger, land mass."
                    },
                     new Core.Models.Glossary
                     {
                         Id = Guid.Parse("f623e3a8-3858-4336-a5c5-bb397013b75d"),
                         Term = "alkaline",
                         Definition = "Term pertaining to a highly basic, as opposed to acidic, subtance. For example, hydroxide or carbonate of sodium or potassium."
                     });
        }
    }

}
