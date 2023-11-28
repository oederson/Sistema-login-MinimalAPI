
import styled  from 'styled-components';
import { useSelector, useDispatch } from 'react-redux';
import { Link,useNavigate } from 'react-router-dom';
import { logOutUser } from '../src/redux/user/actions';
import { useEffect, useState } from 'react';

const Container = styled.div`
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
@media screen and (max-width: 1000px){
      width: 100%;
}
    
`;
const Wrapper = styled.div`
    width: 100%;
    padding: 0px 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    
`;
const Esquerda = styled.div`
    width: 33.3%;
    height: 30px;
    display: flex;
    align-items: center;
    margin-top: 32px;
    margin-bottom: 0px;
    `;
const Botao = styled.button`
    border-radius: 8px;
    border: 1px solid black;
    padding: 0.6em 1.2em;
    font-size: 1em;
    font-weight: 500;
    font-family: inherit;
    background-color: white;
    cursor: pointer;
    transition: border-color 0.25s;
    `;
const Procura = styled.div`
    width: 50%;
    margin-top: 5px;
    margin-left: 25px;
    padding: 5px;
    `;
const InputContainer = styled.div`
    width: 200%;
    height: 40px;
    background-color: white;
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-right:200px;

    border: 2px solid teal;
`;
const Icone = styled.div`
    position: absolute;

    height: 40px;
    border-radius: 50%;
    color: teal;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-left: 460px;
    font-size: 40;
    @media screen and (max-width: 1000px){
      color: white;
      }
`;
const Centro = styled.div`
    display: flex;
    width: 33.3%;
    text-align: center;
    align-items: center;
    justify-content: center;
    `;
const Logo = styled.h1`
    color: #008080;
    font-weight: bold;
    text-shadow: 1px 2px 2px #0a0a0a;
    `;
const Direita = styled.div`
    width: 33.3%;
    flex-direction: row;
    display: flex;
    text-align: center;
    align-items: center;
    justify-content: flex-end;
    `;
const ItemMenu = styled.div`
    font-size: 14px;
    cursor: pointer;
    margin-left: 25px;
    `;
const Titulo = styled.h3`
    color: teal;
    font-size: 20px;
    font-weight: bold;
    text-shadow: 0.5px 1px 0.5px #014d1a;
    @media screen and (max-width: 750px){
        font-size:14px;
      }
`;
const Logomarca = styled.img`
    width: 300px;
    height: 90px;
    @media screen and (max-width: 1000px){
      width: 200px;
      height: 55px;
      }
`;

const NavBar = () => {
    const dispatch = useDispatch();
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    const [logOutUsuario, setLogout] = useState("");
    const navigate = useNavigate(); 
    const handleLogOutClick = () => {
        dispatch(logOutUser()
        ,setLogout("1")
        )
    }
    useEffect(() => {
        if(logOutUsuario != "")
        {
            navigate("/")  
        }
     }, [logOutUsuario]); 
  
    
    const oQueFazerNaBarraDeNagevacao = () => {
        if(currentUser != null){
            return(<Container>
                        <Wrapper>
                           <Esquerda>
                            <Link to= {currentUser.role == "Admin" ? "/admin" : "/"} style={{ textDecoration: 'none' }}>
                             <a>{currentUser.role}</a>
                             </Link>
                           </Esquerda>
                            <Direita>
                                <ItemMenu>
                                <Botao>
                                <div onClick={handleLogOutClick }>Logout</div>
                                
                                </Botao>
                                </ItemMenu>
                            </Direita>
                        </Wrapper>
                        <hr/>  
                    </Container>)
            }else{
    return(<Container>
                <Wrapper>
                    <Esquerda>
                        <Link to="/" style={{ textDecoration: 'none' }}>
                        <a>ninguem logado</a>
                        </Link>                           
                </Esquerda>
                    <Direita>
                        <ItemMenu>
                        <Botao>
                        <Link to={"/login"} style={{ textDecoration: 'none' }}>LOGIN</Link>
                        </Botao>
                        </ItemMenu>
                        <ItemMenu>
                        <Botao>
                        <Link to="/cadastrar" style={{ textDecoration: 'none' }}>Cadastrar</Link> 
                        </Botao>
                        </ItemMenu>
                    </Direita>
                </Wrapper>
                <hr/>  
            </Container>)
        }
}
    
  return (oQueFazerNaBarraDeNagevacao())
}

export default NavBar