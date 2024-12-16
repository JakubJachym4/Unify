import { getUserData } from "./api/User/UserRequests";
import { universityInformation } from "./stores/university";
import { user } from "./stores/user";

export const Load = async () => {
    await universityInformation.load();

    const token = localStorage.getItem('token');
    if (token) {
        try {
            const userData = await getUserData(token);
            user.set(userData);
            // Set cookie for redirect middleware
            document.cookie = `user=true; Path=/; SameSite=Strict`;
        } catch (error) {
            localStorage.removeItem('token');
        }
    }
    return {}
  }