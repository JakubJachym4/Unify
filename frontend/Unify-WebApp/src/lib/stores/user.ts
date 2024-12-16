import { writable } from 'svelte/store';

export interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
}

export const user = writable<User | null>(null);
