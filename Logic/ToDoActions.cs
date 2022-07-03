using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Interfaces;
using ToDoApp.Models;

namespace ToDoApp.Logic
{
    public class ToDoActions : IToDoActions
    {
        private string FilePath { get; set; }
        private const string FileName = "TODO_Items.txt";

        /// <summary>
        /// Set the <see cref="FilePath"/> variable up in the constructor so we can use it across
        /// the class as we will need to access the file location multiple times.
        /// </summary>
        public ToDoActions()
        {
            // Get the path for MyDocuments and then set FilePath to this and the FileName.
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FilePath = Path.Combine(docPath, FileName);
        }

        /// <summary>
        /// Add a new <see cref="ToDoItem"/> as a new line in the TODO_Items.txt file.
        /// </summary>
        /// <param name="arguments">string array of the arguments passed in from the console.</param>
        public void AddToDoItem(string[] arguments)
        {
            // Convert the string array to a list of strings so we can easily remove elements with linq.
            var argumentList = arguments.ToList();

            // Remove all the commands from the list.
            argumentList.RemoveAll(x => x.StartsWith("-"));

            // If validation false then stop execution and inform the use .
            if (!ValidateInsertedItems(argumentList)) return;

            // Assign the properties the to the ToDoItem model, validation has already occurred
            // so we know there is only two elements in the list and the first will be the title
            // and the second has already been parsed as a valid DateTime so use that.
            var todoItem = new ToDoItem
            {
                Title = argumentList.ElementAtOrDefault(0),
                DueDate = Convert.ToDateTime(argumentList.ElementAtOrDefault(1)),
                Status = "undone"
            };

            // Append text to a file named "TODO_Items.txt", if it doesn't exist it will be created.
            using var outputFile = new StreamWriter(FilePath, true);
            outputFile.WriteLine(CreateNewToDoTextLine(todoItem.Title, todoItem.DueDate.ToShortDateString(), todoItem.Status));
        }

        /// <summary>
        /// Tries to find the corresponding ToDo_Item in the TODO_Items.txt, if any matches are found
        /// then they will all be removed, in-case of duplicates.
        /// </summary>
        /// <param name="title">The current ToDo_Item the user wants to remove from the document.</param>
        public void RemoveToDoItemByTitle(string title)
        {
            // Get the ToDo_Items.txt file and store each occurrence.
            List<string> allToDoItems = File.ReadAllLines(FilePath).ToList();

            // If validation fails then return out;
            if (!ValidateRemoveItems(allToDoItems, title)) return;

            // We know the title must occur in the TODO_Items.txt so remove all occurrences and save the file.
            allToDoItems.RemoveAll(x => x.Contains(title));
            File.WriteAllLines(FilePath, allToDoItems);
            Console.WriteLine($"Successfully removed all TODO items with the Title: {title}");
        }

