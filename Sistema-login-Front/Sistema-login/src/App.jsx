import './App.css'
import { Outlet } from 'react-router-dom'
import Login from './Paginas/Login'
import Cadastrar from './Paginas/Cadastrar'

function App() {
  
  return (
    <div>
    <Outlet />
    </div>
  )
}

export default App
