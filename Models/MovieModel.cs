using FilmFlow.Models.BaseTables;
using System;
using System.Collections.ObjectModel;

namespace FilmFlow.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public string? Desription { get; set; }
        public float Rating { get; set; }
        public int ReviewCount { get; set; }
        public int? Year { get; set; }
        public ObservableCollection<GenreModel> Genres { get; set; }
        public ObservableCollection<CountryModel> Countries { get; set; }
        public string Url {  get; set; }
        public string? Tagline { get; set; }
        public string? Director { get; set; }
        public string? Screenwriter { get; set; }
        public string? Producer { get; set; }
        public string? Videographer { get; set; }
        public string? Composer { get; set; }
        public string? Drawer { get; set; }
        public string? Montage { get; set; }
        public int? Budget { get; set; }
        public int? Collected { get; set; }
        public DateTime? Premier { get; set; }
        public int? Age { get; set; }
        public MovieModel()
        {
            
        }
        public MovieModel(Movie movie, int reviewCount)
        {
            Id = movie.Id;
            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.NameRu : movie.NameEn;
            Cover = movie.Cover;
            Desription = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.DescriptionRu : movie.DescriptionEn;
            Genres = new ObservableCollection<GenreModel>();
            foreach (var genre in movie.Genre)
            {
                Genres.Add(new GenreModel
                {
                    Id = genre.Genre.Id,
                    Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? genre.Genre.NameRu : genre.Genre.NameEn
                });
            }
            Countries = new ObservableCollection<CountryModel>();
            foreach (var country in movie.Country)
            {
                Countries.Add(new CountryModel
                {
                    Id = country.Country.Id,
                    Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? country.Country.NameRu : country.Country.NameEn
                }) ;
            }
            Rating = movie.Rating;
            Year = movie.Year;
            Url = movie.Url;
            ReviewCount = reviewCount;
            Tagline = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.TaglineRu : movie.Metadata.TaglineEn;
            Director = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.DirectorRu : movie.Metadata.DirectorEn;
            Screenwriter = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.ScreenwriterRu : movie.Metadata.ScreenwriterEn;
            Producer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.ProducerRu : movie.Metadata.ProducerEn;
            Videographer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.VideographerRu : movie.Metadata.VideographerEn;
            Composer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.ComposerRu : movie.Metadata.ComposerEn;
            Drawer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.DrawerRu : movie.Metadata.DrawerEn;
            Montage = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.Metadata.MontageRu : movie.Metadata.MontageEn;
            Budget = movie.Metadata.Budget;
            Collected = movie.Metadata.Collected;
            Premier = movie.Metadata.Premier == null || movie.Metadata.Premier.Value.Year < 1000 ? null : movie.Metadata.Premier;
            Age = movie.Metadata.Age;
        }
    }
}
