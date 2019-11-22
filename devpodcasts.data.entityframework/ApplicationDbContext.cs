using DevPodcast.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevPodcast.Data.EntityFramework
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<BasePodcast> BasePodcast { get; set; }
        public virtual DbSet<Episode> Episode { get; set; }
        public virtual DbSet<EpisodeTag> EpisodeTag { get; set; }
        public virtual DbSet<Podcast> Podcast { get; set; }
        public virtual DbSet<PodcastTag> PodcastTag { get; set; }
        public virtual DbSet<PodcastCategory> PodcastCategory { get; set; }
        public virtual DbSet<EpisodeCategory> EpisodeCategory { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EpisodeCategory>(entity =>
            {
                entity.HasKey(e => new {e.CategoryId, e.EpisodeId});

                entity.HasOne(d => d.Episode)
                    .WithMany(p => p.EpisodeCategories)
                    .HasForeignKey(d => d.EpisodeId);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.EpisodeCategories)
                    .HasForeignKey(d => d.CategoryId);
            });

            modelBuilder.Entity<PodcastCategory>(entity =>
            {
                entity.HasKey(e => new {e.CategoryId, e.PodcastId});

                entity.HasOne(d => d.Podcast)
                    .WithMany(p => p.PodcastCategories)
                    .HasForeignKey(d => d.PodcastId);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PodcastCategories)
                    .HasForeignKey(d => d.CategoryId);
            });

            modelBuilder.Entity<BasePodcast>(entity =>
            {
                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.ItunesId).HasMaxLength(50);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Episode>(entity =>
            {
                entity.Property(e => e.AudioDuration)
                    .HasMaxLength(10);

                entity.Property(e => e.AudioType).HasMaxLength(10);

                entity.Property(e => e.Author).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Podcast)
                    .WithMany(p => p.Episodes)
                    .HasForeignKey(d => d.PodcastId)
                    .HasConstraintName("FK_Episode_Podcast");
            });

            modelBuilder.Entity<EpisodeTag>(entity =>
            {
                entity.HasKey(e => new {e.TagId, e.EpisodeId});

                entity.HasOne(d => d.Episode)
                    .WithMany(p => p.EpisodeTags)
                    .HasForeignKey(d => d.EpisodeId);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.EpisodeTags)
                    .HasForeignKey(d => d.TagId);
            });

            modelBuilder.Entity<Podcast>(entity =>
            {
                entity.Property(e => e.Artists)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.EpisodeCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.ItunesId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.LatestReleaseDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PodcastTag>(entity =>
            {
                entity.HasKey(e => new {e.TagId, e.PodcastId});

                entity.HasOne(d => d.Podcast)
                    .WithMany(p => p.PodcastTags)
                    .HasForeignKey(d => d.PodcastId);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PodcastTags)
                    .HasForeignKey(d => d.TagId);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}