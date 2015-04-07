namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbModel : DbContext
    {
        public DbModel()
            : base("name=DbModel")
        {
        }

        public virtual DbSet<AudioFiles> AudioFiles { get; set; }
        public virtual DbSet<ImageFiles> ImageFiles { get; set; }
        public virtual DbSet<PartsOfSpeechEn> PartsOfSpeechEn { get; set; }
        public virtual DbSet<PartsOfSpeechRu> PartsOfSpeechRu { get; set; }
        public virtual DbSet<Translations> Translations { get; set; }
        public virtual DbSet<Words> Words { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Translations>()
                .HasMany(e => e.ImageFiles)
                .WithRequired(e => e.Translations)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Words>()
                .HasMany(e => e.AudioFiles)
                .WithRequired(e => e.Words)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Words>()
                .HasMany(e => e.Translations)
                .WithRequired(e => e.Words)
                .WillCascadeOnDelete(false);
        }
    }
}
