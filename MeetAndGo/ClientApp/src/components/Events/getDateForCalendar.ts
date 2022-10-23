import { getObjectFromStorage } from "../../common/services/localStorage";
import { getDateCalendarEnFormat } from "../../common/helpers/dateHelpers";

export default () => {
    const today = new Date();
    try {
      const storageValue = getObjectFromStorage('date');
      if (!storageValue) {
        return getDateCalendarEnFormat(today);
      }
      else {
        const storageDate = new Date(storageValue);
        storageDate.setHours(0, 0, 0, 0);
        today.setHours(0, 0, 0, 0);
        return getDateCalendarEnFormat(storageDate < today ? today : storageDate);
      }
    } catch (error) {
      return getDateCalendarEnFormat(today);
    }
  }