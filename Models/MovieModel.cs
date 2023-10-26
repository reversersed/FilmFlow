using FilmFlow.Models.BaseTables;
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
        public int Year { get; set; }
        public ObservableCollection<GenreModel> Genres { get; set; }

        public MovieModel()
        {
            
        }
        public MovieModel(Movie movie, int reviewCount, ObservableCollection<GenreModel> genres)
        {
            Id = movie.Id;
            Name = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.NameRu : movie.NameEn;
            Cover = movie.Cover.Url;
            Desription = FilmFlow.Properties.Settings.Default.Language.Equals("ru-RU") ? movie.DescriptionRu : movie.DescriptionEn;
            Genres = new ObservableCollection<GenreModel>(genres);
            Rating = movie.Rating;
            Year = movie.Year;
            ReviewCount = reviewCount;
        }
    }
}
