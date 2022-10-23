import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import { showSuccesNotification, showErrorNotification } from '../../common/helpers/toastHelpers';
import { CompanyVisit, CustomerBooking, EventName, GetEventsQueryParams, GetEventsQueryResponse, MakeBookingParams, NewVisit } from './types';
import { setBookingModalError, setBookingModalView, setBookingVisitId } from '../store/bookingModal/bookingModalSlice';
import { BookingErrorType, BookingModalView, ErrorApi } from '../store/bookingModal/types';
import { EventKind } from './apiConstants';

const apiUrl = !process.env.NODE_ENV || process.env.NODE_ENV === 'development' ?
    'https://localhost:44395/api' :
    `${document.location.origin}/api`;

const prepareGetEventsQueryParams = (arg: GetEventsQueryParams) => {
    const { day, cityId, lastVisitId, categoryId, timeOfDay } = arg;
    let params: GetEventsQueryParams = { day, cityId };
    if (lastVisitId) params = { ...params, lastVisitId };
    if (categoryId) params = { ...params, categoryId };
    if (timeOfDay) params = { ...params, timeOfDay };
    return params;
}

export const meetgoApi = createApi({
    reducerPath: 'meetgoApi',
    baseQuery: fetchBaseQuery({ baseUrl: apiUrl }),
    tagTypes: ['Event', 'CustomerBooking', 'CompanyVisit'],
    endpoints: (builder) => ({
        getUserClaims: builder.query<{ [key: string]: string }, void>({
            query: () => 'auth/me'
        }),
        getEvents: builder.query<GetEventsQueryResponse, GetEventsQueryParams>({
            query: (arg) => {
                return {
                    url: 'event',
                    params: prepareGetEventsQueryParams(arg),
                }
            },
            providesTags: ['Event']
        }),
        getEventNames: builder.query<EventName[], void>({
            query: () => 'event/names'
        }),
        getClientBookings: builder.query<CustomerBooking[], void>({
            query: () => {
                return {
                    url: 'booking/client'
                }
            },
            providesTags: ['CustomerBooking']
        }),
        getCompanyVisits: builder.query<CompanyVisit[], void>({
            query: () => {
                return {
                    url: 'visit/company'
                }
            },
            providesTags: ['CompanyVisit']
        }),
        makeBooking: builder.mutation<void, MakeBookingParams>({
            query: (params) => ({
                url: `booking/make/${params.visitId}`,
                method: 'POST',
            }),
            invalidatesTags: ['CustomerBooking'],
            async onQueryStarted(params, { dispatch, queryFulfilled }) {
                const result = dispatch(
                    meetgoApi.util.updateQueryData('getEvents', params.getEventsParams, (draft) => {
                        const event = draft.events[params.eventId]
                        const index = event.visits.findIndex(v => v.id === params.visitId);

                        if (index === -1) return;
                        event.kind === EventKind.Booking ? event.visits.splice(index, 1) : event.visits[index].bookingsNumber++;
                    })
                )
                try {
                    dispatch(setBookingModalView(BookingModalView.SPINNER));
                    await queryFulfilled;
                    dispatch(setBookingModalView(BookingModalView.SUCCESS));
                } catch (ex: any) {
                    result.undo();
                    if (ex && ex.error && ex.error.data === ErrorApi.NO_PHONE_NUMBER) {
                        dispatch(setBookingVisitId(params.visitId));
                        dispatch(setBookingModalView(BookingModalView.ADD_PHONE_NUMBER));
                    }
                    else if (ex && ex.error && ex.error.data === ErrorApi.PHONE_NUMBER_NOT_VERIFIED) {
                        dispatch(setBookingVisitId(params.visitId));
                        dispatch(setBookingModalView(BookingModalView.VERIFY_PHONE_NUMBER));
                    }
                    else {
                        dispatch(setBookingModalError(ex.error.data));
                        dispatch(setBookingModalView(BookingModalView.ERROR));
                    }
                }
            }
        }),
        cancelBooking: builder.mutation<void, number>({
            query: (bookingId) => ({
                url: `booking/cancel/${bookingId}`,
                method: 'PUT'
            }),
            invalidatesTags: ['CustomerBooking', 'Event']
        }),
        cancelVisit: builder.mutation<void, number>({
            query: (visitId) => ({
                url: `visit/cancel/${visitId}`,
                method: 'PUT'
            }),
            invalidatesTags: ['CompanyVisit', 'Event']
        }),
        addNewVisit: builder.mutation<void, NewVisit>({
            query: (visit) => ({
                url: `visit/add-new`,
                method: 'POST',
                body: visit
            }),
            invalidatesTags: ['CompanyVisit', 'Event'],
            async onQueryStarted(_, { queryFulfilled }) {
                try {
                    await queryFulfilled;
                    showSuccesNotification('Pomyślnie dodane nowy termin wizyty.');

                } catch (error: any) {
                    showErrorNotification('Nie udało się dodać nowej wizyty.');
                }
            }
        }),
        confirmBooking: builder.mutation<void, { bookingId: number, visitId: number }>({
            query: (params) => ({
                url: `booking/confirm/${params.bookingId}`,
                method: 'PUT'
            }),
            async onQueryStarted(params, { dispatch, queryFulfilled }) {
                const result = dispatch(
                    meetgoApi.util.updateQueryData('getCompanyVisits', undefined, (draft) => {
                        const visit = draft.find(v => v.id === params.visitId);
                        if (!visit || visit.bookings?.length < 1) return;

                        const booking = visit.bookings.find(b => b.id === params.bookingId);
                        if (!booking) return;
                        booking.isConfirmed = true;
                    })
                )
                try {
                    await queryFulfilled;
                    showSuccesNotification('Rezerwacja została potwierdzona.');
                } catch {
                    result.undo();
                    showErrorNotification('Nie udało się potwierdzić rezerwacji.');
                }
            },
        }),
        addPhoneNumber: builder.mutation<void, string>({
            query: (phoneNumber) => ({
                url: 'phonenumber/add',
                method: 'POST',
                body: { phoneNumber }
            }),
            async onQueryStarted(_phoneNumber, { dispatch, queryFulfilled }) {
                try {
                    dispatch(setBookingModalView(BookingModalView.SPINNER));
                    await queryFulfilled;
                    dispatch(setBookingModalView(BookingModalView.VERIFY_PHONE_NUMBER));
                } catch (error: any) {
                    dispatch(setBookingModalView(BookingModalView.ERROR));
                    dispatch(setBookingModalError(BookingErrorType.ADD_PHONE_FAILURE));
                }
            }
        }),
        verifyPhoneNumber: builder.mutation<void, string>({
            query: (token) => ({
                url: 'phonenumber/verify',
                method: 'POST',
                body: { token }
            }),
            async onQueryStarted(_token, { dispatch, queryFulfilled }) {
                try {
                    dispatch(setBookingModalView(BookingModalView.SPINNER));
                    await queryFulfilled;
                    dispatch(setBookingModalView(BookingModalView.AUTOBOOKING))
                    showSuccesNotification('Pomyślnie zweryfikowano numer telefonu.');

                } catch (error: any) {
                    dispatch(setBookingModalView(BookingModalView.ERROR));
                    dispatch(setBookingModalError(BookingErrorType.WRONG_SMS_TOKEN));
                }
            }
        }),
        requestVerificationSms: builder.mutation<void, void>({
            query: () => ({
                url: 'phonenumber/request-token',
                method: 'PUT'
            }),
            async onQueryStarted(_, { dispatch, queryFulfilled }) {
                try {
                    await queryFulfilled;
                } catch (error: any) {
                    dispatch(setBookingModalView(BookingModalView.ERROR));
                    dispatch(setBookingModalError(BookingErrorType.SMS_FAILURE));
                }
            }
        })
    })
})

export const {
    useGetUserClaimsQuery,
    useGetEventsQuery,
    useMakeBookingMutation,
    useCancelBookingMutation,
    useGetClientBookingsQuery,
    useGetCompanyVisitsQuery,
    useCancelVisitMutation,
    useConfirmBookingMutation,
    useAddPhoneNumberMutation,
    useVerifyPhoneNumberMutation,
    useRequestVerificationSmsMutation,
    useAddNewVisitMutation,
    useGetEventNamesQuery
} = meetgoApi;