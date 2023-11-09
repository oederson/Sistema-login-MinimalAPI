import React from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { loginUser, logOutUser } from '../redux/user/actions';
import { Link } from 'react-router-dom';

function Home() {
    const { currentUser } = useSelector(rootReducer => rootReducer.userReducer);
    console.log(currentUser);
    const dispatch = useDispatch();
    const handleLoginClick = () => {
        dispatch(loginUser({token: "Vai receber o token", role:"role para controle no front" }))
    }
    const handleLogOutClick = () => {
        dispatch(logOutUser())
    }
  return (
    <div>
        Home
        <button>
            {currentUser == null ? (<Link to={"/login"}>LOGIN</Link>) : (<div onClick={handleLogOutClick}>Logout</div>)}               
        </button>
    </div>
  )
}

export default Home