import { writable } from 'svelte/store';
import type { UniversityInformation } from '../api/Common/UniversityInfoRequest';
import { getUniversityInformation } from '../api/Common/UniversityInfoRequest';


function createUniversityStore() {
    const { subscribe, set } = writable<UniversityInformation | null>(null);

    return {
        subscribe,
        load: async () => {
            try {
                const info = await getUniversityInformation();
                set(info);
            } catch (error) {
                console.error('Failed to load university info:', error);
            }
        }
    };
}

export const universityInformation = createUniversityStore();