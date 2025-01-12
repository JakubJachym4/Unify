import { writable } from 'svelte/store';
import type { UserResponse } from '$lib/api/User/UserRequests';
import { getAllUsers } from '$lib/api/User/UserRequests';

const STORAGE_KEY = 'globalUsers';

function createGlobalUsersStore() {
    const storedUsers = typeof localStorage !== 'undefined' 
        ? JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]') 
        : [];

    const { subscribe, set } = writable<UserResponse[]>(storedUsers);

    return {
        subscribe,
        set: (users: UserResponse[]) => {
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(users));
            }
            set(users);
        },
        refresh: async () => {
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const users = await getAllUsers(token);
                    if (typeof localStorage !== 'undefined') {
                        localStorage.setItem(STORAGE_KEY, JSON.stringify(users));
                    }
                    set(users);
                } catch (error) {
                    console.error('Failed to refresh users:', error);
                }
            }
        }
    };
}

export const globalUsers = createGlobalUsersStore();