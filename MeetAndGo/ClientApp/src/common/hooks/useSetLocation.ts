import { useEffect } from "react";
import { setLocation } from "../../app/store/location/locationSlice";
import { useAppDispatch } from '../../app/store/hooks';

export function useSetLocation() {
    const dispatch = useAppDispatch();
    useEffect(() => {

        const onSuccess = (position: any) => dispatch(setLocation({ latitude: position.coords.latitude, longitude: position.coords.longitude }));

        const error = () => {};

        navigator.geolocation.getCurrentPosition(onSuccess, error, { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 });
    }, [dispatch]);
}