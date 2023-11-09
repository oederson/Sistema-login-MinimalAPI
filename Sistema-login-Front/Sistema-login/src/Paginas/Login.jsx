import { useEffect, useState } from "react";
import styled from "styled-components"
import { loginUser, logOutUser } from '../redux/user/actions';
import { useDispatch, useSelector } from "react-redux";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";


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
    const dispatch = useDispatch();
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    console.log(currentUser);
    
    const navigate = useNavigate();
    const handleClick = async (e) => {
        e.preventDefault();
        var res = await axios.create({baseURL: " https://localhost:7220"}).post("/login", {username, password} );
        if(res != null){
        dispatch(loginUser({token: res.data.token, role:res.data.role }))
        }

        };
      
    useEffect(() => {
       if(currentUser != null){
        navigate("/")
       }
    }, [currentUser]);
    
      

    
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
            <Link to="/cadastrar">Cadastrar um usuario</Link>            
        </Formulario>
    </Wrapper>
</Container>
  )
}

export default Login