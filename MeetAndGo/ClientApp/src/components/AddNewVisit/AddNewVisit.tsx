import React from 'react';
import { useForm } from 'react-hook-form';

import { useAddNewVisitMutation, useGetEventNamesQuery } from '../../app/api/meetgoApi';
import { NewVisit } from '../../app/api/types';
import { getNowDateCalendarTimeEnFormat } from '../../common/helpers/dateHelpers';
import { useScrollToTop } from '../../common/hooks/useScrollToTop';
import './AddNewVisit.css';
import Spinner from '../Spinner';
import { AddNewVisitErrorType } from './types';
import BackBtn from '../BackBtn';

const AddNewVisit: React.FC<{ isUserLogged: boolean }> = ({ isUserLogged }) => {
    useScrollToTop();
    const { register, handleSubmit } = useForm<{ visit: NewVisit }>();
    const [addNewVisit, { isError: isAddingError, isLoading: isAddingNewEvent, error: addError }] = useAddNewVisitMutation();
    const { isFetching: isFetchingNames, data: eventNames, isError: isErrorNamesFetch } = useGetEventNamesQuery();
    const onSubmit = (data: { visit: NewVisit }) => addNewVisit(data.visit)

    const renderFetchingNamesError = () => {
        return (
            <p>Przepraszamy - wystąpił niespodziewany błąd podczas próby pobrania danych do formularza. <br />
                Odśwież stronę i spróbuj ponownie.</p>
        );
    }

    const prepareAddVisitError = (error: any) => {
        if (error && error.data === AddNewVisitErrorType.NOT_ACTIVE)
            return <p className='add-visit-error'>Konto twojej firmy nie zostało w pełni aktywowane. Skontaktuj się z nami.</p>
        else if (error && error.data === AddNewVisitErrorType.TOO_MANY)
            return <p className='add-visit-error'>Przekroczyłeś maksymalną ilość terminów na ten dzień.</p>
        else
            return <p className='add-visit-error'>Przepraszamy, wystąpił błąd podczas dodawania terminu. Sprawdź poprawność daty. Spróbuj ponownie.</p>
    }

    const renderForm = () => {
        return (
            <>
                <BackBtn link={"company-visits"} />
                <form className='add-visit-form' onSubmit={handleSubmit(onSubmit)}>
                    {isAddingError && prepareAddVisitError(addError)}

                    <label className='add-visit-element'>
                        <div className='add-visit-label'>Wydarzenie:</div>
                        <select {...register("visit.eventId", { required: true })} className='add-visit-input'>
                            {eventNames && eventNames.map(en => <option key={en.id} value={en.id}>{en.name}</option>)}
                        </select>
                    </label>

                    <label className='add-visit-element'>
                        <div className='add-visit-label'>Data i godzina:</div>
                        <input {...register("visit.date", { required: true })}
                            className='add-visit-input'
                            type="datetime-local"
                            min={getNowDateCalendarTimeEnFormat()} />
                    </label>

                    <label className='add-visit-element'>
                        <div className='add-visit-label'>Cena (ZŁ):</div>
                        <input {...register("visit.price", { required: true })}
                            className='add-visit-input'
                            type="number"
                            step="any"
                            min="0" />
                    </label>

                    <label className='add-visit-element'>
                        <div className='add-visit-label'>Max osób:</div>
                        <input {...register("visit.maxPersons")}
                            className='add-visit-input'
                            type="number" />
                    </label>

                    <input type="submit" className='btn submit-visit-btn' />
                </form>
            </>
        )
    }

    const renderContent = () => {
        if (!isUserLogged)
            return <p>Zaloguj się aby skorzystać z tej funkcjonalności.</p>
        if (isAddingNewEvent || isFetchingNames)
            return <Spinner />
        else if (isErrorNamesFetch)
            return renderFetchingNamesError();
        else
            return renderForm();
    }

    return (
        <section className='section-container section-container-flex'>
            {renderContent()}
        </section>
    )
}

export default AddNewVisit;