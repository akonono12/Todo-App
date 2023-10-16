using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ItemId);
        builder.Property(x => x.Id)
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

        SqlServerPropertyBuilderExtensions.UseIdentityColumn(builder.Property(x => x.Id), 1L, 1);

        builder.Property(x => x.Created)
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedBy)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.LastModified)
            .HasColumnType("datetime2");

        builder.Property(x => x.LastModifiedBy)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ItemId)
            .HasColumnType("int");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.HasOne(x => x.Item)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        builder.Navigation(x => x.Item);


    }
}
