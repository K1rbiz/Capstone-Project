# Capstone Project
Our application is a personal book management system that helps users keep track of their reading habits and book collections.

A lightweight .NET MAUI Blazor Hybrid Application that lets users track books they own, plan to buy, or are currently reading — all stored locally in an SQLite database.
This project was build as part of our Capstone Project for our course ISP-3660
## Features
- Add, edit, and delete books from your collection.
- Categorize books as "WishList", "Owned", "Reading" or "Finished".
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
- Initializes SQLite (Batteries.Init()).
- Ensures the database exists (EnsureCreated()).
- Registers the LibraryContext and LibraryRepository for dependency injection.

The Home page (Home.razor) uses @inject LibraryRepository Repo to:
- Add new books with AddBookWithStatusAsync().
- Link each new book to a user entry in UserBooks.
- Added ISBN lookup button and tab (Entering the ISBN of a book will fill in the tabs such as title and author etc...)

The Owned Books Page (OwnedBooks.razor) retrieves and displays:
- All books marked as "Owned" using GetOwnedAsync();
- Allows recorded removal or clearing all owned books.
- 

The Wishlist Books Page (Wishlist.razor) retrieves and displays:
- All books marked as "Wishes" using GetWishlistAsync();
- Allows recorded removal or clearing all owned books.

The Login Page (Login.razor) 
- Upon running the application it displays the login page.
- Users can create new or login to existing profiles.
- Passwords are stored locally through secure storage.
- 

The API Being used
- We are consuming an API via HTML and code called "https://openlibrary.org/isbn/%7Bisbn%7D.json"
- 
