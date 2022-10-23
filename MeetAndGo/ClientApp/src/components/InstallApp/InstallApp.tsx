import React from 'react';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import './InstallApp.css';
import logo from '../../common/img/logo192.png'

const InstallApp: React.FC = () => {
  useScrollToTop();
  return (
    <section className='section-container section-container-flex'>
      <img src={logo} alt="logo" />
      <div className='installapp-content'>
        <h2 className='installapp-header'>Dodaj meetgo do ekranu głównego.</h2>

        <h3 className='installapp-system'>Android</h3>
        <ol className='installapp-list installapp-text'>
          <li>Otwórz przeglądarkę Chrome</li>
          <li>Kliknij ustawienia - trzy pionowe kropki obok adresu strony</li>
          <li>W menu wybierz "Dodaj do ekranu głównego".</li>
        </ol>

        <h3 className='installapp-system'>iOS</h3>
        <ol className='installapp-list installapp-text'>
          <li>Otwóz Safari</li>
          <li>Naciśnij na przycisk udostępnij znajdjąćy się na dole przeglądarki.</li>
          <li>Wybierz dodaj do ekranu głównego.</li>
        </ol>

        <p className='installapp-text'>Możesz dodać naszą apkę do ekranu głównego i cieszyć się doświadczeniem aplikacji mobilnej. Jeśli chcesz wiedzieć więcej jak to działa wyszukaj w google: "PWA".</p>

      </div>
    </section>
  )
}

export default InstallApp;