import React from 'react';

import { useCancelBookingMutation } from '../../app/api/meetgoApi';
import { useModalEffects } from '../../common/hooks/useModalEffects';
import Spinner from '../Spinner';
import '../styles/modal.css';
import '../styles/cancelModal.css';
import '../styles/button.css';

const CancelBookingModal: React.FC<{ bookingId: number, hideModal: VoidFunction }> = ({ bookingId, hideModal }) => {
    const handleKeyPress = (event: any) => event.keyCode === 27 && !isLoading && hideModal();
    useModalEffects(handleKeyPress);

    const [cancelBooking, { isError, isLoading, isSuccess }] = useCancelBookingMutation();

    const renderCancelRequest = () =>
        <>
            <div className='mw95 info-text'>
                Czy na pewno chcesz anulować rezerwację?
            </div>
            <div className='cancel-modal-buttons'>
                <div className='btn cancel-modal-button' onClick={() => cancelBooking(bookingId)}>Tak</div>
                <div className='btn cancel-modal-button' onClick={hideModal}>Nie</div>
            </div>
        </>

    const renderError = () =>
        <>
            <div className='mw95 info-text text-failed'>
                Przykro nam, ale wystąpił błąd techniczny podczas anulowania rezerwacji. Skontakuj się z miejscem osobiście.
            </div>
            <div className='btn cancel-modal-button' onClick={hideModal}>OK</div>
        </>

    const renderSuccess = () =>
        <>
            <div className='mw95 info-text text-success'>
                Rezerwacja została anulowana.
            </div>
            <div className='btn cancel-modal-button' onClick={hideModal}>OK</div>
        </>


    const renderSpinner = () =>
        <>
            <div className='modal-spinner-container'>
                <Spinner />
            </div>
        </>

    const renderContent = () => {
        if (isLoading)
            return renderSpinner();
        else if (isError)
            return renderError();
        else if (isSuccess)
            return renderSuccess();
        else
            return renderCancelRequest();
    }

    return (
        <div className='modal'>
            <div className='modal-content cancel-modal'>
                {renderContent()}
            </div>
        </div>
    );
}


export default CancelBookingModal;