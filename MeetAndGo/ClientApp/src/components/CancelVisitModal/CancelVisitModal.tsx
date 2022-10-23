import React from 'react';
import { useModalEffects } from '../../common/hooks/useModalEffects';
import { useCancelVisitMutation } from '../../app/api/meetgoApi';
import Spinner from '../Spinner';
import '../styles/modal.css';
import '../styles/cancelModal.css';
import '../styles/button.css';

const CancelVisitModal: React.FC<{ visitId: number, hideModal: VoidFunction, hasBooking: boolean }> = ({ visitId, hideModal, hasBooking }) => {
    const handleKeyPress = (event: any) => event.keyCode === 27 && !isLoading && hideModal();

    useModalEffects(handleKeyPress);

    const [cancelVisit, { isError, isLoading, isSuccess }] = useCancelVisitMutation();

    const renderCancelRequest = () =>
        <>
            {hasBooking && <p className='mw95 info-text text-failed'>
                UWAGA! Na wydarzenie jest już dokonana rezerwacja. W przypadku anulowania wydarzenia poinformujemy o tym rezerwującego e-mailem. <br />
                Sugerujemy przekazanie również informacji telefonicznie lub sms.
            </p>}
            <p className='mw95 info-text txtC'>
                Czy na pewno chcesz anulować wydarzenie?
            </p>
            <div className='cancel-modal-buttons'>
                <div className='btn cancel-modal-button' onClick={() => cancelVisit(visitId)}>Tak</div>
                <div className='btn cancel-modal-button' onClick={hideModal}>Nie</div>
            </div>
        </>

    const renderError = () =>
        <>
            <div className='mw95 info-text text-failed'>
                Przykro nam, ale wystąpił błąd techniczny podczas anulowania wydarzenia.
            </div>
            <div className='btn cancel-modal-button' onClick={hideModal}>OK</div>
        </>

    const renderSuccess = () =>
        <>
            <div className='info-text text-success'>
                Wydarzenie zostało anulowane.
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


export default CancelVisitModal;