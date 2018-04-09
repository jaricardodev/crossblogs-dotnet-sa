Challenge Statement

Cross-blogs is a backend blogging application written by a startup company called WritingForAll. It allows users to create / update / delete their articles, accepting comments for each article.

Notes:
- Articles have a 120 character limit for their title, and a 32k limit for their body.
- The frontend application is excluded from the current scope. It is a separate, fully-functional application handled by another team, so we do not want to modify it by performing any API modifications.

Your tasks:
- Increase unit test coverage to reach 70%, achieving more than 70% will only consume your valuable time without extra score.
- Find bugs and fix them, hint: we provided Cross-Blogs application in a good structure, so no need to spend your valuable time on structure modifications,  just focus on fixing bugs.
- Articles search endpoint is very slow.

Prerequisites:
- Any IDE
- .NET Core SDK 2.1.4
- MySQL >=5.5

=====================================
Development Environment
=====================================
MySQL:
- Cross-blogs application require a MySQL database to store it's data. Make sure to update the file "appsettings.json" file, changing the connection string named "CrossBlog" to reference your MySQL server.

Cross-blogs application:
- On any terminal move to the "crossblog" folder (the folder containing the "crossblog.csproj" file) and execute these commands:

dotnet restore
dotnet build
dotnet ef database update
dotnet run

- The application will be listening on http://localhost:5000
- Now you can call the api using any tool, like Postman, Curl, etc (See "Some Curl command examples" section)

=====================================
To run unit tests
=====================================
- On any terminal move to the "crossblog.tests" folder (the folder containing the "crossblog.tests.csproj" file) and execute these commands:

dotnet restore
dotnet build
dotnet test

- To check code coverage, execute the batch script:

coverage.bat

=====================================
Some Curl command examples
=====================================
curl -i -H "Content-Type: application/json" -X POST -d "{'title':'How to use docker', 'content':'xyz', 'date': '2018-03-10', 'published':false}" http://localhost:5000/articles
curl -i -H "Content-Type: application/json" http://localhost:5000/articles/search?title=doc