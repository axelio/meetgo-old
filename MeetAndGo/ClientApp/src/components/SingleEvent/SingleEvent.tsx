import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBuilding, faClock, faCreditCard, faUser, faWindowMaximize, faCompass } from '@fortawesome/free-regular-svg-icons';
import LazyLoad from 'react-lazyload';

import { useGetEventsQuery } from "../../app/api/meetgoApi";
import './SingleEvent.css';
import '../styles/button.css';
import '../Events/Events.css';
import Spinner from '../Spinner';
import { Event, GetEventsQueryParams, Visit } from '../../app/api/types';
import { getHours } from '../../common/helpers/dateHelpers';
import { showErrorNotification } from '../../common/helpers/toastHelpers';
import { prepareWebsiteToDisplay } from '../../common/helpers/stringHelpers';
import { EventKind } from '../../app/api/apiConstants';
import { getDistance } from '../../common/helpers/locationHelpers';
import { useAppSelector } from '../../app/store/hooks';
import { selectLocation } from '../../app/store/location/locationSlice';

type SingleEventParams = {
    queryParams: GetEventsQueryParams,
    id: number,
    setBookingModalActive: (event: Event) => void;
    isUserLogged: boolean,
    isCompany: boolean
}

const SingleEvent: React.FC<SingleEventParams> = ({ queryParams, id, setBookingModalActive, isUserLogged, isCompany }) => {
    const { event } = useGetEventsQuery({ ...queryParams }, {
        selectFromResult: ({ data }) => ({ event: data?.events[id] })
    })
    const [imageLoaded, setImageLoaded] = useState(false);

    const coordinates = useAppSelector(selectLocation);

    if (!event) return null;

    const visits: Visit[] = [...event.visits]
        .map((v): Visit => ({ ...v, startDate: new Date(v.startDate) }))
        .sort((a, b) => a.startDate.getTime() - b.startDate.getTime());

    const onBookButtonClicked = () => {
        if (isUserLogged && !isCompany)
            setBookingModalActive({ ...event, visits: visits })
        else
            showErrorNotification(!isUserLogged ? 'Zaloguj się aby móc rezerwować.' : 'Jako firma nie możesz rezerwować.');
    }

    const renderDate = (visit: Visit) => {
        if (event.durationInMinutes) {
            const end = new Date(visit.startDate);
            end.setMinutes(visit.startDate.getMinutes() + event.durationInMinutes);
            return <>{getHours(visit.startDate)} - {getHours(end)}</>;
        } else {
            return <>{getHours(visit.startDate)}</>
        }
    }

    const { name, description, pictureUrl } = event;
    const { companyName, street, number, district, website, longitude, latitude } = event.address;

    const renderPersons = (v: Visit) => event.kind === EventKind.Booking ? <>max osób: {v.maxPersons}</> : <>osób: {v.bookingsNumber}/{v.maxPersons}</>

    return (
        <>
            <div className='visit-container'>
                <div className='lazy-picture-wrapper'>
                    <LazyLoad once >
                        <img width={312} height={234} className={`picture ${!imageLoaded ? 'picture-hidden' : ''}`} src={pictureUrl} alt="" onLoad={() => setImageLoaded(true)} />
                        {!imageLoaded && <div className='spinner-container'><Spinner /></div>}
                    </LazyLoad>
                </div>

                <div className='visit-info'>
                    <div className='event-name'>{name}</div>
                    <div className='event-address'>
                        <div><FontAwesomeIcon className='event-icon' icon={faBuilding} />{companyName} - {street} {number} ({district})</div>
                        {coordinates && longitude && latitude && <div style={{ marginTop: '6px' }}><FontAwesomeIcon className='event-icon' icon={faCompass} />{getDistance(coordinates, { latitude, longitude })}km od Ciebie</div>}
                    </div>

                    <div className='event-description'>{description}</div>
                    {website && <div className='event-website'>
                        <FontAwesomeIcon className='event-icon' icon={faWindowMaximize} />
                        <a className='event-www' href={website} target="_blank" rel="noreferrer">{prepareWebsiteToDisplay(website)}</a>
                    </div>}
                    {(visits.map(v =>
                        <div key={v.id} className='event-details'>
                            <div><FontAwesomeIcon className='event-icon' icon={faClock} />{renderDate(v)}</div>
                            <div><FontAwesomeIcon className='event-icon' icon={faCreditCard} />{v.price} ZŁ</div>
                            <div><FontAwesomeIcon className='event-icon' icon={faUser} />{renderPersons(v)}</div>
                        </div>
                    ))}

                    {event.visits.length > 0 && <div
                        className={`btn book-button ${isUserLogged && !isCompany ? 'book-button-enabled' : 'book-button-disabled'}`}
                        onClick={onBookButtonClicked}>
                        Rezerwuj
                    </div>}

                </div>
            </div>
            <hr className='hr-line' />
        </>
    )
}

export default SingleEvent;