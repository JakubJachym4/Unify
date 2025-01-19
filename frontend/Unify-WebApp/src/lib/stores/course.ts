import { writable } from 'svelte/store';
import type { Course } from '$lib/types/universityClasses';

function createCourseStore() {
    const { subscribe, set, update } = writable<Course[]>([]);

    return {
        subscribe,
        set,
        update,
        reset: () => set([])
    };
}

export const courseStore = createCourseStore();