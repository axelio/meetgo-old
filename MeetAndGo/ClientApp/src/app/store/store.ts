import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import { meetgoApi } from '../api/meetgoApi';
import bookingModalReducer from './bookingModal/bookingModalSlice';
import locationReducer from './location/locationSlice';

export const store = configureStore({
  reducer: {
    [meetgoApi.reducerPath]: meetgoApi.reducer,
    bookingModal: bookingModalReducer,
    location: locationReducer
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(meetgoApi.middleware),
  devTools: true
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
