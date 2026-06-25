import { ChatWindow } from './components/ChatWindow'
import { Sidebar } from './components/Sidebar'

function App() {
  return (
    <div className="flex min-h-screen flex-col bg-slate-100 md:flex-row">
      <Sidebar />
      <main className="flex flex-1 flex-col">
        <ChatWindow />
      </main>
    </div>
  )
}

export default App
