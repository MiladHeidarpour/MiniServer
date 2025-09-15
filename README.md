# MiniServer: Custom C# Web Server & TodoList API

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

A lightweight, from-scratch web server built in C# using `TcpListener`, featuring a full middleware pipeline, a simple Dependency Injection (DI) container, and an attribute-based routing system. This project also hosts a complete TodoList RESTful API to demonstrate the server's capabilities.

This project was developed as a deep dive into the underlying mechanics of web servers and the ASP.NET Core request pipeline.

## ✨ Features

-   **Custom HTTP Server:** Built from the ground up using `System.Net.Sockets.TcpListener`.
-   **Middleware Pipeline:** A flexible, chainable middleware architecture (`IMiddleware`) for processing HTTP requests.
-   **Dependency Injection:** A simple, custom DI container that supports Singleton and Transient lifetimes.
-   **Attribute-Based Routing:** Clean, declarative API routing using custom attributes like `[Route]`, `[FromBody]`, and `[FromRoute]`.
-   **RESTful API:** A full CRUD (Create, Read, Update, Delete) API for managing tasks.
-   **Decoupled Architecture:** A clean separation between the core server logic (`MiniServer.Core`) and the application logic (`TodoList` API).

## 🚀 Getting Started

To run this project locally, you will need the [.NET SDK](https://dotnet.microsoft.com/download) installed.

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git)
    cd YOUR_REPOSITORY_NAME
    ```

2.  **Run the application:**
    Navigate to the main project directory (the one containing the `.csproj` file for the TodoList app) and run the following command:
    ```bash
    dotnet run
    ```

3.  The server will start and listen on `http://localhost:8080`.

##  API Endpoints

The following endpoints are available to interact with the TodoList API.

| Method | Endpoint          | Description                 | Request Body Example                               |
| :----- | :---------------- | :-------------------------- | :------------------------------------------------- |
| `GET`  | `/tasks`          | Get all tasks.              | N/A                                                |
| `GET`  | `/tasks/{id}`     | Get a single task by its ID.| N/A                                                |
| `POST` | `/tasks`          | Create a new task.          | `{"title": "Learn about HTTP"}`                    |
| `PUT`  | `/tasks/{id}`     | Update an existing task.    | `{"title": "Build a Router", "isCompleted": true}` |
| `DELETE`| `/tasks/{id}`    | Delete a task by its ID.    | N/A                                                |

## 🛠️ Technology Stack

-   **.NET** (Console Application)
-   **C#**
