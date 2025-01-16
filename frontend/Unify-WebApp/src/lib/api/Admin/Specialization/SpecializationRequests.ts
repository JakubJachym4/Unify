import { api } from '$lib/api/api';
import type { Specialization } from '$lib/types/university';

export const getAllSpecializations = async (token: string) => {
    return await api<Specialization[]>('GET','/specializations',  null, token
    );}

export const addSpecialization = async (data: { name: string, description: string, fieldOfStudyId: string }, token: string) =>{
    return await api<string>('POST','/specializations', data, token
    )};

export const updateSpecialization = async (id: string, data: { name: string, description: string }, token: string) => {
    return await api('PUT', `/specializations/${id}`,
        { id, ...data },
        token
    )};

export const deleteSpecialization = async (id: string, token: string) => {
    return await api('DELETE', `/specializations/${id}`,
        null,
        token
    )};



export const getSpecializationUsers = async (specializationId: string, token: string) => {
    return await api<string[]>('GET', `/specializations/${specializationId}/students`,
        null,
        token
    )
}

export const assignUserToSpecialization = async (specializationId: string, userId: string, token: string) => {
    return await api('POST', `/specializations/assign-student`,
        { StudentId: userId, SpecializationId: specializationId },
        token
    )
}

export const unassignUserToSpecialization = async (specializationId: string, userId: string, token: string) => {
    return await api('DELETE', `/specializations/unassign-student`,
        { StudentId: userId, SpecializationId: specializationId },
        token
    )
}