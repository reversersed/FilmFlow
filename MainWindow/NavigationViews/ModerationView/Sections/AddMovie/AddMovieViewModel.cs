using FilmFlow.Models.BaseTables;
using FilmFlow.Models;
using FilmFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace FilmFlow.MainWindow.NavigationViews.ModerationView.Sections.AddMovie
{
    class AddMovieViewModel : ViewModelBase
    {
        //Private properties
        private ObservableCollection<GenreModel> _genres;
        private ObservableCollection<CountryModel> _countries;
        private List<int> _selectedGenres = new List<int>();
        private ObservableCollection<CountryModel> _selectedCountries = new ObservableCollection<CountryModel>();
        private ObservableCollection<CountryModel> _showedCountries = new ObservableCollection<CountryModel>();
        private Movie _addMovieModel = new Movie() { Metadata = new MovieMetaData() };
        private string _error;
        private string _countrySearch = string.Empty;

        //Public properties
        public Movie AddMovieModel { get { return _addMovieModel; } set { _addMovieModel = value; OnPropertyChanged(nameof(AddMovieModel)); } }
        public string MovieYear
        {
            get { return _addMovieModel.Year.ToString(); }
            set
            {
                if (value == null || value.Length < 1)
                {
                    _addMovieModel.Year = null;
                    OnPropertyChanged(nameof(MovieYear));
                    return;
                }
                int year = 0;
                if (Int32.TryParse(value, out year))
                {
                    _addMovieModel.Year = year;
                    OnPropertyChanged(nameof(MovieYear));
                }
            }
        }
        public string MovieBudget
        {
            get { return _addMovieModel.Metadata.Budget == null ? string.Empty : _addMovieModel.Metadata.Budget.ToString(); }
            set
            {
                if (value == null || value.Length < 1)
                {
                    _addMovieModel.Metadata.Budget = null;
                    OnPropertyChanged(nameof(MovieBudget));
                    return;
                }
                int budget = 0;
                if (Int32.TryParse(value, out budget))
                {
                    if (budget < 0)
                        return;
                    _addMovieModel.Metadata.Budget = budget;
                    OnPropertyChanged(nameof(MovieBudget));
                }
            }
        }
        public string MovieCollected
        {
            get { return _addMovieModel.Metadata.Collected == null ? string.Empty : _addMovieModel.Metadata.Collected.ToString(); }
            set
            {
                if (value == null || value.Length < 1)
                {
                    _addMovieModel.Metadata.Collected = null;
                    OnPropertyChanged(nameof(MovieCollected));
                    return;
                }
                int collected = 0;
                if (Int32.TryParse(value, out collected))
                {
                    if (collected < 0)
                        return;
                    _addMovieModel.Metadata.Collected = collected;
                    OnPropertyChanged(nameof(MovieCollected));
                }
            }
        }
        public string MovieAgeLimit
        {
            get { return _addMovieModel.Metadata.Age == null ? string.Empty : _addMovieModel.Metadata.Age.ToString(); }
            set
            {
                if (value == null || value.Length < 1)
                {
                    _addMovieModel.Metadata.Age = null;
                    OnPropertyChanged(nameof(MovieAgeLimit));
                    return;
                }
                int age = 0;
                if (Int32.TryParse(value, out age))
                {
                    if (age < 0 || age > 21)
                        return;
                    _addMovieModel.Metadata.Age = age;
                    OnPropertyChanged(nameof(MovieAgeLimit));
                }
            }
        }
        public string CountrySearch { get { return _countrySearch; } set { _countrySearch = value; SortCountries(); OnPropertyChanged(nameof(CountrySearch)); } }
        public string MovieUrl
        {
            get { return AddMovieModel.Url; }
            set
            {
                AddMovieModel.Url = value;
                Uri url;
                if (Uri.TryCreate(value, UriKind.Absolute, out url))
                    OnPropertyChanged(nameof(MovieUrl));
            }
        }
        public string Error { get { return _error; } set { _error = value; OnPropertyChanged(nameof(Error)); } }
        public ObservableCollection<GenreModel> Genres { get { return _genres; } set { _genres = value; OnPropertyChanged(nameof(Genres)); } }
        public ObservableCollection<CountryModel> ShowedCountries { get { return _showedCountries; } set { _showedCountries = value; OnPropertyChanged(nameof(ShowedCountries)); } }
        public ObservableCollection<CountryModel> SelectedCountries { get { return _selectedCountries; } set { _selectedCountries = value; OnPropertyChanged(nameof(SelectedCountries)); } }

        //Models
        private MovieRepository movieRepository;

        //Commands
        public ICommand InsertMovie { get; }
        public ICommand GenreChecked { get; }
        public ICommand GenreUnchecked { get; }
        public ICommand SelectCountry { get; }
        public ICommand UnmarkCountry { get; }

        //Methods
        public AddMovieViewModel()
        {
            movieRepository = new MovieRepository();

            InsertMovie = new ViewModelCommand(InsertMovieCommand);
            GenreChecked = new ViewModelCommand(GenreCheckedCommand);
            GenreUnchecked = new ViewModelCommand(GenreUncheckedCommand);
            SelectCountry = new ViewModelCommand(SelectCountryCommand);
            UnmarkCountry = new ViewModelCommand(UnmarkCountryCommand);

            Genres = movieRepository.LoadGenreCollection();
            _countries = movieRepository.GetCountries();
            SortCountries();
        }
        private void SortCountries()
        {
            ObservableCollection<CountryModel> tmp = new ObservableCollection<CountryModel>(_countries.Where(i => SelectedCountries.FirstOrDefault(x => x.Id == i.Id) == default(CountryModel)).Where(i => CountrySearch.Length < 1 ? true : i.Name.Contains(CountrySearch)).Select(i => i).Take(10).ToList());
            ShowedCountries = tmp;
        }
        private void UnmarkCountryCommand(object obj)
        {
            ObservableCollection<CountryModel> tmp = new ObservableCollection<CountryModel>(SelectedCountries);
            if (tmp.FirstOrDefault(i => i.Id == (int)obj) != default(CountryModel))
                tmp.Remove(tmp.First(i => i.Id == (int)obj));
            SelectedCountries = tmp;
            SortCountries();
        }

        private void SelectCountryCommand(object obj)
        {
            ObservableCollection<CountryModel> tmp = new ObservableCollection<CountryModel>(SelectedCountries);
            if (_countries.FirstOrDefault(i => i.Id == (int)obj) != default(CountryModel))
                tmp.Add(_countries.First(i => i.Id == (int)obj));
            SelectedCountries = tmp;
            SortCountries();
        }

        private void GenreCheckedCommand(object obj) => _selectedGenres.Add((int)obj);
        private void GenreUncheckedCommand(object obj) => _selectedGenres.Remove((int)obj);
        private void InsertMovieCommand(object obj)
        {
            if (AddMovieModel.NameEn == null ||
                AddMovieModel.NameEn.Length < 1 ||
                AddMovieModel.NameRu == null ||
                AddMovieModel.NameRu.Length < 1)
            {
                Error = Application.Current.FindResource("SpecifyMovieName") as string;
                return;
            }
            if (AddMovieModel.DescriptionEn == null ||
                AddMovieModel.DescriptionEn.Length < 1 ||
                AddMovieModel.DescriptionRu == null ||
                AddMovieModel.DescriptionRu.Length < 1)
            {
                Error = Application.Current.FindResource("SpecifyMovieDesc") as string;
                return;
            }
            if (AddMovieModel.Year < 1800 || AddMovieModel.Year > DateTime.Now.Year + 20)
            {
                Error = Application.Current.FindResource("SpecifyMovieYear") as string;
                return;
            }
            if (AddMovieModel.Cover == null || AddMovieModel.Cover.Length < 1 ||
                (!AddMovieModel.Cover.Contains("http") &&
                !AddMovieModel.Cover.Contains("www") &&
                !AddMovieModel.Cover.Contains(".com") &&
                !AddMovieModel.Cover.Contains(".ru") &&
                !AddMovieModel.Cover.Contains(".net")))
            {
                Error = Application.Current.FindResource("SpecifyMovieCover") as string;
                return;
            }
            if (!_selectedGenres.Any())
            {
                Error = Application.Current.FindResource("SelectGenre") as string;
                return;
            }
            foreach (int i in _selectedGenres)
                AddMovieModel.Genre.Add(new MovieGenre() { genreid = i });
            foreach (CountryModel i in SelectedCountries)
                AddMovieModel.Country.Add(new MovieCountry() { CountryId = i.Id });
            movieRepository.AddMovie(AddMovieModel);
            AddMovieModel = new Movie() { Metadata = new MovieMetaData() };
            MovieYear = null;
            Genres = movieRepository.LoadGenreCollection();
            SelectedCountries = new ObservableCollection<CountryModel>();
            CountrySearch = string.Empty;
            SortCountries();
            Error = Application.Current.FindResource("MovieAdded") as string;
        }
    }
}
