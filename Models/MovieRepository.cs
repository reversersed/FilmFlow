﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FilmFlow.Models
{
    public class MovieRepository : RepositoryBase, IMovieRepository
    {
        public MovieRepository() 
        {

        }

        public void LoadMovies(ObservableCollection<MovieModel> Movies)
        {
            using(var connnection = GetConnection())
            using (var command = connnection.CreateCommand())
            {
                connnection.Open();
                command.CommandText = "select movies.id, movies.\"name."+ Properties.Settings.Default.Language.ToString() + "\", movies.\"description." + Properties.Settings.Default.Language.ToString() + "\", covers.url from movies inner join covers on movies.url = covers.id order by movies.id";
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        MovieModel model = new MovieModel();
                        model.Id = (int)reader["id"];
                        model.Name = reader[(string)("name." + Properties.Settings.Default.Language.ToString())].ToString();
                        model.Cover = reader["url"].ToString();
                        model.Description = reader[(string)("description." + Properties.Settings.Default.Language.ToString())].ToString();
                        Movies.Add(model);
                    }
                }
            }
        }
    }
}