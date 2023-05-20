Groove Box
Overview

Groove Box is a desktop application that serves as a YouTube clone, allowing users to post videos, subscribe to authors, leave likes, and watch videos. This README provides an overview of the app's features, architecture, and setup instructions.
Features

    User authentication using ASP.NET Core API with Microsoft Identity JWT.
    Email sender functionality for password reset and email change.
    Integration with SQL database for storing user information and authentication data.
    Integration with MongoDB for accessing video files, genres, and other app data.

Architecture

The Groove Box app follows a client-server architecture with the following components:

    Desktop App: The front-end of the application is coded in C# using .NET MAUI (Multi-platform App UI).
    ASP.NET Core API: The back-end of the application built using ASP.NET Core, providing endpoints for user authentication, video management, and other app functionalities.
    Microsoft Identity: A framework used for authentication and authorization, storing user information and authentication data in a SQL database.
    MongoDB: A NoSQL database used for storing video files, genres, and other app data.

Setup Instructions

To set up the Groove Box app locally, follow these steps:

    Clone the repository: $ git clone https://github.com/HilthonTT/GrooveBox.git
    Navigate to the project directory: $ cd groove-box
    Configure the SQL database connection in the app's configuration file.
    Set up and configure MongoDB. Ensure the MongoDB connection string is correctly set in the app's configuration file.
    Build and run the API server: $ dotnet run
    Build and run the desktop app: $ dotnet maui run

Note: Make sure you have the necessary dependencies and development environment set up to run .NET MAUI and ASP.NET Core applications.
Contributions

Contributions to Groove Box are welcome! If you find any issues or have ideas for new features, please submit them through the issue tracker or submit a pull request with your proposed changes.
License

This project is licensed under the MIT License. See the LICENSE file for more details.
