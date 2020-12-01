using Maincotech.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maincotech.EF
{
    public class AssociationMappingConfiguration<T, TSource, TTarget> : EntityTypeConfiguration<T>
        where T : AssociationBase<TSource, TTarget>
        where TTarget : class
        where TSource : class
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(typeof(T).Name);
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Source).WithMany().HasForeignKey(x => x.SourceId);
            builder.HasOne(x => x.Target).WithMany().HasForeignKey(x => x.TargetId);
            base.Configure(builder);
        }
    }
}