import { loginFailure, loginStart, loginSucess } from "./userReducer";
import ChamadaApi from "../services/metodoRequest.js";

export const loginApi = async ( usuario) =>{
  
    try{
        const res = await ChamadaApi().post("/login",usuario)
        
    }catch(err){
        
    }
}