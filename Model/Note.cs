using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NotesAPP.Model
{
    public class Note : INotifyPropertyChanged
    {
        /*Property list 
          id 
          notebookid
          title
          Datetime createdtime
          Datetime updatedtime
          filelocation
         
             */
        private int id;
        [PrimaryKey,AutoIncrement]

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }

        private int notebookid;
        [Indexed]

        public int NotebookId
        {
            get { return notebookid; }
            set { notebookid = value; OnPropertyChanged("Notebookid"); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }

        private DateTime createdTime;

        public DateTime CreatedTime
        {
            get { return createdTime; }
            set { createdTime = value; OnPropertyChanged("CreatedTime"); }
        }

        private DateTime updatedTime;

        public DateTime UpdatedTime
        {
            get { return updatedTime; }
            set { updatedTime = value; OnPropertyChanged("UpdatedTime"); }
        }

        private string filelocation;

        public string Filelocation
        {
            get { return filelocation; }
            set { filelocation = value; OnPropertyChanged("FileLocation"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
