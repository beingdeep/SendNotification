# Azure Interview Assignment
## Assignment:
### Create an Azure Function app that processes data from a SQL Server database and sends notifications to users based on some criteria. The app should have the
following features:
- The app should be written in C# and use the latest version of the Azure
Functions SDK.
- The app should use a timer trigger to run every 15 minutes and query the
database for new or updated records.
- The app should use a connection string stored in an environment variable to
connect to the database.
- The app should use a stored procedure to retrieve the data from the database.
The stored procedure should accept a parameter for the last execution time
and return only the records that have been created or modified since then.
- The app should use a custom class to model the data returned by the stored
procedure. The class should have properties for the record ID, user ID, user
name, user email, data value, and notification flag.
- The app should use a service bus queue to send messages to another
function that handles the notifications. The messages should contain the user
ID, user name, user email, and data value as properties.
- The app should use dependency injection to register and resolve the service
bus client and any other services or configurations needed by the function.
- The app should use logging and error handling to capture any exceptions or
failures that occur during the execution of the function.
- The app should have unit tests that cover the main logic of the function and
mock any external dependencies.
### Evaluation Criteria:
The assignment will be evaluated based on the following criteria:
- The code quality, readability, and style of the C# code.
- The use of best practices and conventions for Azure Functions development.
- The correctness and completeness of the functionality and logic of the app.
- The test coverage and quality of the unit tests.
- The documentation and comments of the code.
