import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { RootState } from "../store";
import { BookingErrorType, BookingModalView } from "./types";

export interface BookingModalState {
    bookingModalView: BookingModalView,
    bookingError?: BookingErrorType,
    visitId?: number
}

const initialState: BookingModalState = {
    bookingModalView: BookingModalView.FORM
}

export const bookingModalSlice = createSlice({
    name: 'bookingModal',
    initialState,
    reducers: {
        setBookingModalView: (state, action: PayloadAction<BookingModalView>) => {
            state.bookingModalView = action.payload
        },
        setBookingModalError: (state, action: PayloadAction<BookingErrorType>) => {
            state.bookingError = action.payload;
        },
        setBookingVisitId: (state, action: PayloadAction<number>) => {
            state.visitId = action.payload;
        },
        resetState: (state) => {
            state.bookingModalView = BookingModalView.FORM;
            delete state.bookingError;
            delete state.visitId;
        }
    }
});

export const { setBookingModalView, setBookingModalError, resetState, setBookingVisitId } = bookingModalSlice.actions;

export const selectBookingModalView = (state: RootState) => state.bookingModal.bookingModalView;
export const selectBookingModalError = (state: RootState) => state.bookingModal.bookingError;
export const selectBookingVisitId = (state: RootState) => state.bookingModal.visitId;

export default bookingModalSlice.reducer;