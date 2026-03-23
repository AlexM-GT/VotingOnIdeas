using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Infrastructure.Persistence.Configurations;

public sealed class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.UserId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasMany(i => i.Votes)
            .WithOne(v => v.Idea)
            .HasForeignKey(v => v.IdeaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
