import React from 'react';
import { Link } from 'react-router-dom';
import { GetEventsQueryParams } from '../../app/api/types';
import AppInfoBox from '../AppInfoBox';
import EventsSearchBar from '../EventsSearchBar';

import './EventsHeader.css';

type EventsHeaderProps = {
    queryParams: GetEventsQueryParams,
    setQueryParamsAndResetPage: (queryParams: GetEventsQueryParams) => void
}

const EventsHeader: React.FC<EventsHeaderProps> = ({ queryParams, setQueryParamsAndResetPage }) => {
    const isMobile = window.innerWidth < 768;
    
    const setDate = (date: string) => setQueryParamsAndResetPage({ ...queryParams, day: date, lastVisitId: null });
    const setCityId = (cityId: number) => setQueryParamsAndResetPage({ ...queryParams, cityId: cityId, lastVisitId: null });
    const setCategoryId = (categoryId: number) => setQueryParamsAndResetPage({ ...queryParams, categoryId: categoryId, lastVisitId: null });
    const setTimeOfDay = (timeOfDay: number) => setQueryParamsAndResetPage({ ...queryParams, timeOfDay: timeOfDay, lastVisitId: null })

    return (
        <div className='events-header'>
            <AppInfoBox />
            <EventsSearchBar
                queryParams={queryParams}
                setDate={setDate}
                setCityId={setCityId}
                setTimeOfDay={setTimeOfDay}
                setCategoryId={setCategoryId} />
            {isMobile && <Link className='install-app-promo' to="/install">Zainstaluj apkę</Link>}
        </div>
    )
}

export default EventsHeader;