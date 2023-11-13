import React from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { loginUser, logOutUser } from '../redux/user/actions';
import { Link } from 'react-router-dom';
import { useEffect, useState } from "react";
import axios from "axios";

function Home() {
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    console.log(currentUser);
    const dispatch = useDispatch();
    const [dataList, setDataList] = useState([]);
    const handleLoginClick = () => {
        dispatch(loginUser({token: "Vai receber o token", role:"role para controle no front" }))
    }
    const handleLogOutClick = () => {
        setDataList([])
        dispatch(logOutUser())
    }
    const fetchData = async () => {
        try {
            if(currentUser != null){
            const token = currentUser.token;            
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            const response = await axios.create({baseURL: "https://localhost:7023"}).get("/usuarios");
            setDataList(response.data);
            }
        } catch (error) {
             
        if (error.response && error.response.status === 403) {
            console.log('Acesso negado:', error);
        } else {
            // Outros erros
            console.log('Erro na requisição:', error);
        }
        }
    };
    console.log(dataList)
    console.log(dataList.length)
    useEffect(() => {
        fetchData();
    }, []); 

  return (
    <div>
        Home
        <button>
            {currentUser == null ? (<Link to={"/login"}>LOGIN</Link>) : (<div onClick={handleLogOutClick}>Logout</div>)}
            {}               
        </button>
        {dataList.length == 0 ? <div > A lista de usuarios so é mostrada se for um admin e estiver logado</div> :<div>
            <ul>
                    {dataList.map(item => (
                        <li key={item.id}>{item.userName}</li>
                        
                    ))}
                </ul>
            </div>
        }
    </div>
  )
}

export default Home