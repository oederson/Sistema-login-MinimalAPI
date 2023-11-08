import './App.css'
import { Outlet } from 'react-router-dom'
import Login from './Paginas/Login'

function App() {
  
  return (
    <div>
    <Outlet />
    <Login />
    </div>
  )
}

export default App
