export const getNowDate = () => new Date(new Date().toLocaleString("en-US", { timeZone: "Europe/Warsaw" }));

export const getHours = (date: Date) => date.toLocaleTimeString('pl-PL', { hour: 'numeric', minute: 'numeric' })

export const prepareDateToDisplay = (date: Date) => `${date.toLocaleDateString('pl-PL')} - ${getHours(date)}`;

export const getDateCalendarEnFormat = (date: Date) => date.toLocaleDateString("en-CA", { timeZone: "Europe/Warsaw" });

export const getNowDateCalendarEnFormat = () => new Date().toLocaleDateString("en-CA", { timeZone: "Europe/Warsaw" });

export const getNowDateCalendarTimeEnFormat = () => `${new Date().toLocaleDateString("en-CA", { timeZone: "Europe/Warsaw" })}T00:00`;

export const transformCalendarDateFromEnToPl = (date: string) => new Date(date).toLocaleDateString("pl-PL", { timeZone: "Europe/Warsaw" });

export const getTomorrowDate = (date: string) => {
    const newDate = new Date(date);
    newDate.setDate(newDate.getDate() + 1);
    return newDate;
}