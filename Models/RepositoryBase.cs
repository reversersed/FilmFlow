using Microsoft.EntityFrameworkCore;
using FilmFlow.Models.BaseTables;

namespace FilmFlow.Models
{
    public sealed class RepositoryBase : DbContext
    {
        private readonly string _connectionString = string.Format("Server={0}; Username={1}; Password={2}; Database={3};",
            FilmFlow.Properties.Settings.Default.localDbServer,
            FilmFlow.Properties.Settings.Default.localDbUser,
            FilmFlow.Properties.Settings.Default.localDbPassword,
            FilmFlow.Properties.Settings.Default.localDbBase);

        public DbSet<User> users { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<MovieGenre> genres { get; set; }
        public DbSet<GenreCollection> genrecollection { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<CountryCollection> countries { get; set; }
        public DbSet<MovieCountry> movie_countries { get; set; }
        public DbSet<Favourite> favourite { get; set; }
        public DbSet<Replenishment> replenishments { get; set; }
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
