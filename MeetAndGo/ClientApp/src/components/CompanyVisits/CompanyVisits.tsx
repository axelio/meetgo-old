import React, { useState } from 'react';
import { Link } from 'react-router-dom';

import { useGetCompanyVisitsQuery, useConfirmBookingMutation } from '../../app/api/meetgoApi';
import Spinner from '../Spinner';
import '../styles/button.css';
import './CompanyVisits.css';
import { CompanyVisit } from '../../app/api/types';
import { getNowDate, prepareDateToDisplay } from '../../common/helpers/dateHelpers';
import { showErrorNotification } from '../../common/helpers/toastHelpers';
import { preparePolishPhoneNumber } from '../../common/helpers/stringHelpers';
import CancelVisitModal from '../CancelVisitModal';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import BackBtn from '../BackBtn';
import { EventKind } from '../../app/api/apiConstants';

const CompanyVisits: React.FC<{ isUserLogged: boolean }> = ({ isUserLogged }) => {
    useScrollToTop();

    const { isLoading, isError, data, isSuccess } = useGetCompanyVisitsQuery();
    const [confirmBooking] = useConfirmBookingMutation();

    const [cancelModal, setCancelModal] = useState<{ isVisible: boolean, visitId: number }>({
        isVisible: false,
        visitId: NaN
    });

    const setCancelModalActive = (id: number) => setCancelModal({ isVisible: true, visitId: id });
    const disableCancelModal = () => setCancelModal({ isVisible: false, visitId: NaN });

    const now = getNowDate();

    const visits = data && [...data]
        .map((v): CompanyVisit => ({ ...v, startDate: new Date(v.startDate) }))
        .sort((a, b) => a.startDate.getTime() - b.startDate.getTime());

    const onCancelVisitBtnClicked = (visit: CompanyVisit) => {
        if (now < visit.startDate) {
            setCancelModalActive(visit.id);
        } else {
            showErrorNotification('Nie możesz anulować wydarzenia, którego godzina rozpoczęcia już minęła.');
        }
    }

    const onConfirmBookingBtnClicked = (visitId: number, bookingId: number) => confirmBooking({ visitId, bookingId });

    const tryRenderModal = () => {
        var visit = visits?.find(v => v.id === cancelModal.visitId);

        return (
            <>
                {cancelModal.isVisible && cancelModal.visitId &&
                    <CancelVisitModal
                        visitId={cancelModal.visitId}
                        hideModal={disableCancelModal}
                        hasBooking={visit ? Array.isArray(visit.bookings) && visit.bookings.length > 0 : false}
                    />
                }
            </>
        )
    }

    const renderPersons = (v: CompanyVisit) => v.eventKind === EventKind.Booking ? <>Max osób: {v.maxPersons}</> : <>Zapisanych: {v.bookingsNumber}/{v.maxPersons}</>

    const prepareBooking = (v: CompanyVisit) => {
        return v.bookings.map(b => (
            <div key={b.id} className='with-btn-container visit-booking'>
                <div>
                    {<div className='company-event-info-element active-booking'>Rezerwacja: {b.customerMail}</div>}
                    {b.customerPhoneNumber && <div className='company-event-info-element'>Tel: {preparePolishPhoneNumber(b.customerPhoneNumber)}</div>}
                    {b.customerName && <div className='company-event-info-element'>Imię: {b.customerName}</div>}
                    {b.customerName && <div className='company-event-info-element'>Kod: {b.code}</div>}
                </div>
                {!b.isConfirmed && v.requiresConfirmation && now < v.startDate && <div
                    className='btn visits-btn btn-confirm visits-btn-enabled'
                    onClick={() => onConfirmBookingBtnClicked(v.id, b.id)}>
                    Potwierdź
                </div>}
            </div>
        ));
    }

    return (
        <section className='section-container section-container-flex'>
            <BackBtn />
            <Link className='btn new-visit-btn' to="/add-new-visit">Nowa wizyta</Link>

            {isLoading && <Spinner />}

            {isError &&
                <div className='company-booking-error-info info-mobile'>{isUserLogged ? "Wystąpił błąd :(" : "Zaloguj się aby wyświetlić zawartość strony."}</div>
            }

            {isSuccess && (!data || data.length === 0) &&
                <div className='company-booking-empty-info info-mobile'>Obecnie nie ma żadnych nadchodzących wydarzeń</div>}

            {visits && visits.map(v =>
                <div className='company-booking-event-info-container info-mobile' key={v.id}>
                    <div className='company-booking-section'>
                        <div className='with-btn-container'>
                            <div>
                                <div className='company-event-info-title'>{v.eventName}</div>
                                <div className='company-event-info-date'>{prepareDateToDisplay(v.startDate)}</div>
                                <div className='company-event-info-element'>{v.price} ZŁ - {renderPersons(v)}</div>
                            </div>
                            <div
                                className={`btn visits-btn ${now < v.startDate ? 'btn-cancel-enabled' : 'btn-cancel-disabled'} `}
                                onClick={() => onCancelVisitBtnClicked(v)}>
                                Anuluj
                            </div>
                        </div>
                        {v.bookings && prepareBooking(v)}
                    </div>

                    <hr className='company-booking-hr-line' />
                </div>
            )}

            {tryRenderModal()}
        </section>
    );
}

export default CompanyVisits;