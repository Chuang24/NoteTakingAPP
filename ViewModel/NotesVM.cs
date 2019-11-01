using NotesAPP.Model;
using NotesAPP.ViewModel.commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesAPP.ViewModel
{
    public class NotesVM
    {
        public ObservableCollection<Notebook> Notebooks { get; set; }
        private Notebook selectedNotebook
            ;

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                //get notes
            }
        }

        public ObservableCollection<Note> Notes { get; set; }

        public NewNotebookCommand NewNotebookCommand { get; set; }
        public NewNoteCommand NewNoteCommand { get; set; }

        public NotesVM()
        {
            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);
        }

        public void CreateNoteBook()
        {
            Notebook newNotebook = new Notebook() {
                Name = "New Notebook"
            };
            DatabaseHelper.Insert(newNotebook);
        }

        public void CreateNote(int notebookID)
        {
            Note newNote = new Note()
            {
                Notebookid = notebookID,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Title = "New Note"
            };

            DatabaseHelper.Insert(newNote);
        }
    }

}
