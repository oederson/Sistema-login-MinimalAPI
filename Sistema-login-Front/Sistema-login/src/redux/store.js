import { createStore } from "@reduxjs/toolkit";
import rootReducer from './root-reducer.js'

const store = createStore(rootReducer); 
export default store;