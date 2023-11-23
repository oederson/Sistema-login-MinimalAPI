import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import styled from "styled-components";
import axios from "axios";
import { loginUser } from '../redux/user/actions';
import { Link, useNavigate } from "react-router-dom";

const Container = styled.div`
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
`;
const Wrapper = styled.div`
    width: 40%;
    padding: 20px;
`;
const Titulo = styled.h1`
    font-size: 24px;
    font-weight: 300;
`;
const Formulario = styled.form`
    display: flex;
    flex-wrap: wrap;
    flex-direction: column;
`;
const Input = styled.input`
    flex:1;
    min-width: 40%;
    margin: 20px 10px 0px 0px;
    padding: 10px;
`;
const Concordar = styled.span`
    font-size: 12px;
    margin: 20px 0px;
`;
const Botao = styled.button`
    width: 40%;
    border:none;
    padding: 15px 20px;
    background-color: teal;
    color: white;
    cursor: pointer;
`;
const Botoes = styled.div`
  display:flex;
  justify-content: space-around;
  height: 45px; 
`;

const Cadastrar = () => { 
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [usuario, setUsuario] = useState({
        username: "",
        password: "",
        repassword: ""
      }); 

    const handleChange = (e) => {
        const { name, value } = e.target;
        setUsuario((prevState) => ({ ...prevState, [name]: value }));
    };
    const fazOsubmit = async(e) => {
        e.preventDefault();
        try{
            var res = await axios.create({ baseURL: "https://localhost:7023" }).post("/registro", {
            username: usuario.username,
            password: usuario.password,
            repassword: usuario.repassword
        });
        if(res != null){
        dispatch(loginUser({token: res.data.token, role:res.data.role }))
        }                      
        }catch (error) {
            console.error("Erro durante a requisição:", error);
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
            <Titulo>Criar Conta</Titulo>
            <Formulario  className="formulario" onSubmit={fazOsubmit}>
                <Input 
                label="username"
                placeholder="username"
                type="text"
                name="username"
                
                onChange={handleChange}
                required
                />
                <Input 
                label="password"
                placeholder="password"
                type="password"
                name="password"
                
                onChange={handleChange}
                required
                />
                <Input 
                label="repassword"
                placeholder="repassword"
                type="password"
                name="repassword"
                
                onChange={handleChange}
                required
                />
                
                <Concordar>Criando uma conta estou ciente dos termos de utilização.Escrevendo mais um monte de coisas</Concordar>
                <Botoes>
                <Botao type="submit">Criar conta</Botao>
                <Botao>
                <Link to={"/"} style={{ textDecoration: 'none', color: "white"}}>Voltar</Link>
                </Botao>
                </Botoes>
            </Formulario>
        </Wrapper>
    </Container>
  )
}

export default Cadastrar