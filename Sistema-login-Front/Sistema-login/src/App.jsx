import { Outlet } from 'react-router-dom'
import NavBar from '../Componentes/Navbar'
import styled from 'styled-components'

const Container = styled.div`
    width: 100%;
    height : 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}`;

function App() {
  
  return (
    <Container>
      <NavBar/>
      <Outlet />
    </Container>
  )
}

export default App
