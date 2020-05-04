# Bot Template
by the idiot named lord_dashh.
It is by no means correct or any good but
it works pretty well for me and that's what i care about.

## Getting Started
create the resource dir
`mkdir Resources`
then create a config.json
`vi Resources/config.json # or whatever`

## Project Structure
- Program.cs 	- Entry point of the application, put nothing here;
- Bot.cs 		- Main bot here (setup, event handlers, etc.)
- Database.cs 	- Simple wrapper around SQLiteConnection.
- Modules/ 		- Put all module code (commands) here.
- Services/ 	- Put services (long lasting multithreaded shit) here.
- Objects/ 		- Put all data/config sturctures here, *no* code! Only data!
- Objects/Sql 	- Same as above but for Sql objects only.
- Objects/Json 	- Same as above but for Json objects only.
- Managers/ 	- This is important, these are static classes that each manage a Objects/Sql object.
