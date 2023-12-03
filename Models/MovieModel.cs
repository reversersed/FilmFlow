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
        public bool IsInFavourite { get; set; }
        public MovieModel()
        {
            
        }
        public MovieModel(Movie movie, int reviewCount, bool IsInFavourite)
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
            Tagline = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.TaglineRu : movie.TaglineEn;
            Director = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.DirectorRu : movie.DirectorEn;
            Screenwriter = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.ScreenwriterRu : movie.ScreenwriterEn;
            Producer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.ProducerRu : movie.ProducerEn;
            Videographer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.VideographerRu : movie.VideographerEn;
            Composer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.ComposerRu : movie.ComposerEn;
            Drawer = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.DrawerRu : movie.DrawerEn;
            Montage = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.MontageRu : movie.MontageEn;
            Budget = movie.Budget;
            Collected = movie.Collected;
            Premier = movie.Premier == null || movie.Premier.Value.Year < 1000 ? null : movie.Premier;
            Age = movie.Age;
            this.IsInFavourite = IsInFavourite;
        }
    }
}
