import React from 'react';
import { Link, useLocation } from 'react-router-dom';

import './Navbar.css';

type NavbarProps = {
    isUserLogged: boolean,
    isLoadingUser: boolean,
    isCompany: boolean
};

const Navbar: React.FC<NavbarProps> = ({ isLoadingUser, isUserLogged, isCompany }) => {
    const location = useLocation();

    const handleLogoClick = () => location.pathname === '/' && window.scrollTo(0, 0);

    const renderUserPanel = () => {
        return (
            <div className='user-panel'>
                {isUserLogged ?
                    <>
                        <Link className='nav-link' to={isCompany ? 'company-visits' : 'client-bookings'}>{isCompany ? 'Wydarzenia' : 'Rezerwacje'}</Link>
                        <a className='nav-link' href='./Identity/Account/Manage/'>Konto</a>
                        <a className='nav-link' href='./Identity/Account/Logout/'>Wyloguj</a>
                    </> :
                    <>
                        <a className='nav-link' style={{ fontWeight: 500 }} href='./Identity/Account/Register/'>Zarejestruj</a>
                        <a className='nav-link' style={{ fontWeight: 500 }} href='./Identity/Account/Login/'>Zaloguj</a>
                    </>}
            </div>)
    }

    return (
        <nav className='navbar'>
            <div className='nav-container'>
                <Link className='logo' onClick={handleLogoClick} to="/">meetgo</Link>
                {!isLoadingUser && renderUserPanel()}
            </div>
        </nav>
    )
}


export default Navbar;
