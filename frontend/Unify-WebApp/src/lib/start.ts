import { getUserData, getAllUsers } from "./api/User/UserRequests";
import { universityInformationStore } from "./stores/university";
import { user } from "./stores/user";
import { globalUsers } from "./stores/globalUsers";
import { goto } from "$app/navigation";

export const Load = async () => {
    await universityInformationStore.load();

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
            globalUsers.set([]);
            document.cookie = `user=false; Path=/; SameSite=Strict`;
            goto('/login');
        }
    } else {
        document.cookie = `user=false; Path=/; SameSite=Strict`;
    }
    return {};
}