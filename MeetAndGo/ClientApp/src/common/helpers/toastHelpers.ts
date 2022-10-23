import { toast } from "react-toastify";

export const showErrorNotification = (msg: string) => {
    toast.error(msg, {
        position: "bottom-right",
        autoClose: 3500,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: false,
        draggable: true,
        progress: undefined,
        theme: 'colored'
    });
}

export const showSuccesNotification = (msg: string) => {
    toast.success(msg, {
        position: "bottom-right",
        autoClose: 3500,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: false,
        draggable: true,
        progress: undefined,
        theme: 'colored' 
    });
}