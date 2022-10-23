import React from 'react';
import { useState } from 'react';
import { EventKind } from '../../app/api/apiConstants';
import { useGetClientBookingsQuery } from '../../app/api/meetgoApi';
import { CustomerBooking } from '../../app/api/types';
import { prepareDateToDisplay } from '../../common/helpers/dateHelpers';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import BackBtn from '../BackBtn';
import CancelBookingModal from '../CancelBookingModal';
import Spinner from '../Spinner';
import '../styles/button.css';
import './CustomerBookings.css';

const CustomerBookings: React.FC<{ isUserLogged: boolean }> = ({ isUserLogged }) => {
    useScrollToTop();

    const { isLoading, isError, data, isSuccess } = useGetClientBookingsQuery();

    const [cancelModal, setCancelModal] = useState<{ isVisible: boolean, bookingId: number }>({
        isVisible: false,
        bookingId: NaN
    });

    const setCancelModalActive = (id: number) => setCancelModal({ isVisible: true, bookingId: id });
    const disableCancelModal = () => setCancelModal({ isVisible: false, bookingId: NaN });

    const bookings = data && [...data]
        .map((b): CustomerBooking => ({ ...b, visitStartDate: new Date(b.visitStartDate) }))
        .sort((a, b) => new Date(a.visitStartDate).getTime() - new Date(b.visitStartDate).getTime());

    const displayPplInfo = (b: CustomerBooking) => b.eventKind === EventKind.Booking ? `Max osób: ${b.maxPersons}` : `Osób: ${b.bookingsNumber}/${b.maxPersons}`

    return (
        <section className='section-container section-container-flex'>
            <BackBtn />
            {isLoading && <Spinner />}

            {isError &&
                <div className='customer-booking-error-info info-mobile'>{isUserLogged ? 'Wystąpił błąd :(' : 'Zaloguj się aby wyświetlić zawartość strony.'}</div>
            }

            {isSuccess && (!data || data.length === 0) &&
                <div className='customer-booking-empty-info info-mobile'>Obecnie nie masz żadnych rezerwacji.</div>}

            {bookings && bookings.map(b =>
                <div className='customer-booking-event-info-container info-mobile' key={b.id}>
                    <div className='customer-booking-section'>
                        <div>
                            <div className='customer-event-info-title'>{b.eventName}</div>
                            <div className='customer-event-info-element' style={{ color: b.isConfirmed ? 'green' : 'red' }}>
                                {b.isConfirmed ? 'Rezerwacja potwierdzona' : 'Rezerwacja czeka na potwierdzenie'}
                            </div>
                            <div className='customer-event-info-date'>{prepareDateToDisplay(b.visitStartDate)}</div>
                            <div className='customer-event-info-element'>Kod: {b.code}</div>
                            <div className='customer-event-info-element'>{b.price} ZŁ, {displayPplInfo(b)}</div>
                            <div className='customer-event-info-element'>{b.companyName}</div>
                        </div>
                        <div className='btn cancel-booking-button' onClick={() => setCancelModalActive(b.id)}>Anuluj</div>
                    </div>
                    <hr className='customer-booking-hr-line' />
                </div>
            )}

            {cancelModal.isVisible && cancelModal.bookingId && <CancelBookingModal bookingId={cancelModal.bookingId} hideModal={disableCancelModal} />}

        </section>
    );
}

export default CustomerBookings;