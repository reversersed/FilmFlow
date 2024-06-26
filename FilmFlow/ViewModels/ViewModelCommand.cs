﻿using System;
using System.Windows.Input;

namespace FilmFlow.ViewModels
{
    class ViewModelCommand : ICommand
    {
        private readonly Action<object?>? _execute;
        private readonly Predicate<object?>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ViewModelCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public ViewModelCommand(Action<object> execute)
        {
            _execute = execute;
            _canExecute = null;
        }
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
