using System;
using System.Linq;

namespace ToDoApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var actions = new ToDoActions();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-add":
                        actions.AddToDoItem(args);
                        break;
                    case "-remove_by_title":
                        actions.RemoveToDoItemByTitle(args[1]);
                        break;
                    case "-list_all":
                        actions.ListToDoItems();
                        break;
                    case "-mark_complete":
                        actions.MarkAsComplete(args[1]);
                        break;
                    case "-sort_date_ascending":
                        actions.SortByDateAscending();
                        break;
                    case "-sort_date_descending":
                        actions.SortByDateDescending();
                        break;
                    case "-update_by_day":
                        actions.BulkUpdateByDay(args);
                        break;
                    default:
                        ShowUserError();
                        break;
                }
            }
            else
            {
                ShowUserError();
            }
        }

        public static void ShowUserError()
        {
            Console.WriteLine("The command you passed in is not recognized, valid commands are:");
            Console.WriteLine("-add");
            Console.WriteLine("-remove_by_title");
            Console.WriteLine("-list_all");
            Console.WriteLine("-mark_complete");
            Console.WriteLine("-sort_date_ascending");
            Console.WriteLine("-sort_date_descending");
            Console.WriteLine("-update_by_day");
        }
    }
}
