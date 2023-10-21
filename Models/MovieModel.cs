using FilmFlow.Models.BaseTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public string? Desription { get; set; }
        public int Price { get; set; }
        public ObservableCollection<GenreModel> Genres { get; set; }
    }
}
