import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { Cities } from '../../app/api/apiConstants';

import { setObjectInStorage, setValueInStorage } from '../../common/services/localStorage';
import { GetEventsQueryParams } from '../../app/api/types';
import { getNowDateCalendarEnFormat } from '../../common/helpers/dateHelpers';
import EventsAdvancedSearchFilters from '../EventsAdvancedSearchFilters';
import './EventsSearchBar.css';

type EventsSearchBarProps = {
    setDate: (date: string) => void,
    setCityId: (cityId: number) => void,
    queryParams: GetEventsQueryParams,
    setCategoryId: (categoryId: number) => void,
    setTimeOfDay: (timeOfDay: number) => void
}

const EventsSearchBar: React.FC<EventsSearchBarProps> = ({ queryParams, setDate, setCityId, setCategoryId, setTimeOfDay }) => {
    const [advancedSearchFiltersVisible, setAdvancedSearchFilters] = useState(false);

    const showAdvancedFilters = () => setAdvancedSearchFilters(true);

    const handleDateChange = (date: string) => {
        setDate(date);
        setObjectInStorage('date', date);
    }

    const handleCityChange = (cityId: string) => {
        setCityId(Number(cityId));
        setValueInStorage('cityId', cityId);
    }

    return (
        <div className='search-container'>
            <form className='search-bar'>

                <input className='element el-first'
                    type="date"
                    value={queryParams.day}
                    min={getNowDateCalendarEnFormat()}
                    onChange={e => handleDateChange(e.target.value)} />

                <select className='element'
                    value={queryParams.cityId}
                    onChange={e => handleCityChange(e.target.value)}>
                    {Object.entries(Cities).map(([cityName, id]) => <option key={cityName} value={id}>{cityName}</option>)}
                </select>

            </form>

            {!advancedSearchFiltersVisible &&
                <div className='search-more-filters' onClick={showAdvancedFilters}>
                    <FontAwesomeIcon className='more-filters-icon' icon={faPlus} />
                    Więcej filtrów
                </div>}

            {advancedSearchFiltersVisible && <EventsAdvancedSearchFilters
                queryParams={queryParams}
                setCategoryId={setCategoryId}
                setTimeOfDay={setTimeOfDay} />}
        </div>
    );
}

export default EventsSearchBar;