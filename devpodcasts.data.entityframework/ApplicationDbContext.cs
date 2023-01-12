using devpodcasts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace devpodcasts.Data.EntityFramework
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
    
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        public virtual DbSet<BasePodcast> BasePodcast { get; set; }
        public virtual DbSet<Episode> Episode { get; set; }
        public virtual DbSet<Podcast> Podcast { get; set; }
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
            //Category
            modelBuilder.Entity<Category>()
                .Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            //Episode
            modelBuilder.Entity<Episode>(episode =>
            {
                episode.HasKey(e => e.Id);

                episode
                .HasMany<Category>(c => c.Categories)
                .WithMany(e => e.Episodes);

                episode
                .HasMany<Tag>(c => c.Tags)
                .WithMany(e => e.Episodes);
            });




            //Podcast
            modelBuilder.Entity<Podcast>(podcast =>
            {
                podcast.HasKey(e => e.Id);

                podcast
                .HasMany<Category>(c => c.Categories)
                .WithMany(e => e.Podcasts);

                podcast
                .HasMany<Tag>(c => c.Tags)
                .WithMany(e => e.Podcasts);

                podcast.Property(e => e.Artists).HasMaxLength(200).IsUnicode(false);

                podcast.Property(e => e.Description).IsRequired();
            });


            //Tag
            modelBuilder.Entity<Tag>()
                .Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}