import React from 'react';
import Spinner from '../../Spinner';
import '../BookingModal.css';
import '../../styles/modal.css';

const BookingModalSpinner: React.FC = () => {
    return (
        <div className='modal-content booking-modal'>
            <div className='modal-spinner-container'>
                <Spinner />
            </div>
        </div>
    )
}



export default BookingModalSpinner;