export const setObjectInStorage = (key: string, value: any) => {
    try {
        window.localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
        console.log(`Error while saving to the storage:  ${error}`);
    }
}

export const setValueInStorage = (key: string, value: any) => {
    try {
        window.localStorage.setItem(key, value);
    } catch (error) {
        console.log(`Error while saving to the storage:  ${error}`);
    }
}

export const getValueFromStorage = (key: string) => {
    try {
        const item = window.localStorage.getItem(key);
        return item ? item : null;
    } catch (error) {
        console.log(`Error while trying to read from the storage: ${error}`);
        return null;
    }
}

export const getObjectFromStorage = (key: string) => {
    try {
        const item = window.localStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    } catch (error) {
        console.log(`Error while trying to read from the storage: ${error}`);
        return null;
    }
}