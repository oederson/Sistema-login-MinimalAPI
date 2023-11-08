import { useEffect, useState } from "react";
import styled from "styled-components"
import { loginApi } from "../redux/apiChamada";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";


const Container = styled.div`

`;
const Wrapper = styled.div`


`;
const Titulo = styled.h1`
    font-size: 24px;
    font-weight: 300;

`;
const Formulario = styled.form`
    display: flex;
    flex-direction: column;
`;
const Input = styled.input`
    flex:1;
    min-width: 40%;
    margin: 10px 0px;
    padding: 10px;
`;

const Botao = styled.button`
    width: 40%;
    border:none;
    padding: 15px 20px;
    background-color: teal;
    color: white;
    cursor: pointer;
    margin-bottom: 10px;
    &:disabled{
        color: green;
        cursor: not-allowed;
    }
`;
const Linka = styled.a`
    margin: 5px 0px;
    font-size: 12px;
    text-decoration: underline;
    cursor: pointer;
`;
const Error = styled.span`
    color: red;
`;

const Login = () => {
    
    const [username, setUsuario] = useState("");
    const [password, setSenha] = useState("");
    
    
    const navigate = useNavigate();
    const handleClick = async (e) => {
        e.preventDefault();
        await loginApi( { username, password})
        };
      
    useEffect(() => {
       
    }, []);
    
      

    
  return (
    <Container>
    <Wrapper>
        <Titulo>Entrar</Titulo>
        <Formulario>
            <Input 
            placeholder="Nome de usuario" 
            onChange={(e)=>setUsuario(e.target.value)}
            />
            <Input 
            placeholder="senha"
            type="password"
            autocomplete="current-password" 
            onChange={(e)=>setSenha(e.target.value)}
            />
            <Botao onClick={handleClick} >Entrar</Botao>
            
            <Linka>Esqueceu a senha ?</Linka>
            <Linka>Criar nova conta</Linka>
            <Linka>Vai ter que ter um eu nao sou robo</Linka>
        </Formulario>
    </Wrapper>
</Container>
  )
}

export default Login