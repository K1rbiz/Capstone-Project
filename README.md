# Capstone Project

A lightweight .NET MAUI Blazor Hybrid Application that lets users track books they own, plan to buy, or are currently reading — all stored locally in an SQLite database.
This project was build as part of our Capstone Project for our course ISP-3660
## Features
- Add, edit, and delete books from your collection.
- Categorize books as "Owned", "To Buy", or "Reading".
- View owned books in a visually appealing grid layout.
- Persistent local storage using SQLite.
- Clear all books with a single button.
- Clear individual book entries.
- Repository Pattern for clean architecture and maintainability.

## Technologies Used
- Frontend: .NET MAUI, Blazor, HTML, CSS
- Backend: C#, SQLite, Entity Framework Core
- Architecture: Repository Pattern + Dependency Injection
- IDE Used: Visual Studio 2022

## How it Works
When the app launches, MauiProgram.cs:
-Initializes SQLite (Batteries.Init()).
-Ensures the database exists (EnsureCreated()).
-Registers the LibraryContext and LibraryRepository for dependency injection.

The Home page (Home.razor) uses @inject LibraryRepository Repo to:
-Add new books with AddBookWithStatusAsync().
-Link each new book to a user entry in UserBooks.

The Owned Books Page (OwnedBooks.razor) retrieves and displays:
-All books marked as "Owned" using GetBooksByStatusAsync().
-Allows recdord removal or clearing all owned books.