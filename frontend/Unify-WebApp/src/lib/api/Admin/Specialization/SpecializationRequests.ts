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