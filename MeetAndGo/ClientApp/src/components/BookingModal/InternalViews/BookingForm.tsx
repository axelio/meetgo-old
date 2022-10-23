import React from 'react';
import { useForm } from 'react-hook-form';
import { faTimesCircle } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { useMakeBookingMutation } from '../../../app/api/meetgoApi';
import { Event, GetEventsQueryParams, Visit } from '../../../app/api/types';
import { getHours, transformCalendarDateFromEnToPl } from '../../../common/helpers/dateHelpers';
import '../BookingModal.css';
import '../../styles/modal.css';
import '../../styles/button.css';
import { EventKind } from '../../../app/api/apiConstants';

const BookingForm: React.FC<BookingFormProps> = ({ event, hideModal, params }) => {
    const { register, handleSubmit } = useForm<{ id: number }>();

    const [makeBooking] = useMakeBookingMutation();

    const onSubmit = (data: { id: number }) => makeBooking({ visitId: data.id, eventId: event.id, getEventsParams: params });

    const prepareOptionItem = (v: Visit) => event.kind === EventKind.Booking ?
        <option key={v.id} value={v.id}> {getHours(v.startDate)}, {v.price} ZŁ, {v.maxPersons} os.</option> :
        <option key={v.id} value={v.id}> {getHours(v.startDate)}, {v.price} ZŁ, 1 miejsce</option>;

    return (
        <form className='modal-content booking-modal' onSubmit={handleSubmit(onSubmit)}>
            <FontAwesomeIcon className='modal-close-icon' icon={faTimesCircle} onClick={hideModal} size={'3x'} />
            <div className='booking-event-name'>{event.name}</div>
            <div>
                <div className='booking-date'>{transformCalendarDateFromEnToPl(params.day)}</div>
                <select className='select-date' {...register("id")}>
                    {[...event.visits].map(prepareOptionItem)}
                </select>
            </div>
            <input className='btn book-visit-button' type="submit" value="Rezerwuj" />
            <div className='booking-informations'>
                <div className='booking-payment'>Za rezerwację zapłacisz na miejscu.</div>
            </div>
        </form>
    )
}

export default BookingForm;

type BookingFormProps = {
    event: Event,
    hideModal: () => void,
    params: GetEventsQueryParams
};