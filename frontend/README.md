# RAG Document Assistant — Frontend

A React + TypeScript + Tailwind CSS chat UI for the SPAssistant RAG backend.

## Prerequisites

- [Node.js](https://nodejs.org/) 18 or later
- The ASP.NET Core backend running locally (see below)

## Quick start

### 1. Start the backend

```bash
cd ../backend/SPRagAPI
dotnet run
```

The API runs at `http://localhost:5042` (and `https://localhost:7163`).

### 2. Install and run the frontend

```bash
cd frontend
npm install
npm run dev
```

Open [http://localhost:5173](http://localhost:5173) in your browser.

During development, Vite proxies `/Chat` and `/Documents` to the backend, so you do not need to configure CORS for the proxy path. CORS is enabled on the backend for `http://localhost:5173` if you prefer calling the API directly.

## Environment variables

Copy `.env.example` to `.env.local` if you want to call the backend without the Vite proxy:

```bash
cp .env.example .env.local
```

Set `VITE_API_BASE_URL=http://localhost:5042` in `.env.local`.

## Features

- **Chat interface** — ask questions and receive AI-generated answers
- **Source documents** — each answer shows linked sources with title, snippet, and URL
- **Loading & error states** — visual feedback while waiting or on failure
- **Sync Documents** — separate sidebar action that calls `POST /Documents/sync`
- **Responsive layout** — sidebar on desktop, stacked header on mobile

## Project structure

```
src/
├── components/
│   ├── ChatInput.tsx      # Question input and send button
│   ├── ChatWindow.tsx     # Message list and chat state
│   ├── ErrorBanner.tsx    # Dismissible error display
│   ├── LoadingDots.tsx    # Typing indicator
│   ├── MessageBubble.tsx  # User/assistant message bubbles
│   ├── Sidebar.tsx        # Branding, features, sync action
│   ├── SourceCard.tsx     # Single source document card
│   ├── SourceList.tsx     # List of sources under an answer
│   └── SyncButton.tsx     # Document sync (independent of chat)
├── lib/
│   └── api.ts             # fetch wrappers for /Chat and /Documents/sync
├── types/
│   └── api.ts             # TypeScript types matching backend DTOs
├── App.tsx                # Layout shell
├── main.tsx               # React entry point
└── index.css              # Tailwind CSS import
```

## API endpoints used

| Method | Endpoint           | Purpose                          |
|--------|--------------------|----------------------------------|
| POST   | `/Chat`            | Send a question, get answer + sources |
| POST   | `/Documents/sync`  | Re-index documents from OneDrive |

## Build for production

```bash
npm run build
npm run preview
```

For production deployment, set `VITE_API_BASE_URL` to your deployed API URL before building.

## Scripts

| Command         | Description              |
|-----------------|--------------------------|
| `npm run dev`   | Start dev server         |
| `npm run build` | Type-check and build     |
| `npm run preview` | Preview production build |
