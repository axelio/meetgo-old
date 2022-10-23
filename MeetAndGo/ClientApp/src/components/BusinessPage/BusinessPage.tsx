import React from 'react';
import { Link } from 'react-router-dom';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import logo from '../../common/img/logo192.png'
import './BusinessPage.css';

const BusinessPage: React.FC = () => {
    useScrollToTop();

    return (
        <>
            <div className='business-img-header'>
                <div className='business-promo-text'>Dobra zabawa służy biznesom!</div>
            </div>

            <section className='section-container section-container-flex business-section'>
                TODO
            </section>
        </>
    )
}

export default BusinessPage;