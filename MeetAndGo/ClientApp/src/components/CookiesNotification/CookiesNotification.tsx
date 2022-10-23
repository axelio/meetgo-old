import React from 'react';
import { Link } from 'react-router-dom';
import './CookiesNotification.css';

const CookiesNotification: React.FC<{ handleCookiesAccepted: Function }> = ({ handleCookiesAccepted }) => {
    return (
        <div className='cookies-notification'>
            <div>Ta strona wykorzystuje pliki cookie.</div>
            <Link className='cookies-privacy-link' to="/privacy">Zobacz politykę prywatności.</Link>
            <div className='cookies-ok-btn' onClick={() => handleCookiesAccepted()}>OK</div>
        </div>
    )
}

export default CookiesNotification;