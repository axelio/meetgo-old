export const preparePolishPhoneNumber = (number: string) => {
    const parts = number.replace('+48', '').match(/.{1,3}/g);
    return parts ? parts.join("-") : number;
}

export const prepareWebsiteToDisplay = (website: string) => website.replace(/(^\w+:|^)\/\//, '');