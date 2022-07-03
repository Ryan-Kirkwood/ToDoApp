using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Interfaces
{
    public interface IToDoActions
    {
        public void AddToDoItem(string[] args);
        public void RemoveToDoItemByTitle(string title);
        public void RemoveToDoItemByDate(string date);
        public void ListToDoItems();
        public void MarkAsComplete(string title);
        public void SortByDateAscending();
        public void SortByDateDescending();
        public void BulkUpdateByDay(string[] args);
    }
}
