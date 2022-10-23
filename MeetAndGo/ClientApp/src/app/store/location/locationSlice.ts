import { createSlice, PayloadAction } from "@reduxjs/toolkit"
import { RootState } from "../store";
import { Coordinates } from "./types";

export interface LocationState {
    position?: Coordinates,
}

const initialState: LocationState = {};

export const locationSlice = createSlice({
    name: 'locationSlice',
    initialState,
    reducers: {
        setLocation: (state, action: PayloadAction<Coordinates>) => {
            state.position = action.payload;
        }
    }
})

export const { setLocation } = locationSlice.actions;

export const selectLocation = (state: RootState) => state.location.position;

export default locationSlice.reducer;