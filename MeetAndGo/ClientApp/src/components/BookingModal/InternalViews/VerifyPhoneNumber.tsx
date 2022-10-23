import React, { useEffect } from 'react';
import { faTimesCircle } from '@fortawesome/free-regular-svg-icons';
import { useForm } from 'react-hook-form';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useRequestVerificationSmsMutation, useVerifyPhoneNumberMutation } from '../../../app/api/meetgoApi';
import '../BookingModal.css';
import '../../styles/modal.css';
import '../../styles/button.css';

const VerifyPhoneNumber: React.FC<{ hideModal: () => void }> = ({ hideModal }) => {
    const { register, handleSubmit } = useForm<{ token: string }>();
    
    const [verifyPhoneNumber] = useVerifyPhoneNumberMutation();
    const [requestToken] = useRequestVerificationSmsMutation();

    useEffect(() => {
        requestToken();
    }, [requestToken]);

    const onSubmit = (data: { token: string }) => verifyPhoneNumber(data.token);

    return (
        <form className='modal-content booking-modal' onSubmit={handleSubmit(onSubmit)}>
            <FontAwesomeIcon className='modal-close-icon' icon={faTimesCircle} onClick={hideModal} size={'3x'} />
            <div className='verify-number-header'>Jednorazowo zweryfikuj swój numer telefonu.</div>

            <div style={{ textAlign: 'center' }}>
                <p className='insert-token'>Wprowadź kod wysłany SMS</p>
                <input className='bookingmodal-input' {...register("token", { required: true })} />
            </div>

            <div style={{ textAlign: 'center', fontSize: 12 }}>
                Jeżeli nie otrzymałeś kodu sprawdź poprawność swojego numeru telefonu (zakładka Konto) i spróbuj ponownie.
            </div>
            <input className='btn book-visit-button' style={{ marginBottom: 25 }} type="submit" value="Potwierdź kod" />
        </form>
    )
}

export default VerifyPhoneNumber;