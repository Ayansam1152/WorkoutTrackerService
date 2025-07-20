# 🏋️‍♂️ Workout Tracker API

A full-stack **Workout Tracking API** built with **.NET 8**, **PostgreSQL (Supabase)**, and **JWT-based authentication**. Designed to allow users to manage personalized workout plans, schedule sessions, and track progress — all through a clean, RESTful interface.

---


## 🚀 Features

- 🧑‍💻 User authentication (sign up, login, JWT secured)
- 📋 Create & manage workout plans
- 🏃 Schedule workouts for specific dates & times
- 📈 Generate progress reports from past sessions
- ✅ Soft delete support
- 🌐 Swagger UI for API testing
- 🤖 **LLM Chat Endpoint**: Ask coding, fitness, or general questions via `/v1/workout/llm/chat` (powered by OllamaSharp & Llama 3)

---


## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL (Supabase)
- **Security**: JWT Auth
- **ORM**: Entity Framework Core
- **LLM**: OllamaSharp, Llama 3 (local LLM integration)
- **Containerized**: Docker, Docker Hub

---

## 🤖 LLM Chat API Usage

Send a POST request to `/v1/workout/llm/chat` with `{ "prompt": "your question here" }` to get concise, well-formatted answers (including code) for fitness, coding, or general queries.

---

## 🧱 Database Design

The database schema includes:

- `Users`
- `Exercise` (meta-data)
- `Workout` (user-created)
- `WorkoutExercise` (many-to-many mapping)
- `WorkoutSchedule` (scheduling with status)
