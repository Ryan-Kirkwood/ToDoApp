
# ToDoApp
A console application that accepts a list of different commands along with arguments must be executed from the command line, change the C:\ToDoApp.exe to wherever you have stored the project.
## List of commands
"-add"
"-remove_by_title"
"-list_all"
"-mark_complete"
"-sort_date_ascending"
"-sort_date_descending"
"-update_by_day"

## Usage

|Command|Execution|
|--|--|
|-add|C:\ToDoApp.exe -add "todo" 02/07/2022|
|-remove_by_title|C:\ToDoApp.exe -remove_by_title "todo"|
|-list_all|C:\ToDoApp.exe -list_all|
|-mark_complete|C:\ToDoApp.exe -mark_complete "todo"|
|-sort_date_ascending|C:\ToDoApp.exe -sort_date_ascending|
|-sort_date_descending|C:\ToDoApp.exe -sort_data_descending|
|-update_by_day|C:\ToDoApp.exe -update_by_day "Saturday"|

## Notes

 - -add command takes two arguments, a title and a due date and should be given in that order.
 - -remove_by_title takes one argument and that is the title.
 - -list_all does not take any arguments and will just log all the current TODO items in the text file.
 - -mark_complete takes one argument which is the TODO item to update, if it is found in the text file then the status will be updated to done.
 - -sort_date_ascending takes no arguments and will log out all the TODO items in the text file in ascending order on the Due Date.
 - -sort_date_descending takes no arguments and will log out all the TODO items in the text file in descending order on the Due Date.
 - -update_by_day takes two argument which is a day (Monday-Sunday) and status, if any records matching the day are found then those days are updated with the status passed in.
