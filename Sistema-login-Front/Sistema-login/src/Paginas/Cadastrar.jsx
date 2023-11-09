import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import styled from "styled-components"

const Container = styled.div`
    width: 100%;
    height: 100%;

    background-size:cover;
    display: flex;
    align-items: center;
    justify-content: center;
`;
const Wrapper = styled.div`
    width: 40%;
    padding: 20px;
    background-color: black;

`;
const Titulo = styled.h1`
    font-size: 24px;
    font-weight: 300;

`;
const Formulario = styled.form`
    display: flex;
    flex-wrap: wrap;
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
const Error = styled.span`
    color: red;
`;

const Cadastrar = () => { 

   
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
            var res = await axios.create({baseURL: " https://localhost:7220"}).post("/login", {username, password, repassword} );
        if(res != null){
        dispatch(loginUser({token: res.data.token, role:res.data.role }))
        }
            
            
        }catch{
            
        }
     };
 
    useEffect(() => {
       
    }, []);

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
                value={usuario.username}
                onChange={handleChange}
                required
                />
                <Input 
                label="password"
                placeholder="password"
                type="password"
                name="password"
                value={usuario.password}
                onChange={handleChange}
                required
                />
                <Input 
                label="repassword"
                placeholder="repassword"
                type="password"
                name="repassword"
                value={usuario.repassword}
                onChange={handleChange}
                required
                />
                
                <Concordar>Criando uma conta estou ciente dos termos de utilização.Escrevendo mais um monte de coisas</Concordar>
                
                <Botao type="submit">Criar conta</Botao>
                
            </Formulario>
            <Link to={"/"}>Voltar</Link>
        </Wrapper>
    </Container>
  )
}

export default Cadastrar