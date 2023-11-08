import { createSlice } from "@reduxjs/toolkit";


const usuarioSlice = createSlice({
    name: "usuario",
    initialState:{
        currentUser : {
            token : "" 
        },
        isFeching: false,
        logado: false,
        error: false
    },
    reducers:{
        loginStart:(state)=>{
            state.isFeching=true;
        },
        loginSucess:(state,action)=>{
            state.isFeching=false;
            state.currentUser=action.payload;
            state.logado=true;
        },
        loginFailure:(state,action)=>{
            state.isFeching=false;
            state.error=true;
        },
        logout: (state) => {
            state.logado = false;
            state.currentUser = null;
            state.isFeching = false;
            state.error = false;
          },
       },
    },
);

export const {loginStart, loginSucess, loginFailure, logout} = usuarioSlice.actions
export default usuarioSlice.reducer;