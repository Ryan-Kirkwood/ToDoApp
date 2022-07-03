using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Models
{
    public class ToDoItem
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "undone";
    }
}
