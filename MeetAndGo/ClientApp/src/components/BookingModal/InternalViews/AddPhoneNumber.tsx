import React from 'react';
import { useForm } from 'react-hook-form';
import { faTimesCircle } from '@fortawesome/free-regular-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useAddPhoneNumberMutation } from '../../../app/api/meetgoApi';
import '../BookingModal.css';
import '../../styles/modal.css';
import '../../styles/button.css';

const AddPhoneNumber: React.FC<{ hideModal: () => void }> = ({ hideModal }) => {
    const { register, handleSubmit, formState: { errors } } = useForm<{ phoneNumber: string }>();
    const [addPhoneNumber] = useAddPhoneNumberMutation();
    const onSubmit = (data: { phoneNumber: string }) => addPhoneNumber(data.phoneNumber);

    return (
        <form className='modal-content booking-modal' onSubmit={handleSubmit(onSubmit)}>
            <FontAwesomeIcon className='modal-close-icon' icon={faTimesCircle} onClick={hideModal} size={'3x'} />
            <div style={{ textAlign: 'center' }}>
                <p className='polish-phone-number'>Polski numer telefonu (bez +48)</p>
                <input className='bookingmodal-input' type="tel" {...register("phoneNumber", { pattern: /^[0-9]{9}$/, required: true })} />
            </div>
            <input className='btn book-visit-button' style={{ marginBottom: 25 }} type="submit" value="Dodaj numer" />
            <div className='text-failed' style={{ visibility: errors && errors.phoneNumber ? 'visible' : 'hidden' }}>
                Nieprawidłowy numer telefonu.
            </div>
        </form>
    )
}

export default AddPhoneNumber;