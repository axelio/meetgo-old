import React, { useEffect } from 'react';
import { selectBookingVisitId } from '../../../app/store/bookingModal/bookingModalSlice';
import { useAppSelector } from '../../../app/store/hooks';
import { useMakeBookingMutation } from '../../../app/api/meetgoApi';
import { Event, GetEventsQueryParams } from '../../../app/api/types';
import BookingModalSpinner from './BookingModalSpinner';

const AutoBooking: React.FC<{ event: Event, params: GetEventsQueryParams }> = ({ event, params }) => {
    const [makeBooking] = useMakeBookingMutation();

    const visitId = useAppSelector(selectBookingVisitId);

    useEffect(() => {
        if (visitId) makeBooking({ visitId: visitId, getEventsParams: params, eventId: event.id });
    });

    return (
        <BookingModalSpinner />
    )
}



export default AutoBooking;