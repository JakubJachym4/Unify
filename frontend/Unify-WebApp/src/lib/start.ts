import { getUserData, getAllUsers } from "./api/User/UserRequests";
import { universityInformation } from "./stores/university";
import { user } from "./stores/user";
import { globalUsers } from "./stores/globalUsers";

export const Load = async () => {
    await universityInformation.load();

    const token = localStorage.getItem('token');
    if (token) {
        try {
            const userData = await getUserData(token);
            user.set(userData);
            // Set cookie for redirect middleware
            document.cookie = `user=true; Path=/; SameSite=Strict`;

            // Fetch all users and set the globalUsers store
            const users = await getAllUsers(token);
            globalUsers.set(users);
            
        } catch (error) {
            localStorage.removeItem('token');
            document.cookie = `user=false; Path=/; SameSite=Strict`;
        }
    } else {
        document.cookie = `user=false; Path=/; SameSite=Strict`;
    }
    return {};
}