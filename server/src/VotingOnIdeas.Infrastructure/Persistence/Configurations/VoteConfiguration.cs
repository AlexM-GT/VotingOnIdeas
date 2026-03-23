using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Infrastructure.Persistence.Configurations;

public sealed class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.IdeaId)
            .IsRequired();

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.Value)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        // Enforce one vote per (UserId, IdeaId) pair
        builder.HasIndex(v => new { v.UserId, v.IdeaId }).IsUnique();

        builder.ToTable(tb => tb.HasCheckConstraint(
            "CK_Votes_Value",
            $"[Value] >= {Vote.MinValue} AND [Value] <= {Vote.MaxValue}"));
    }
}
