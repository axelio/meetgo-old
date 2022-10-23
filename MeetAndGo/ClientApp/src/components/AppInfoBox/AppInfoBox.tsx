import React from 'react';
import { faQuestionCircle } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Link } from 'react-router-dom';
import './AppInfoBox.css';

const AppInfoBox: React.FC = () => {
    return (
        <div className='app-info-box'>
            <div className='promo-text'>Zaplanuj swój wolny czas</div>
            <div>
                <FontAwesomeIcon className='how-it-works-icon' icon={faQuestionCircle} />
                <Link className='how-it-works' to="/how-it-works">Jak to działa?</Link>
            </div>
        </div>
    )
}

export default AppInfoBox;