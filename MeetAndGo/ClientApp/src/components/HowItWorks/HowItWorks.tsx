import React from 'react';
import { Link } from 'react-router-dom';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import './HowItWorks.css';
import logo from '../../common/img/logo192.png'

const HowItWorks: React.FC = () => {
  useScrollToTop();

  return (
    <section className='section-container section-container-flex'>
      <img src={logo} alt="logo" />
      <div className='howitworks-content'>
        <h2 className='howitworks-header'>O meetgo</h2>

        <p className='howitworks-text'>
          Meetgo to platforma, która ułatwi Wam zaplanowanie wolnego czasu.
          Znajdziesz tu rezerwacje na wydarzenia i aktywności.
        </p>

        TODO

        <div className='howitworks-dots'>***</div>
        <div className='howitworks-info'>
          <div className='howitworks-info-inner'>Jeżeli chcesz się z nami skontaktować wyślij maila na adres: <a className='howitworks-link ' href="mailto: TODO">TODO</a></div>
          <Link className='howitworks-link howitworks-info-inner' to="/business">Współpraca</Link>
          <Link className='howitworks-link howitworks-info-inner' to="/regulations">Regulamin</Link>
          <Link className='howitworks-link howitworks-info-inner' to="/privacy">Polityka prywatności</Link>
          <Link className='howitworks-link howitworks-info-inner' to="/install">Zaintaluj aplikację</Link>
        </div>
      </div>
    </section>
  )
}

export default HowItWorks;