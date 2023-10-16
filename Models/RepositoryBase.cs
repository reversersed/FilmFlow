#define LOCALHOST

using Microsoft.EntityFrameworkCore;
using FilmFlow.Models.BaseTables;


namespace FilmFlow.Models
{
    public sealed class RepositoryBase : DbContext
    {
#if LOCALHOST
        private const string _connectionString = "Server=localhost; Username=postgres; Password=123; Database=FilmFlow;";
#else
        private const string _connectionString = "Server=bi0jwfmv9pdbuytvqaua-postgresql.services.clever-cloud.com; Username=ucc7pv5oypegng25lao6; Password=3aaUCiENjpcjZBGNSWsbSY7YhhAEsR; Database=bi0jwfmv9pdbuytvqaua;";
#endif
        public DbSet<Cover> covers { get; set; }
        public DbSet<MovieUrl> movieurl { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<Session> sessions { get; set; }
        public DbSet<MovieGenre> genres { get; set; }
        public DbSet<GenreCollection> genrecollection { get; set; }
        public RepositoryBase()
        {
            Database.EnsureCreated();
            this.ChangeTracker.LazyLoadingEnabled = true;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