        public void RemoveToDoItemByDate(string date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read all of the current ToDo_Items from TODO_Items.txt, if there is none then inform the
        /// user, otherwise we log them all to the console.
        /// </summary>
        public void ListToDoItems()
        {
            // Get all the TODO_items from the text file.
            List<string> allToDoItems = File.ReadAllLines(FilePath).ToList();

            if (!allToDoItems.Any())
            {
                Console.WriteLine("It seems there are no TODO items in your list, please add one.");
            }

            // Loop through the TODO_items and display to the user.
            foreach (var item in allToDoItems)
            {
                Console.WriteLine($"TODO: {item}");
            }
        }

        /// <summary>
        /// Get all the items in the TODO_Items.txt file, if any and find the match for the value
        /// passed as an argument, if nothing is found then inform the user that they may have
        /// mistyped and to try again. If a match is found then set the third item <see cref="ToDoItem.Status"/>
        /// as done.
        /// </summary>
        /// <param name="title">The title of the ToDo_Item that the user wants to mark as done.</param>
        public void MarkAsComplete(string title)
        {
            // Create and index so we can get the right line in the TODO_Items file, also get all the lines from the file.
            var index = 0;
            var allToDoItems = File.ReadAllLines(FilePath).ToList();

            var foundToDo = false;

            // Loop through all the TODO_Items and search for a match on the title.
            foreach (var item in allToDoItems)
            {
                // If there is a match found then we want to update it to done.
                if (item.Contains(title))
                {
                    // We have found the ToDo_Item so we can display the relevant message at the end.
                    foundToDo = true;

                    // Split the current item by the hyphen separator as that's how TODO_Items are created.
                    var split = item.Split(" - ");

                    // Create a new ToDoItem using the values from the split list.
                    var todoItem = new ToDoItem
                    {
                        Title = split[0],
                        DueDate = Convert.ToDateTime(split[1]),
                        Status = "done"
                    };

                    // Find the current item in the text file and overwrite it with the create a new line of text,
                    // this will be used to overwrite the current ToDo_Item.
                    allToDoItems[index] = CreateNewToDoTextLine(todoItem.Title, todoItem.DueDate.ToShortDateString(), todoItem.Status);
                    File.WriteAllLines(FilePath, allToDoItems);

                    // Leave the loop.
                    break;
                }

                index++;
            }

            Console.WriteLine(foundToDo
                ? $"Successfully marked {allToDoItems[index]} as done."
                : $"Could not find ToDo item: {title}, please ensure you have entered the name correctly.");
        }

        /// <summary>
        /// Get all the ToDo_Items and order them by ascending order on the Due Date.
        /// </summary>
        public void SortByDateAscending()
        {
            // Get all the ToDo_Items from the TODO_Items.txt file.
            List<ToDoItem> toDos = GetAllToDoItemsAsList(File.ReadAllLines(FilePath).ToList());

            // If not ToDo_Items exist then inform the user and stop execution.
            if (!toDos.Any())
            {
                Console.WriteLine("Sorry no ToDo items could be found, please ensure you have created at least one.");
                return;
            }

            // Sort the ToDo_Items in ascending order using the Due Date.
            var sortedToDos = toDos.OrderBy(x => x.DueDate);

            // Write to the console the ordered list.
            LogOrderedToDoItemsToConsole(sortedToDos);
        }

        /// <summary>
        /// Get all the ToDo_Items and order them by descending order on the Due Date.
        /// </summary>
        public void SortByDateDescending()
        {
            // Get all the ToDo_Items from the TODO_Items.txt file.
            List<ToDoItem> toDos = GetAllToDoItemsAsList(File.ReadAllLines(FilePath).ToList());

            // If not ToDo_Items exist then inform the user.
            if (!toDos.Any())
            {
                Console.WriteLine("Sorry no ToDo items could be found, please ensure you have created at least one.");
                return;
            }

            // Sort the list of ToDo_Items in descending order using the Due Date.
            var sortedToDos = toDos.OrderByDescending(x => x.DueDate);

            // Log the ordered list to the console.
            LogOrderedToDoItemsToConsole(sortedToDos);
        }

        /// <summary>
        /// Search for the corresponding day that is passed in through the console and if a match is
        /// found then update all occurrences for that day with the provided status argument.
        /// </summary>
        /// <param name="args">The arguments passed in from the console.</param>
        public void BulkUpdateByDay(string[] args)
        {
            // Convert the string array to a list and remove all the commands (-)
            var argumentList = args.ToList();
            argumentList.RemoveAll(x => x.StartsWith("-"));

            // Check the arguments are valid, if not stop execution.
            var valid = ValidateBulkUpdate(argumentList);

            if (!valid) return;

            // Get all the current ToDo_Items from the TODO_Items.txt file and create a list string from this.
            var toDoItems = GetAllToDoItemsAsList(File.ReadAllLines(FilePath).ToList());
            var allToDoItems = File.ReadAllLines(FilePath).ToList();

            // If either list does not contain elements then stop execution and inform the user.
            if (!toDoItems.Any() || !allToDoItems.Any())
            {
                Console.WriteLine("No ToDo items could be found, please ensure you have entered at least one.");
                return;
            }

            // We need an index so we know which line to update, and a bool to detect a match in the provided day.
            var index = 0;
            var foundDay = false;

            // Loop through the toDoItems list of ToDoItems.
            foreach (var toDoItem in toDoItems)
            {
                // If the first argument (day) is equal to the current toDoItems day then we have a match.
                if (argumentList.ElementAtOrDefault(0) == toDoItem.DueDate.DayOfWeek.ToString())
                {
                    foundDay = true;

                    // Create a new ToDoItem using the values from the current toDoItem and day of argument.
                    var todoItem = new ToDoItem
                    {
                        Title = toDoItem.Title,
                        DueDate = toDoItem.DueDate,
                        Status = argumentList.ElementAtOrDefault(1)
                    };

                    // Find the current item in the text file and overwrite it with the create a new line of text,
                    // this will be used to overwrite the current ToDo_Item.
                    allToDoItems[index] = CreateNewToDoTextLine(todoItem.Title, todoItem.DueDate.ToShortDateString(), todoItem.Status);
                }

                index++;
            }


            // If we have found a matching day then we update the TODO_Items.txt, otherwise inform the user no match was found.
            if (foundDay)
            {
                File.WriteAllLines(FilePath, allToDoItems);
                Console.WriteLine($"Successfully updated all entries for {argumentList.ElementAtOrDefault(0)}");
            }
            else
            {
                Console.WriteLine("No ToDo items could be found with the provided day.");
            }
        }

        /// <summary>
        /// Make sure when inserting a new ToDo_Item that the current count of the items isn't greater
        /// than 2 as we strip out any commands (-) so all that should be left is the <see cref="ToDoItem.Title"/>
        /// and the <see cref="ToDoItem.DueDate"/>.
        /// </summary>
        /// <param name="items">A list of the remaining arguments passed into the console.</param>
        /// <returns>A bool to indicate whether the ToDo_Item is valid or not.</returns>
        private bool ValidateInsertedItems(List<string> items)
        {
            // As we strip any items in the list with "-" (command) we know there should only be two parameters left.
            if (items.Count > 2)
            {
                Console.WriteLine("Adding new TODO items only supports two arguments, a title and date.");
                return false;
            }

            // Date is the second argument so make sure a valid date has been entered.
            var date = items.ElementAtOrDefault(1);
            var validDate = DateTime.TryParse(date, out var dt);

            // If the date string passed in through the console cannot be parsed then inform the user.
            if (!validDate)
            {
                Console.WriteLine("The date you passed in for Due Date could not be validated, please try again.");
                return false;
            }

            // If both validations pass then everything looks ok.
            return true;
        }

        /// <summary>
        /// Make sure that items exist in the TODO_Items.txt file and that the title that is passed
        /// in exists within the file, if not the item requested for removal is invalid.
        /// </summary>
        /// <param name="items">All ToDo_Items retrieved from the text file.</param>
        /// <param name="title">The title of the ToDo_Item requested to be removed.</param>
        /// <returns></returns>
        private bool ValidateRemoveItems(List<string> items, string title)
        {
            // If there are not items stored in the text file then inform the user to enter one.
            if (!items.Any())
            {
                Console.WriteLine("It appears there are no TODO items in your list, please enter at least one.");
                return false;
            }

            // If the list of items do not contain the current title then inform the user.
            if (!items.Any(x => x.Contains(title)))
            {
                Console.WriteLine($"The item with the provided title: {title} could not be found.");
                return false;
            }

            // If both validations pass then everything looks ok.
            return true;
        }

        /// <summary>
        /// Make sure that the arguments passed into the bulk update by day are valid, there should be
        /// no more than 2 and the day passed in should be a valid day (Monday-Sunday)
        /// </summary>
        /// <param name="items">The arguments taken from the console.</param>
        /// <returns>A bool to determine if the arguments are valid.</returns>
        public bool ValidateBulkUpdate(List<string> items)
        {
            // We strip out all the commands (-) so only two arguments should remain.
            if (items.Count > 2)
            {
                Console.WriteLine("Bulk updating by day only supports two arguments, a day and status.");
                return false;
            }

            // The first argument is the day so make sure it's a valid day.
            if (!Enum.IsDefined(typeof(DayOfWeek), items.ElementAtOrDefault(0) ?? string.Empty))
            {
                Console.WriteLine("The day you hae entered is not a valid one, please enter a valid day.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a new ToDo_Item to be entered into the TODO_Items.txt file, this can be used
        /// for new insertions or when updating an item to done.
        /// </summary>
        /// <param name="title">The title of the new/updated ToDo_Item.</param>
        /// <param name="dueDate">The Due Date of the new/updated ToDo_Item.</param>
        /// <param name="status">The Status of the new/updated ToDo_Item.</param>
        /// <returns></returns>
        private string CreateNewToDoTextLine(string title, string dueDate, string status)
        {
            // Use hyphen's as the separator so that we can split the string later.
            return $"{title} - {dueDate} - {status}";
        }

        /// <summary>
        /// Take all of the ToDo_Items from the TODO_Items.txt file, we check there are items in the list
        /// before this method call so no validation is needed here, loop over each ToDo_Item and split
        /// each item out using the "-" separator. Once we have split them out we can add new ToDoItem
        /// to the list of ToDoItems which we return to the user so it can be sorted.
        /// </summary>
        /// <param name="allToDoItems">All ToDo_Items found in the TODO_Items.txt file.</param>
        /// <returns><see cref="List{T}"/> of <see cref="ToDoItem"/>'s</returns>
        private List<ToDoItem> GetAllToDoItemsAsList(List<string> allToDoItems)
        {
            List<ToDoItem> toDos = new List<ToDoItem>();

            foreach (var allToDoItem in allToDoItems)
            {
                var split = allToDoItem.Split(" - ");

                // Create a new ToDoItem using the values from the split list.
                var todoItem = new ToDoItem
                {
                    Title = split[0],
                    DueDate = Convert.ToDateTime(split[1]),
                    Status = "done"
                };

                toDos.Add(todoItem);
            }

            return toDos;
        }

        /// <summary>
        /// Take the ordered list of <see cref="ToDoItem"/>'s and log to the console on a single line.
        /// </summary>
        /// <param name="orderedToDoItems">Ordered list of <see cref="ToDoItem"/>'s</param>
        private void LogOrderedToDoItemsToConsole(IOrderedEnumerable<ToDoItem> orderedToDoItems)
        {
            foreach (var toDoItem in orderedToDoItems)
            {
                Console.WriteLine($"{toDoItem.Title} - {toDoItem.DueDate.ToShortDateString()} - {toDoItem.Status}");
            }
        }
    }
}