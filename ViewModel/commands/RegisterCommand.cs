﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesAPP.ViewModel.commands
{
    public class RegisterCommand : ICommand
    {
        public LoginVM VM { get; set; }
        public event EventHandler CanExecuteChanged;

        public RegisterCommand(LoginVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
