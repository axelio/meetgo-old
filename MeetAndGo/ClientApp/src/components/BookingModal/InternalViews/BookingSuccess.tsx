import React from 'react';
import '../BookingModal.css';
import '../../styles/modal.css';
import '../../styles/button.css';

const BookingSuccess: React.FC<{ hideModal: () => void, requiresConfirmation: boolean }> = ({ hideModal, requiresConfirmation }) => {
    return (
        <div className='modal-content booking-modal booking-info'>
            <div className='mw95'>
                <p className='info-text text-success'>
                    Wydarzenie zostało zarezerwowane pomyślnie. <br />
                    Zobacz swoje rezerwacje w zakładce w pasku menu na górze strony.
                </p>
                {requiresConfirmation && <p className='info-text text-failed'>
                    UWAGA! Rezerwacja musi zostać potwierdzona przez gospodarza. <br />
                    O potwierdzeniu powiadomimy Cię mailowo.
                </p>}
            </div>
            <div className='btn book-visit-button' onClick={hideModal}>OK</div>
        </div>
    )
}

export default BookingSuccess;