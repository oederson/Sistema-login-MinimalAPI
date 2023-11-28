import React from 'react'
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import styled from "styled-components";
import axios from "axios";
import { loginUser } from '../redux/user/actions';
import { Link, useNavigate } from "react-router-dom";

function Admin() {
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    const dispatch = useDispatch();
    const navigate = useNavigate();
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
        if(currentUser == null){
            navigate("/")
           }
        fetchData();
    }, []); 
  return (
    <div>
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

export default Admin