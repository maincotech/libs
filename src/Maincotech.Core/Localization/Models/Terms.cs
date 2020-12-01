using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maincotech.Localization.Models
{
    public class Terms : EntityBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string CultureCode { get; set; }
    }

    public class TermsMappingConfiguration : EntityTypeConfiguration<Terms>
    {
        public override void Configure(EntityTypeBuilder<Terms> builder)
        {
            builder.ToTable(nameof(Terms));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).HasMaxLength(150);
            builder.Property(x => x.Value).IsRequired(true);
            builder.Property(x => x.CultureCode).IsRequired(true).HasMaxLength(10);
            base.Configure(builder);
        }
    }
}