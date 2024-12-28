import { writable } from 'svelte/store';
import type { UserResponse } from '$lib/api/User/UserRequests';
import { getAllUsers } from '$lib/api/User/UserRequests';

function createGlobalUsersStore() {
    const { subscribe, set } = writable<UserResponse[]>([]);

    return {
        subscribe,
        set,
        refresh: async () => {
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const users = await getAllUsers(token);
                    set(users);
                } catch (error) {
                    console.error('Failed to refresh users:', error);
                }
            }
        }
    };
}

export const globalUsers = createGlobalUsersStore();