import React from 'react';
import { selectBookingModalError } from '../../../app/store/bookingModal/bookingModalSlice';
import { useAppSelector } from '../../../app/store/hooks';
import { BookingErrorType } from '../../../app/store/bookingModal/types';
import '../BookingModal.css';
import '../../styles/modal.css';
import '../../styles/button.css';

const BookingError: React.FC<{ hideModal: () => void }> = ({ hideModal }) => {
    const error = useAppSelector(selectBookingModalError);

    const getErrorMessage = (error: BookingErrorType | undefined) => {
        switch (error) {
            case BookingErrorType.ALREADY_BOOKED:
                return <p>Przykro nam. Niestety to wydarzenie zostało właśnie zarezerwowane przez kogoś innego.</p>
            case BookingErrorType.TOO_MANY_BOOKINGS:
                return <p>Możesz dokonać tylko dwóch rezerwacji dziennie.</p>
            case BookingErrorType.SMS_FAILURE:
                return (
                    <p>
                        Wystąpił błąd podczas próby wysyłania SMS. Sprawdź czy został podany prawidłowy numer telefonu (zakładka Konto). <br /><br />
                        Spróbuj ponownie za chwilę. Jeżeli ten błąd się powtórzy - skontaktuj się z nami.
                    </p>
                );
            case BookingErrorType.ADD_PHONE_FAILURE:
                return (
                    <p>
                        Wystąpił niespodziewany błąd podczas próby dodania numeru telefonu. <br /><br />
                        Spróbuj dodać numer telefonu ponownie bezpośrednio w panelu Konto, a potem ponownie dokanaj rezerwacji.
                    </p>
                );
            case BookingErrorType.WRONG_SMS_TOKEN:
                return <p>Wprowadzono nieprawidłowy kod weryfikujący.</p>
            case BookingErrorType.DEFAULT:
            case undefined:
            default:
                return <p>Przepraszamy - wystąpił błąd podczas rezerwacji wydarzenia. Spróbuj ponownie.</p>
        }
    }

    return (
        <div className='modal-content booking-modal booking-info'>
            <div className='mw95 info-text text-failed'>
                {getErrorMessage(error)}
            </div>
            <div className='btn book-visit-button' onClick={hideModal}>OK</div>
        </div>
    )
}

export default BookingError;