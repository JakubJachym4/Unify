import { writable } from 'svelte/store';
import type { UniversityInformation } from '../api/Common/UniversityInfoRequest';
import { getUniversityInformation } from '../api/Common/UniversityInfoRequest';
import type { AcademicLocation, Faculty, FieldOfStudy, Specialization } from '$lib/types/university';
import { getAllFieldsOfStudy } from '$lib/api/Admin/FieldsOfStudy/FieldsOfStudyRequests';
import { getAllSpecializations } from '$lib/api/Admin/Specialization/SpecializationRequests';
import { GetAllFaculties } from '$lib/api/Admin/Faculty/FacultyRequests';
import { GetAllLocations } from '$lib/api/Common/LocationRequests';


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

function createFieldOfStudyStore() {
    const { subscribe, set } = writable<FieldOfStudy[]>([]);

    return {
        subscribe,
        load: async (token: string) => {
            try {
                const fields = await getAllFieldsOfStudy(token);
                set(fields);
            } catch (error) {
                console.error('Failed to load fields of study:', error);
            }
        }
    };
}

function createSpecializationStore() {
    const { subscribe, set } = writable<Specialization[]>([]);

    return {
        subscribe,
        load: async (token: string) => {
            try {
                const specializations = await getAllSpecializations(token);
                set(specializations);
            } catch (error) {
                console.error('Failed to load specializations:', error);
            }
        }
    };
}

function createFacultyStore() {
    const { subscribe, set } = writable<Faculty[]>([]);

    return {
        subscribe,
        load: async (token: string) => {
            try {
                const fields = await GetAllFaculties(token);
                set(fields);
            } catch (error) {
                console.error('Failed to load fields of study:', error);
            }
        }
    };
}

function createLocationStore() {
    const { subscribe, set } = writable<AcademicLocation[]>([]);

    return {
        subscribe,
        load: async (token: string) => {
            try {
                const locations = await GetAllLocations(token);
                set(locations);
            } catch (error) {
                console.error('Failed to load locations:', error);
            }
        }
    };
}

export const universityInformationStore = createUniversityStore();
export const fieldOfStudiesStore = createFieldOfStudyStore();
export const specializationsStore = createSpecializationStore();
export const facultiesStore = createFacultyStore();
export const locationsStore = createLocationStore();