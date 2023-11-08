import axios from "axios";

const ChamadaApi = (token) => {
  
  const tipoResquisiçao = () =>{
    if(token){
        return axios.create({
            baseURL: "http://localhost:8080",
            headers: {
              Authorization: token,
            },
          });
    }else{
        return axios.create({
            baseURL: "https://localhost:7220",
        });
        }
  }
    return (tipoResquisiçao())
};

export default ChamadaApi;