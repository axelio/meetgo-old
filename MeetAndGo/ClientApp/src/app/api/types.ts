export type Visit = {
    id: number,
    startDate: Date,
    price: number,
    maxPersons: number,
    bookingsNumber: number
}

export type Event = {
    id: number,
    name: string,
    description: string,
    durationInMinutes: number | null,
    pictureUrl: string,
    address: Address,
    visits: Visit[],
    requiresConfirmation: boolean,
    order: number,
    kind: number
}

export type GetEventsQueryResponse = {
    events: { [id: number]: Event },
    lastVisitId: number | null,
    visitsCount: number
}

export type Address = {
    street: string,
    number: string,
    companyName: string,
    district: string,
    website: string,
    longitude: number,
    latitude: number
}

export type CustomerBooking = {
    id: number
    visitStartDate: Date,
    eventName: string,
    eventKind: number,
    price: number,
    maxPersons: number,
    companyName: string,
    isConfirmed: boolean,
    bookingsNumber: number,
    code: string
}

export type CompanyVisit = {
    id: number
    startDate: Date,
    eventName: string,
    eventKind: number,
    price: number,
    bookingsNumber: number,
    maxPersons: number,
    requiresConfirmation: boolean,
    bookings: [{
        id: number,
        customerMail: string,
        isConfirmed: boolean,
        customerPhoneNumber: string,
        customerName: string,
        code: string
    }]
}

export type GetEventsQueryParams = {
    day: string,
    cityId: number,
    lastVisitId?: number | null,
    timeOfDay?: number,
    categoryId?: number
};

export type MakeBookingParams = {
    visitId: number,
    getEventsParams: GetEventsQueryParams,
    eventId: number
}

export type NewVisit = {
    date: string,
    eventId: number,
    maxPersons: number,
    price: number
}

export type EventName = {
    id: number,
    name: string,
    kind: number
}