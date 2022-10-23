import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../app/store/hooks';
import { selectBookingModalView } from '../../app/store/bookingModal/bookingModalSlice';
import { BookingModalView } from '../../app/store/bookingModal/types';
import { Event, GetEventsQueryParams } from '../../app/api/types';
import AddPhoneNumber from './InternalViews/AddPhoneNumber';
import BookingError from './InternalViews/BookingError';
import BookingForm from './InternalViews/BookingForm';
import BookingModalSpinner from './InternalViews/BookingModalSpinner';
import VerifyPhoneNumber from './InternalViews/VerifyPhoneNumber';
import BookingSuccess from './InternalViews/BookingSuccess';
import AutoBooking from './InternalViews/AutoBooking';
import { resetState } from '../../app/store/bookingModal/bookingModalSlice';
import { useModalEffects } from '../../common/hooks/useModalEffects';
import './BookingModal.css';
import '../styles/modal.css';

type BookingModalParams = { event: Event, hideModal: () => void, params: GetEventsQueryParams }

const BookingModal: React.FC<BookingModalParams> = ({ event, hideModal, params }) => {
    const view = useAppSelector(selectBookingModalView);
    const dispatch = useAppDispatch();

    useEffect(() => {
        return () => {
            dispatch(resetState());
        }
    }, [dispatch]);

    const handleKeyPress = (event: any) => event.keyCode === 27 && view !== BookingModalView.SPINNER && hideModal();
    useModalEffects(handleKeyPress);

    const getView = () => {
        switch (view) {
            case BookingModalView.FORM:
                return <BookingForm event={event} params={params} hideModal={hideModal} />
            case BookingModalView.SPINNER:
                return <BookingModalSpinner />
            case BookingModalView.SUCCESS:
                return <BookingSuccess hideModal={hideModal} requiresConfirmation={event.requiresConfirmation} />
            case BookingModalView.ERROR:
                return <BookingError hideModal={hideModal} />
            case BookingModalView.ADD_PHONE_NUMBER:
                return <AddPhoneNumber hideModal={hideModal} />
            case BookingModalView.VERIFY_PHONE_NUMBER:
                return <VerifyPhoneNumber hideModal={hideModal} />
            case BookingModalView.AUTOBOOKING:
                return <AutoBooking event={event} params={params} />
            default:
                return <BookingForm event={event} params={params} hideModal={hideModal} />
        }
    }

    return (
        <div className='modal'>
            {getView()}
        </div>
    );
}


export default BookingModal;