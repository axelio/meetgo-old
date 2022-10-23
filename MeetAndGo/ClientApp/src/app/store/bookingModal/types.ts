export enum ErrorApi {
    ALREADY_BOOKED = 'ALREADY_BOOKED',
    TOO_MANY_BOOKINGS = 'TOO_MANY_BOOKINGS',
    SMS_FAILURE = 'SMS_FAILURE',
    NO_PHONE_NUMBER = 'NO_PHONE_NUMBER',
    PHONE_NUMBER_NOT_VERIFIED = 'PHONE_NUMBER_NOT_VERIFIED',
    WRONG_SMS_TOKEN = 'WRONG_SMS_TOKEN'
}

export enum BookingErrorType {
    ALREADY_BOOKED = 'ALREADY_BOOKED',
    TOO_MANY_BOOKINGS = 'TOO_MANY_BOOKINGS',
    SMS_FAILURE = 'SMS_FAILURE',
    DEFAULT = 'DEFAULT',
    ADD_PHONE_FAILURE = 'ADD_PHONE_FAILURE',
    WRONG_SMS_TOKEN = 'WRONG_SMS_TOKEN'
}


export enum BookingModalView {
    FORM = 'form',
    SPINNER = 'spinner',
    ERROR = 'error',
    ADD_PHONE_NUMBER = 'addphonenumber',
    VERIFY_PHONE_NUMBER = 'verifyphonenumber',
    SUCCESS = 'success',
    AUTOBOOKING = 'autobooking'
}