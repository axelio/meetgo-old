import { Coordinates } from "../../app/store/location/types";

export function getDistance(first: Coordinates, second: Coordinates) {
    const R = 6371; // Radius of the earth in km
    const dLat = deg2rad(second.latitude - first.latitude);  // deg2rad below
    const dLon = deg2rad(second.longitude - second.longitude);
    const a =
        Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(deg2rad(first.latitude)) * Math.cos(deg2rad(second.latitude)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    const distanceInKm = R * c; // Distance in km
    return distanceInKm.toFixed(1);
}

function deg2rad(deg: number) {
    return deg * (Math.PI / 180)
}