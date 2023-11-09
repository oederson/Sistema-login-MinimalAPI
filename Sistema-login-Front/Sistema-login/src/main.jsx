
import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App.jsx'
import './index.css'
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react'
import store from "./redux/store.js";
import Login from './Paginas/Login.jsx';
import Home from './Paginas/Home.jsx';
import Cadastrar from './Paginas/Cadastrar.jsx';

const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        path: '/',
        element: <Home />,
      },
      {
        path: '/login',
        element: <Login />,
      },
      {
        path: '/cadastrar',
        element: <Cadastrar />,
      }
    ]}]);
ReactDOM.createRoot(document.getElementById('root')).render(
  <Provider store={store}>
    
      <RouterProvider router={router} />
   
  </Provider>
)
