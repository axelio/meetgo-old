import React, { useState, Suspense, lazy } from 'react';
import { BrowserRouter as Router, Redirect, Route, Switch } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ReactGA from 'react-ga';

import { getObjectFromStorage, setValueInStorage } from '../../common/services/localStorage';
import { useGetUserClaimsQuery } from '../../app/api/meetgoApi';
import CookiesNotification from '../CookiesNotification';
import Footer from '../Footer';
import Navbar from '../Navbar';
import Spinner from '../Spinner';
import './App.css';
import { useSetLocation } from '../../common/hooks/useSetLocation';

const App: React.FC = () => {
  ReactGA.initialize('UA-210085081-1');
  ReactGA.pageview(window.location.pathname);

  useSetLocation();
  
  const { isLoading, isSuccess, data } = useGetUserClaimsQuery();
  const [cookiesAccepted, setCookiesAccepted] = useState(getObjectFromStorage('cookiesAccepted') || false)
  const isCompany = !!data?.["Company"];

  const handleCookiesAccepted = () => {
    setValueInStorage('cookiesAccepted', true);
    setCookiesAccepted(true);
  }

  const Events = lazy(() => import('../Events'));
  const CompanyVisits = lazy(() => import('../CompanyVisits'));
  const Contact = lazy(() => import('../Contact'));
  const CustomerBookings = lazy(() => import('../CustomerBookings'));
  const HowItWorks = lazy(() => import('../HowItWorks'));
  const Privacy = lazy(() => import('../Privacy'));
  const AddNewVisit = lazy(() => import('../AddNewVisit'));
  const Regulations = lazy(() => import('../Regulations'));
  const InstallApp = lazy(() => import('../InstallApp'));
  const Business = lazy(() => import('../BusinessPage'));

  const FallBack = () => <section className='section-container section-container-flex'><Spinner /></section>

  return (
    <Router>
      <div className='app-container'>
        <Navbar isLoadingUser={isLoading} isUserLogged={isSuccess} isCompany={isCompany} />
        <Suspense fallback={<FallBack />}>
          <Switch>
            <Route exact path="/">
              <Events isUserLogged={isSuccess} isCompany={isCompany} />
            </Route>
            <Route exact path='/company-visits' >
              <CompanyVisits isUserLogged={isSuccess} />
            </Route>
            <Route exact path='/add-new-visit' >
              <AddNewVisit isUserLogged={isSuccess} />
            </Route>
            <Route exact path='/client-bookings'>
              <CustomerBookings isUserLogged={isSuccess} />
            </Route>
            <Route exact path='/privacy' component={Privacy} />
            <Route exact path='/contact' component={Contact} />
            <Route exact path='/how-it-works' component={HowItWorks} />
            <Route exact path='/regulations' component={Regulations} />
            <Route exact path='/install' component={InstallApp} />
            <Route exact path='/business' component={Business} />
            <Route exact path='/wspolpraca' component={Business} />
            <Redirect to="/" />
          </Switch>
          {!cookiesAccepted && <CookiesNotification handleCookiesAccepted={handleCookiesAccepted} />}
          <ToastContainer />
        </Suspense>
        <Footer />
      </div>
    </Router>
  )
}

export default App;
