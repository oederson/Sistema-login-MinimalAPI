import React from 'react'
import { useSelector } from 'react-redux'
import { useEffect, useState } from "react";
import axios from "axios";

function Home() {
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);

    const [dataList, setDataList] = useState([]);

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

    useEffect(() => {
        fetchData();
    }, []); 

  return (
    <div>
        Home
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