using NotesAPP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesAPP.ViewModel.commands
{
    public class HasEditedCommand : ICommand
    {
        public NotesVM VM { get; set; }
        public HasEditedCommand(NotesVM vm)
        {
            VM = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Notebook notebook = parameter as Notebook;
            VM.HasRenamed(notebook);
        }
    }
}
