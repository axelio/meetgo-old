import React from 'react';

import '../styles/button.css';
import '../EventsSearchBar/EventsSearchBar.css';
import { Categories, TimeOfDay } from '../../app/api/apiConstants';
import { GetEventsQueryParams } from '../../app/api/types';
import { categoriesTranslation, timeTranslation } from './translations';

type EventsAdvancedSearchFiltersProps = {
    queryParams: GetEventsQueryParams,
    setCategoryId: (categoryId: number) => void,
    setTimeOfDay: (timeOfDay: number) => void
};

const EventsAdvancedSearchFilters: React.FC<EventsAdvancedSearchFiltersProps> = ({ queryParams, setCategoryId, setTimeOfDay }) => {
    return (
        <form className='search-bar adv-search-bar'>
            <select className='element el-first'
                value={queryParams.timeOfDay || 0}
                onChange={(e) => setTimeOfDay(Number(e.target.value))}>
                {Object.entries(TimeOfDay).map(([timeOfDay, id]) => <option key={id} value={id}>{timeTranslation[timeOfDay]}</option>)}
            </select>

            <select className='element '
                value={queryParams.categoryId || 0}
                onChange={(e) => setCategoryId(Number(e.target.value))}>
                {Object.entries(Categories).map(([category, id]) => <option key={id} value={id}>{categoriesTranslation[category]}</option>)}
            </select>
        </form>
    );
}

export default EventsAdvancedSearchFilters;