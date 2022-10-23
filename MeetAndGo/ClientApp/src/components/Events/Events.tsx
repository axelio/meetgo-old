import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronCircleLeft, faChevronCircleRight } from '@fortawesome/free-solid-svg-icons';
import { getObjectFromStorage, setObjectInStorage } from '../../common/services/localStorage';
import { useGetEventsQuery } from '../../app/api/meetgoApi';
import { Event, GetEventsQueryParams } from '../../app/api/types';
import Spinner from '../Spinner';
import SingleEvent from '../SingleEvent';
import './Events.css';
import { getDateCalendarEnFormat, getTomorrowDate } from '../../common/helpers/dateHelpers';
import getDateForCalendar from './getDateForCalendar';
import { Cities } from '../../app/api/apiConstants';
import BookingModal from '../BookingModal';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import { useLocation } from 'react-router-dom';
import EventsHeader from '../EventsHeader';

const Events: React.FC<{ isUserLogged: boolean, isCompany: boolean }> = ({ isUserLogged, isCompany }) => {
  useScrollToTop();

  const location = useLocation();

  const [queryParams, setQueryParams] = useState<GetEventsQueryParams>({
    day: location?.search === "?showBusiness=true" ? '2025-12-10' : getDateForCalendar(),
    cityId: location?.search === "?showBusiness=true" ? Cities.Gdańsk : getObjectFromStorage('cityId') || Cities.Gdańsk,
    lastVisitId: null,
    categoryId: 0,
    timeOfDay: 0
  });

  const [bookingModal, setBookingModal] = useState<{ isVisible: boolean, event: Event | undefined }>({
    isVisible: false,
    event: undefined
  });

  const [page, setPage] = useState(1);

  const [pageLastIdQueryParam, setPageLastIdQueryParam] = useState<{ [page: number]: number | null }>({});

  const { isFetching, data } = useGetEventsQuery(queryParams);

  const setQueryParamsAndResetPage = (queryParams: GetEventsQueryParams) => {
    setQueryParams(queryParams);
    setPage(1);
  }

  const nextPageClicked = () => {
    setPageLastIdQueryParam({ ...pageLastIdQueryParam, [page]: queryParams.lastVisitId || null });
    setPage(page + 1);
    setQueryParams({ ...queryParams, lastVisitId: data?.lastVisitId || null });
  }

  const prevPageClicked = () => {
    setPage(page - 1);
    setQueryParams({ ...queryParams, lastVisitId: pageLastIdQueryParam[page - 1] })
  }

  const setNextDayDate = () => {
    const tomorrow = getTomorrowDate(queryParams.day);
    const calendarFormat = getDateCalendarEnFormat(tomorrow);
    setQueryParamsAndResetPage({ ...queryParams, day: getDateCalendarEnFormat(tomorrow), lastVisitId: null });
    setObjectInStorage('date', calendarFormat);
  }

  const setBookingModalActive = (event: Event) => setBookingModal({ isVisible: true, event });

  const disableBookingModal = () => setBookingModal({ isVisible: false, event: undefined });

  const events = data && data.events ? Object.values(data.events).sort((a, b) => a.order - b.order) : [];

  return (
    <>
      <EventsHeader queryParams={queryParams} setQueryParamsAndResetPage={setQueryParamsAndResetPage} />
      <section className='section-container section-container-flex ev-section'>
        {isFetching ? <Spinner /> :
          events.length > 0 ? events.map(event => <SingleEvent
            key={event.id}
            setBookingModalActive={setBookingModalActive}
            queryParams={queryParams}
            id={event.id}
            isUserLogged={isUserLogged}
            isCompany={isCompany} />) :
            <div className='empty-events'>
              Niestety nie mamy już więcej rezerwacji na ten dzień dla podanych filtrów wyszukiwania. :(
              {page === 1 && <p className='other-days' onClick={setNextDayDate}>Pokaż kolejny dzień</p>}
            </div>
        }

        <div className='navigation-arrows'>
          {page !== 1 && <FontAwesomeIcon onClick={prevPageClicked} className='nav-icon' size={"3x"} icon={faChevronCircleLeft} />}
          {data && data?.visitsCount === 30 && <FontAwesomeIcon onClick={nextPageClicked} className='nav-icon nav-icon--next' size={"3x"} icon={faChevronCircleRight} />}
        </div>

        {bookingModal.isVisible && bookingModal.event && <BookingModal event={bookingModal.event} params={queryParams} hideModal={disableBookingModal} />}
      </section>
    </>
  )
}

export default Events;
