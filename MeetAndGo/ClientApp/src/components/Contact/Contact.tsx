import React from 'react';
import './Contact.css';
import logo from '../../common/img/logo192.png';

const Contact: React.FC = () => {
    return (
        <section className='section-container section-container-flex'>
            <div className='contact-content'>
                <ul className='contact-list contact-text'>
                    <li>E-mail: <a className='contact-link' href="mailto: TODO">TODO@</a></li>
                </ul>
            </div>
            <img src={logo} alt="logo" />
        </section>
    )
}

export default Contact;