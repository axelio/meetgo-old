import React from 'react';
import { Link } from 'react-router-dom';
import './Footer.css';

const Footer: React.FC = () => {
    return (
        <footer>
            <div className='footer-links-container'>
                <Link className='footer-link' to="/contact">Kontakt</Link>
                <Link className='footer-link footer-install' to="/install">Zainstaluj aplikację</Link>
                <Link className='footer-link' to="/business">Współpraca</Link>
            </div>
            <div className='footer-links-container'>
                <Link className='footer-link' to="/regulations">Regulamin</Link>
                <Link className='footer-link' to="/privacy">Polityka prywatności</Link>
                <div className='footer-txt'>2022 - meetgo &copy;</div>
            </div>
        </footer>
    )
}

export default Footer;